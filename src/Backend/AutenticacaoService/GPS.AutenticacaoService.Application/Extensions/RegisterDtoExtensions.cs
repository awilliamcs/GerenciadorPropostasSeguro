using GPS.AutenticacaoService.Application.DTOs;
using GPS.AutenticacaoService.Application.Validators;

namespace GPS.AutenticacaoService.Application.Extensions
{
    public static class RegisterDtoExtensions
    {
        public static List<string> Validar(this RegisterDto dto)
        {
            var validator = new RegisterDtoValidator();
            var result = validator.Validate(dto);

            return result.IsValid ? [] : [.. result.Errors.Select(e => e.ErrorMessage)];
        }
    }
}