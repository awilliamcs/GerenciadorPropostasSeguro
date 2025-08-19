using GPS.AutenticacaoService.Domain.Entidades;

namespace GPS.AutenticacaoService.Domain.Interfaces
{
    public interface IAutenticacaoRepository
    {
        Task<ApplicationUser> SalvarAsync(ApplicationUser user, CancellationToken ct = default);
        Task<ApplicationUser?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    }
}
