using FluentValidation;

namespace MediPoint.Application.Features.Doctors.ForgotPassword;

public class DoctorForgotPasswordCommandValidator : AbstractValidator<DoctorForgotPasswordCommand>
{
    public DoctorForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}
