using FluentValidation;
using GPS.CrossCutting.Enums;
using GPS.PropostaService.Application.DTOs;

namespace GPS.PropostaService.Application.Validators
{
    public class PropostaDtoValidator : AbstractValidator<PropostaDto>
    {
        public PropostaDtoValidator()
        {
            RuleFor(p => p.IdPessoa)
                .NotEmpty().WithMessage("IdPessoa é obrigatório.");

            RuleFor(p => p.Tipo)
                .Must(tipo => Enum.IsDefined(typeof(TipoProposta), tipo))
                .WithMessage("Tipo de proposta inválido.");

            RuleFor(p => p.Valor)
                .GreaterThan(0).WithMessage("Valor deve ser maior que zero.");
        }
    }
}
