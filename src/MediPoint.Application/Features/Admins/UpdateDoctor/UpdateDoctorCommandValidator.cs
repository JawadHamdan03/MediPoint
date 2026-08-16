using FluentValidation;

namespace MediPoint.Application.Features.Admins.UpdateDoctor;

public class UpdateDoctorCommandValidator : AbstractValidator<UpdateDoctorCommand>
{
    public UpdateDoctorCommandValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("Doctor id is required.");

        RuleFor(x => x.Doctor.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.Doctor.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.Doctor.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number must be a valid international format (E.164).");

        RuleFor(x => x.Doctor.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .Must(BeAtLeast21YearsOld).WithMessage("Doctor must be at least 21 years old.")
            .Must(BeValidAge).WithMessage("Date of birth is not valid.");

        RuleFor(x => x.Doctor.Gender)
            .IsInEnum().WithMessage("A valid gender selection is required.");

        RuleFor(x => x.Doctor.Specialty)
            .NotEmpty().WithMessage("Medical specialty is required.")
            .MaximumLength(100).WithMessage("Specialty name cannot exceed 100 characters.");

        RuleFor(x => x.Doctor.LicenseNumber)
            .NotEmpty().WithMessage("Medical license number is required.")
            .MaximumLength(50).WithMessage("License number cannot exceed 50 characters.");

        RuleFor(x => x.Doctor.YearsOfExperience)
            .GreaterThanOrEqualTo(0).WithMessage("Years of experience cannot be negative.")
            .LessThanOrEqualTo(70).WithMessage("Please enter a realistic number of years of experience.");

        RuleFor(x => x.Doctor.ConsultationFee)
            .GreaterThanOrEqualTo(0).WithMessage("Consultation fee cannot be negative.");

        RuleFor(x => x.Doctor.Biography)
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
