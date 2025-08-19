using GPS.AutenticacaoService.Application.Commands;
using GPS.AutenticacaoService.Application.DTOs;
using GPS.AutenticacaoService.Application.Events;
using GPS.AutenticacaoService.Application.Interfaces;
using GPS.AutenticacaoService.Application.Sagas;
using GPS.AutenticacaoService.Domain.Entidades;
using GPS.CrossCutting.Messaging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GPS.AutenticacaoService.Application.Services
{
    public class RegistroUsuarioSagaOrchestrator(
        UserManager<ApplicationUser> userManager,
        IClienteMQ clienteMQ,
        ILogger<RegistroUsuarioSagaOrchestrator> logger) : IRegistroUsuarioSagaOrchestrator
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IClienteMQ _clienteMQ = clienteMQ;
        private readonly ILogger<RegistroUsuarioSagaOrchestrator> _logger = logger;
        private readonly Dictionary<Guid, RegistroUsuarioSagaData> _sagas = [];

        public async Task<Guid> IniciarSagaAsync(RegistroUsuarioSagaData sagaData, CancellationToken ct = default)
        {
            sagaData.SagaId = Guid.NewGuid();
            sagaData.Status = RegistroUsuarioSagaStatus.Iniciado;
            sagaData.DataInicio = DateTime.UtcNow;

            _sagas[sagaData.SagaId] = sagaData;
            _logger.LogInformation("SAGA iniciada. SagaId: {SagaId}", sagaData.SagaId);

            var dadosRegistro = (RegisterDto)sagaData.DadosRegistro;
            var comando = new CriarPessoaComando
            {
                SagaId = sagaData.SagaId,
                Nome = dadosRegistro.Nome,
                Email = dadosRegistro.Email,
                Telefone = dadosRegistro.Telefone,
                DataNascimento = dadosRegistro.DataNascimento
            };

            await _clienteMQ.PublicarMensagemParaFila(comando);
            return sagaData.SagaId;
        }

        public async Task ProcessarPessoaCriadaAsync(PessoaCriadaEvent evento, CancellationToken ct = default)
        {
            if (!_sagas.TryGetValue(evento.SagaId, out var saga))
            {
                _logger.LogWarning("SAGA não encontrada. SagaId: {SagaId}", evento.SagaId);
                return;
            }

            if (saga.Status != RegistroUsuarioSagaStatus.Iniciado)
            {
                _logger.LogWarning("SAGA em estado inválido. SagaId: {SagaId}, Estado: {Status}", evento.SagaId, saga.Status);
                return;
            }

            saga.Status = RegistroUsuarioSagaStatus.PessoaCriada;
            saga.PessoaCriada = new
            {
                IdPessoa = evento.PessoaId,
                evento.Nome,
                evento.Email,
                evento.Telefone,
                evento.DataNascimento
            };

            _logger.LogInformation("Pessoa criada na SAGA. SagaId: {SagaId}, PessoaId: {PessoaId}", evento.SagaId, evento.PessoaId);

            try
            {
                var dadosRegistro = (RegisterDto)saga.DadosRegistro;
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = dadosRegistro.Email,
                    Email = dadosRegistro.Email,
                    EmailConfirmed = true,
                    PessoaId = evento.PessoaId
                };

                var result = await _userManager.CreateAsync(user, dadosRegistro.Senha);
                if (result.Succeeded)
                {
                    saga.Status = RegistroUsuarioSagaStatus.UsuarioCriado;
                    saga.UserId = user.Id;

                    var usuarioCriadoEvent = new UsuarioCriadoEvent
                    {
                        SagaId = evento.SagaId,
                        UserId = user.Id,
                        PessoaId = evento.PessoaId,
                        Email = dadosRegistro.Email
                    };

                    await _clienteMQ.PublicarMensagemParaFila(usuarioCriadoEvent);

                    saga.Status = RegistroUsuarioSagaStatus.Finalizado;
                    saga.DataFim = DateTime.UtcNow;

                    _logger.LogInformation("SAGA finalizada com sucesso. SagaId: {SagaId}", evento.SagaId);
                }
                else
                {
                    saga.Erros.AddRange(result.Errors.Select(e => e.Description));
                    await CompensarPessoaAsync(saga);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário na SAGA. SagaId: {SagaId}", evento.SagaId);
                saga.Erros.Add($"Erro ao criar usuário: {ex.Message}");
                await CompensarPessoaAsync(saga);
            }
        }

        public Task ProcessarPessoaCriacaoFalhouAsync(PessoaCriacaoFalhouEvent evento, CancellationToken ct = default)
        {
            if (!_sagas.TryGetValue(evento.SagaId, out var saga))
            {
                _logger.LogWarning("SAGA não encontrada. SagaId: {SagaId}", evento.SagaId);
                return Task.CompletedTask;
            }

            saga.Status = RegistroUsuarioSagaStatus.Falhou;
            saga.Erros.Add(evento.Erro);
            saga.DataFim = DateTime.UtcNow;

            _logger.LogError("SAGA falhou na criação da pessoa. SagaId: {SagaId}, Erro: {Erro}", evento.SagaId, evento.Erro);

            return Task.CompletedTask;
        }

        public Task ProcessarUsuarioCriadoAsync(UsuarioCriadoEvent evento, CancellationToken ct = default)
        {
            _logger.LogInformation("Usuário criado com sucesso. SagaId: {SagaId}, UserId: {UserId}", evento.SagaId, evento.UserId);
            return Task.CompletedTask;
        }

        private async Task CompensarPessoaAsync(RegistroUsuarioSagaData saga)
        {
            saga.Status = RegistroUsuarioSagaStatus.CompensandoPessoa;

            if (saga.PessoaCriada != null)
            {
                var pessoaCriadaObj = saga.PessoaCriada as dynamic;
                var compensacaoEvent = new DeletarPessoaCompensacaoEvent
                {
                    SagaId = saga.SagaId,
                    PessoaId = (Guid)pessoaCriadaObj!.IdPessoa,
                    Motivo = "Falha na criação do usuário"
                };

                await _clienteMQ.PublicarMensagemParaFila(compensacaoEvent);

                var pessoaId = (Guid)pessoaCriadaObj!.IdPessoa;
                _logger.LogInformation("Compensação iniciada para a pessoa. SagaId: {SagaId}, PessoaId: {PessoaId}", saga.SagaId, pessoaId);
            }

            saga.Status = RegistroUsuarioSagaStatus.Falhou;
            saga.DataFim = DateTime.UtcNow;
        }

        public Task<RegistroUsuarioSagaData?> ObterSagaAsync(Guid correlationId, CancellationToken ct = default)
        {
            _sagas.TryGetValue(correlationId, out var saga);
            return Task.FromResult(saga);
        }
    }
}
