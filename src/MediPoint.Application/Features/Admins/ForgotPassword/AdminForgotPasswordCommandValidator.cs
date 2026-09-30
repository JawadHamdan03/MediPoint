using FluentValidation;

namespace MediPoint.Application.Features.Admins.ForgotPassword;

public class AdminForgotPasswordCommandValidator : AbstractValidator<AdminForgotPasswordCommand>
{
    public AdminForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}
