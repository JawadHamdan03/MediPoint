using FluentValidation;

namespace MediPoint.Application.Features.Patients.SignUp;

public class SignUpPatientCommandValidator : AbstractValidator<SignUpPatientCommand>
{
    public SignUpPatientCommandValidator()
    {
        RuleFor(x => x.Request.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.Request.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email address cannot exceed 100 characters.");

        RuleFor(x => x.Request.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[\^$*.[\]{}()?""!@#%&/\\,><':;|_~`]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.Request.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number must be a valid international format (E.164).");

        RuleFor(x => x.Request.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .Must(NotBeInFuture).WithMessage("Date of birth cannot be in the future.")
            .Must(BeValidAge).WithMessage("Date of birth is not valid.");

        RuleFor(x => x.Request.Gender)
            .IsInEnum().WithMessage("A valid gender selection is required.");

        RuleFor(x => x.Request.BloodType)
            .MaximumLength(10).WithMessage("Blood type cannot exceed 10 characters.");

        RuleFor(x => x.Request.Address)
            .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");

        RuleFor(x => x.Request.EmergencyContactName)
            .MaximumLength(100).WithMessage("Emergency contact name cannot exceed 100 characters.");

        RuleFor(x => x.Request.EmergencyContactPhone)
            .MaximumLength(20).WithMessage("Emergency contact phone cannot exceed 20 characters.");
    }

    private bool NotBeInFuture(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return dateOfBirth <= today;
    }

    private bool BeValidAge(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return dateOfBirth >= today.AddYears(-120);
    }
}
