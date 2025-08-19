using GPS.AutenticacaoService.Application.DTOs;
using GPS.AutenticacaoService.Application.Validators;

namespace GPS.AutenticacaoService.Application.Extensions
{
    public static class LoginDtoExtensions
    {
        public static List<string> Validar(this LoginDto dto)
        {
            var validator = new LoginDtoValidator();
            var result = validator.Validate(dto);

            return result.IsValid ? [] : [.. result.Errors.Select(e => e.ErrorMessage)];
        }
    }
}