using FluentValidation;

namespace MediPoint.Application.Features.Patients.Chat;

public class ChatCommandValidator : AbstractValidator<ChatCommand>
{
    public ChatCommandValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(2000).WithMessage("Message must be 2000 characters or fewer.");

        RuleFor(x => x.PatientId).NotEmpty();
    }
}
