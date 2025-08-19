using FluentValidation;
using GPS.AutenticacaoService.Application.DTOs;

namespace GPS.AutenticacaoService.Application.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("E-mail é obrigatório.");

            RuleFor(p => p.Senha)
                .NotEmpty().WithMessage("Senha é obrigatória.");
        }
    }
}
