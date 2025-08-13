using GPS.PropostaService.Application.DTOs;
using GPS.PropostaService.Application.Validators;

namespace GPS.PropostaService.Application.Extensions
{
    public static class PropostaDtoExtensions
    {
        public static List<string> Validar(this PropostaDto dto)
        {
            var validator = new PropostaDtoValidator();
            var result = validator.Validate(dto);

            return result.IsValid ? [] : [.. result.Errors.Select(e => e.ErrorMessage)];
        }
    }
}