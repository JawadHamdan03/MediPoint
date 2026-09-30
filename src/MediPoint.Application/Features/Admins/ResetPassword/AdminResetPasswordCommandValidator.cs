using FluentValidation;

namespace MediPoint.Application.Features.Admins.ResetPassword;

public class AdminResetPasswordCommandValidator : AbstractValidator<AdminResetPasswordCommand>
{
    public AdminResetPasswordCommandValidator()
    {
        RuleFor(x => x.Request.Token)
            .NotEmpty().WithMessage("Reset token is required.");

        RuleFor(x => x.Request.NewPassword)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[\^$*.[\]{}()?""!@#%&/\\,><':;|_~`]").WithMessage("Password must contain at least one special character.");
    }
}
