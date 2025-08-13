using GPS.CrossCutting;
using GPS.PessoaService.Application.DTOs;

namespace GPS.PessoaService.Application.Interfaces
{
    public interface IAutenticacaoService
    {
        Task<(bool Success, object Result, IEnumerable<string> Errors)> RegistrarUsuarioAsync(RegisterDto dto, CancellationToken ct = default);
        Task<Result<AutenticacaoResponseDto>> LoginAsync(LoginDto dto, CancellationToken ct = default);
    }
}
