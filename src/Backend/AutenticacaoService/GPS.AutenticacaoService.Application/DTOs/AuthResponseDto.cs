namespace GPS.AutenticacaoService.Application.DTOs
{
    public record AuthResponseDto(string Token, DateTime Expiration);
}
