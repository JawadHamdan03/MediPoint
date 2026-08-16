using FluentValidation;

namespace MediPoint.Application.Features.Patients.UpdateDetails;

public class UpdatePatientDetailsCommandValidator : AbstractValidator<UpdatePatientDetailsCommand>
{
    public UpdatePatientDetailsCommandValidator()
    {
        RuleFor(x => x.Details.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.Details.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.Details.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number must be a valid international format (E.164).");

        RuleFor(x => x.Details.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .Must(NotBeInFuture).WithMessage("Date of birth cannot be in the future.")
            .Must(BeValidAge).WithMessage("Date of birth is not valid.");

        RuleFor(x => x.Details.Gender)
            .IsInEnum().WithMessage("A valid gender selection is required.");

        RuleFor(x => x.Details.BloodType)
            .MaximumLength(10).WithMessage("Blood type cannot exceed 10 characters.");

        RuleFor(x => x.Details.Address)
            .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");

        RuleFor(x => x.Details.EmergencyContactName)
            .MaximumLength(100).WithMessage("Emergency contact name cannot exceed 100 characters.");

        RuleFor(x => x.Details.EmergencyContactPhone)
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
