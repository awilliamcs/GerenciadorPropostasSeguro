using GPS.PessoaService.Application.DTOs;
using GPS.PessoaService.Application.Validators;

namespace GPS.PessoaService.Application.Extensions
{
    public static class PessoaDtoExtensions
    {
        public static List<string> Validar(this PessoaDto dto)
        {
            var validator = new PessoaDtoValidator();
            var result = validator.Validate(dto);

            return result.IsValid ? [] : [.. result.Errors.Select(e => e.ErrorMessage)];
        }
    }
}