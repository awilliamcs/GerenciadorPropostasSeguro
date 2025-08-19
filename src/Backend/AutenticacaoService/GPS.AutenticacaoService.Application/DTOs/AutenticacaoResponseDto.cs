namespace GPS.AutenticacaoService.Application.DTOs
{
    public record AutenticacaoResponseDto(
        string Token,
        DateTime Expiracao
    );
}
