using GPS.AutenticacaoService.Application.Events;
using GPS.AutenticacaoService.Application.Sagas;

namespace GPS.AutenticacaoService.Application.Interfaces
{
    public interface IRegistroUsuarioSagaOrchestrator
    {
        Task<Guid> IniciarSagaAsync(RegistroUsuarioSagaData sagaData, CancellationToken ct = default);
        Task ProcessarPessoaCriadaAsync(PessoaCriadaEvent evento, CancellationToken ct = default);
        Task ProcessarPessoaCriacaoFalhouAsync(PessoaCriacaoFalhouEvent evento, CancellationToken ct = default);
        Task ProcessarUsuarioCriadoAsync(UsuarioCriadoEvent evento, CancellationToken ct = default);
        Task<RegistroUsuarioSagaData?> ObterSagaAsync(Guid correlationId, CancellationToken ct = default);
    }
}
