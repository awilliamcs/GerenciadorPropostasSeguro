using GPS.PessoaService.Domain.Entidades;

namespace GPS.PessoaService.Domain.Interfaces
{
    public interface IPessoaRepository
    {
        Task<Pessoa> SalvarAsync(Pessoa pessoa, CancellationToken ct = default);
        Task<Pessoa?> ObterPorIdAsync(Guid idPessoa, CancellationToken ct = default);
    }
}
