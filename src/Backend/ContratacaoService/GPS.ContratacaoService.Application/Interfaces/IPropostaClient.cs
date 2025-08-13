using GPS.ContratacaoService.Application.DTOs;

namespace GPS.ContratacaoService.Application.Interfaces
{
    public interface IPropostaClient
    {
        Task<PropostaDto?> ObterPorIdAsync(Guid idProposta, CancellationToken ct = default);
    }
}
