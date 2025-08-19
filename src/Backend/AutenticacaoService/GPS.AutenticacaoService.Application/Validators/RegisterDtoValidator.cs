using FluentValidation;
using GPS.AutenticacaoService.Application.DTOs;

namespace GPS.AutenticacaoService.Application.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("E-mail é obrigatório.");

            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório.");

            RuleFor(p => p.Senha)
                .NotEmpty().WithMessage("Senha é obrigatória.");

            RuleFor(p => p.Telefone)
                .NotEmpty().WithMessage("Telefone é obrigatório.");

            RuleFor(p => p.DataNascimento)
                .NotEmpty().WithMessage("Data Nascimento é obrigatório.");

        }
    }
}
