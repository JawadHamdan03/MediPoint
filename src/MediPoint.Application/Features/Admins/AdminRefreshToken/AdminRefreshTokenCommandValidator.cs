using FluentValidation;

namespace MediPoint.Application.Features.Admins.AdminRefreshToken;

public class AdminRefreshTokenCommandValidator:AbstractValidator<AdminRefreshTokenCommand>
{
    public AdminRefreshTokenCommandValidator()
    {
        RuleFor(x => x.refreshToken).NotEmpty();
    }
}