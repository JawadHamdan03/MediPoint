using FluentValidation;

namespace MediPoint.Application.Features.Login;



public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x=>x.email).NotEmpty().WithMessage("Email can not be empty");
        RuleFor(x=>x.password).NotEmpty().WithMessage("Password can not be empty");

    }
}