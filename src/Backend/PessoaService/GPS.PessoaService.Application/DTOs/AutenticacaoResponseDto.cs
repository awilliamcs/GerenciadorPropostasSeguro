namespace GPS.PessoaService.Application.DTOs
{
    public record AutenticacaoResponseDto(
        string Token,
        DateTime Expiracao
    );
}
