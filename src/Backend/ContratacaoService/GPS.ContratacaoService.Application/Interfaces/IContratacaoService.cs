using GPS.ContratacaoService.Application.DTOs;

namespace GPS.ContratacaoService.Application.Interfaces
{
    public interface IContratacaoService
    {
        Task SalvarAsync(ContratacaoDto contratacaoDto, CancellationToken ct = default);
        Task<ContratacaoListResponseDto> ObterAsync(int pagina, int quantidadeItens, CancellationToken ct = default);
        Task<ContratacaoDto?> ObterPorIdAsync(Guid idContratacao, CancellationToken ct = default);
    }
}