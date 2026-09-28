using FluentValidation;

namespace MediPoint.Application.Features.Doctors.GenerateAppointmentSlots;

public class GenerateAppointmentSlotsCommandValidator : AbstractValidator<GenerateAppointmentSlotsCommand>
{
    public GenerateAppointmentSlotsCommandValidator()
    {
        RuleFor(x => x.Request.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .Must(NotBeInPast).WithMessage("Start date cannot be in the past.");

        RuleFor(x => x.Request.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThanOrEqualTo(x => x.Request.StartDate).WithMessage("End date must be on or after the start date.");

        RuleFor(x => x)
            .Must(x => x.Request.EndDate.DayNumber - x.Request.StartDate.DayNumber <= 90)
            .WithMessage("Date range cannot exceed 90 days.");

        RuleFor(x => x.Request.DaysOfWeek)
            .NotEmpty().WithMessage("At least one day of week is required.");

        RuleFor(x => x.Request.StartTime)
            .LessThan(x => x.Request.EndTime).WithMessage("Start time must be before end time.");

        RuleFor(x => x.Request.SlotDurationMinutes)
            .GreaterThan(0).WithMessage("Slot duration must be greater than zero.")
            .LessThanOrEqualTo(480).WithMessage("Slot duration cannot exceed 8 hours.");
    }

    private static bool NotBeInPast(DateOnly date) => date >= DateOnly.FromDateTime(DateTime.UtcNow);
}
