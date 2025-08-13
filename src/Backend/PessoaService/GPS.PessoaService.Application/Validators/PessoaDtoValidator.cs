using FluentValidation;
using GPS.PessoaService.Application.DTOs;

namespace GPS.PessoaService.Application.Validators
{
    public class PessoaDtoValidator : AbstractValidator<PessoaDto>
    {
        public PessoaDtoValidator()
        {
            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(150).WithMessage("O nome não pode ultrapassar 150 caracteres.");

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail informado não é válido.")
                .MaximumLength(150).WithMessage("O e-mail não pode ultrapassar 150 caracteres.");

            RuleFor(p => p.Telefone)
                .NotEmpty().WithMessage("O telefone é obrigatório.")
                .Matches(@"^\+?\d{8,15}$").WithMessage("O telefone deve conter apenas números e, opcionalmente, o código do país.")
                .MaximumLength(15).WithMessage("O telefone não pode ultrapassar 15 caracteres.");

            RuleFor(p => p.DataNascimento)
                .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
                .LessThan(DateTime.Today).WithMessage("A data de nascimento deve ser no passado.")
                .GreaterThan(DateTime.Today.AddYears(-120)).WithMessage("A data de nascimento não pode indicar mais de 120 anos de idade.");
        }
    }
}
