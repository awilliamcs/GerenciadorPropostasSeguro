using FluentValidation;
using GPS.ContratacaoService.Application.DTOs;

namespace GPS.ContratacaoService.Application.Validators
{
    public class ContratacaoDtoValidator : AbstractValidator<ContratacaoDto>
    {
        public ContratacaoDtoValidator()
        {
            RuleFor(p => p.IdProposta)
                .NotEmpty().WithMessage("IdProposta é obrigatório.");
        }
    }
}
