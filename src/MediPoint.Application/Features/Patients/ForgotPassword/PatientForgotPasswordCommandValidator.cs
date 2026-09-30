using FluentValidation;

namespace MediPoint.Application.Features.Patients.ForgotPassword;

public class PatientForgotPasswordCommandValidator : AbstractValidator<PatientForgotPasswordCommand>
{
    public PatientForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}
