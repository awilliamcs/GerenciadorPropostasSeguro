using GPS.ContratacaoService.Application.DTOs;
using GPS.ContratacaoService.Application.Validators;

namespace GPS.ContratacaoService.Application.Extensions
{
    public static class ContratacaoDtoExtensions
    {
        public static List<string> Validar(this ContratacaoDto dto)
        {
            var validator = new ContratacaoDtoValidator();
            var result = validator.Validate(dto);

            return result.IsValid ? [] : [.. result.Errors.Select(e => e.ErrorMessage)];
        }
    }
}