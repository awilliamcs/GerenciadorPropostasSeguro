using GPS.PropostaService.Application.DTOs;

namespace GPS.PropostaService.Application.Interfaces
{
    public interface IPropostaService
    {
        Task SalvarAsync(PropostaDto propostaDto, CancellationToken ct = default);
        Task<PropostaListResponseDto> ObterAsync(int pagina, int quantidadeItens, CancellationToken ct = default);
        Task<PropostaDto?> ObterPorIdAsync(Guid idProposta, CancellationToken ct = default);
    }
}