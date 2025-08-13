using GPS.PessoaService.Application.DTOs;

namespace GPS.PessoaService.Application.Interfaces
{
    public interface IPessoaService
    {
        Task SalvarAsync(PessoaDto pessoaDto, CancellationToken ct = default);
        Task<PessoaDto?> ObterPorIdAsync(Guid idPessoa, CancellationToken ct = default);
    }
}