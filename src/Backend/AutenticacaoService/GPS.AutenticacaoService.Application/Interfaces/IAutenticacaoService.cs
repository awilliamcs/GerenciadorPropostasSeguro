using GPS.CrossCutting;
using GPS.AutenticacaoService.Application.DTOs;

namespace GPS.AutenticacaoService.Application.Interfaces
{
    public interface IAutenticacaoService
    {
        Task<(bool Success, object Result, IEnumerable<string> Errors)> CriarUsuarioComPessoaAsync(RegisterDto dto, CancellationToken ct = default);
        Task<(bool Success, object Result, IEnumerable<string> Errors)> CriarUsuarioAsync(RegisterDto dto, CancellationToken ct = default);
        Task<(bool Success, object Result, IEnumerable<string> Errors)> ConsultarStatusSagaAsync(Guid sagaId, CancellationToken ct = default);
        Task<Result<AutenticacaoResponseDto>> EfetuarLoginAsync(LoginDto dto, CancellationToken ct = default);
    }
}
