using FluentValidation;

namespace MediPoint.Application.Features.Admins.RegisterPatient;

public class RegisterPatientCommandValidator : AbstractValidator<RegisterPatientCommand>
{
    public RegisterPatientCommandValidator()
    {
        RuleFor(x => x.patientRequest.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.patientRequest.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.patientRequest.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email address cannot exceed 100 characters.");

        RuleFor(x => x.patientRequest.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[\^$*.[\]{}()?""!@#%&/\\,><':;|_~`]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.patientRequest.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number must be a valid international format (E.164).");

        RuleFor(x => x.patientRequest.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .Must(NotBeInFuture).WithMessage("Date of birth cannot be in the future.")
            .Must(BeValidAge).WithMessage("Date of birth is not valid.");

        RuleFor(x => x.patientRequest.Gender)
            .IsInEnum().WithMessage("A valid gender selection is required.");

        RuleFor(x => x.patientRequest.BloodType)
            .MaximumLength(10).WithMessage("Blood type cannot exceed 10 characters.");

        RuleFor(x => x.patientRequest.Address)
            .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");

        RuleFor(x => x.patientRequest.EmergencyContactName)
            .MaximumLength(100).WithMessage("Emergency contact name cannot exceed 100 characters.");

        RuleFor(x => x.patientRequest.EmergencyContactPhone)
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
