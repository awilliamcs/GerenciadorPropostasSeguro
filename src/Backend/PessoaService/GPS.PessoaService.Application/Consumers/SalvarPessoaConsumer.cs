using AutoMapper;
using GPS.CrossCutting.Messaging;
using GPS.PessoaService.Application.Commands;
using GPS.PessoaService.Application.DTOs;
using GPS.PessoaService.Application.Events;
using GPS.PessoaService.Domain.Entidades;
using GPS.PessoaService.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GPS.PessoaService.Application.Consumers
{
    public class SalvarPessoaConsumer(IPessoaRepository pessoaRepository, IMapper mapper, ILogger<SalvarPessoaConsumer> logger) : IConsumer<PessoaDto>
    {
        private readonly IPessoaRepository _pessoaRepository = pessoaRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<SalvarPessoaConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<PessoaDto> context)
        {
            try
            {
                var pessoa = _mapper.Map<Pessoa>(context.Message);
                await _pessoaRepository.SalvarAsync(pessoa, context.CancellationToken);

                _logger.LogInformation("Pessoa salva com sucesso. ID: {PessoaId}", pessoa.IdPessoa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar pessoa. ID: {PessoaId}", context.Message.IdPessoa);
                throw;
            }
        }
    }

    public class CriarPessoaComandoConsumer(IPessoaRepository pessoaRepository, IClienteMQ clienteMQ, ILogger<CriarPessoaComandoConsumer> logger) : IConsumer<CriarPessoaComando>
    {
        private readonly IPessoaRepository _pessoaRepository = pessoaRepository;
        private readonly IClienteMQ _clienteMQ = clienteMQ;
        private readonly ILogger<CriarPessoaComandoConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<CriarPessoaComando> context)
        {
            try
            {
                var pessoa = new Pessoa(context.Message.Nome, context.Message.Email, context.Message.Telefone, context.Message.DataNascimento);

                var pessoaSalva = await _pessoaRepository.SalvarAsync(pessoa, context.CancellationToken);

                var evento = new PessoaCriadaEvent
                {
                    SagaId = context.Message.SagaId,
                    PessoaId = new Guid(pessoaSalva.IdPessoa.ToString()),
                    Nome = pessoaSalva.Nome,
                    Email = pessoaSalva.Email,
                    Telefone = pessoaSalva.Telefone,
                    DataNascimento = pessoaSalva.DataNascimento
                };

                await _clienteMQ.PublicarMensagemParaFila(evento);

                _logger.LogInformation("Pessoa criada via SAGA. SagaId: {SagaId}, PessoaId: {PessoaId}", context.Message.SagaId, pessoaSalva.IdPessoa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pessoa via SAGA. SagaId: {SagaId}", context.Message.SagaId);

                var eventoFalha = new PessoaCriacaoFalhouEvent
                {
                    SagaId = context.Message.SagaId,
                    Erro = ex.Message
                };

                await _clienteMQ.PublicarMensagemParaFila(eventoFalha);
            }
        }
    }

    public class DeletarPessoaCompensacaoConsumer(ILogger<DeletarPessoaCompensacaoConsumer> logger) : IConsumer<DeletarPessoaCompensacaoEvent>
    {
        private readonly ILogger<DeletarPessoaCompensacaoConsumer> _logger = logger;

        public Task Consume(ConsumeContext<DeletarPessoaCompensacaoEvent> context)
        {
            try
            {
                _logger.LogInformation("Compensação executada para pessoa. SagaId: {SagaId}, PessoaId: {PessoaId}, Motivo: {Motivo}",
                    context.Message.SagaId, context.Message.PessoaId, context.Message.Motivo);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na compensação da pessoa. SagaId: {SagaId}, PessoaId: {PessoaId}", context.Message.SagaId, context.Message.PessoaId);
                throw;
            }
        }
    }
}
