using FluentValidation;

namespace MediPoint.Application.Features.Admins.AdminAddsDoctor;

public class AddDoctorCommandValidator : AbstractValidator<AddDoctorCommand>
{
    public AddDoctorCommandValidator()
    {
        RuleFor(x => x.doctorRequest.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.doctorRequest.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.doctorRequest.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email address cannot exceed 100 characters.");

        RuleFor(x => x.doctorRequest.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[\^$*.[\]{}()?""!@#%&/\\,><':;|_~`]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.doctorRequest.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number must be a valid international format (E.164).");

        RuleFor(x => x.doctorRequest.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .Must(BeAtLeast21YearsOld).WithMessage("Doctor must be at least 21 years old.")
            .Must(BeValidAge).WithMessage("Date of birth is not valid.");

        RuleFor(x => x.doctorRequest.Gender)
            .IsInEnum().WithMessage("A valid gender selection is required.");

        RuleFor(x => x.doctorRequest.Specialty)
            .NotEmpty().WithMessage("Medical specialty is required.")
            .MaximumLength(100).WithMessage("Specialty name cannot exceed 100 characters.");

        RuleFor(x => x.doctorRequest.LicenseNumber)
            .NotEmpty().WithMessage("Medical license number is required.")
            .MaximumLength(50).WithMessage("License number cannot exceed 50 characters.");

        RuleFor(x => x.doctorRequest.YearsOfExperience)
            .GreaterThanOrEqualTo(0).WithMessage("Years of experience cannot be negative.")
            .LessThanOrEqualTo(70).WithMessage("Please enter a realistic number of years of experience.");

        RuleFor(x => x.doctorRequest.ConsultationFee)
            .GreaterThanOrEqualTo(0).WithMessage("Consultation fee cannot be negative.");

        RuleFor(x => x.doctorRequest.Biography)
            .MaximumLength(1000).WithMessage("Biography cannot exceed 1000 characters.");
    }
        
    private bool BeAtLeast21YearsOld(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return dateOfBirth <= today.AddYears(-21);
    }

    private bool BeValidAge(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return dateOfBirth >= today.AddYears(-100);
    }
}