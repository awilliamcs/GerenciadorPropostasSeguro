namespace GPS.PessoaService.Application.DTOs
{
    public record AuthResponseDto(string Token, DateTime Expiration);
}
