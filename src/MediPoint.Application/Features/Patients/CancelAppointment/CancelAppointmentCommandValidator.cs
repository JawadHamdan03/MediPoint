using FluentValidation;

namespace MediPoint.Application.Features.Patients.CancelAppointment;

public class CancelAppointmentCommandValidator : AbstractValidator<CancelAppointmentCommand>
{
    public CancelAppointmentCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty().WithMessage("Appointment id is required.");

        RuleFor(x => x.CancellationReason)
            .MaximumLength(500).WithMessage("Cancellation reason cannot exceed 500 characters.");
    }
}
