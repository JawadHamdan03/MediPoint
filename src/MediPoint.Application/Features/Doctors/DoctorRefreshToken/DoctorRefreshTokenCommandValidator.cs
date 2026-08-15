using FluentValidation;

namespace MediPoint.Application.Features.Doctors.DoctorRefreshToken;

public class DoctorRefreshTokenCommandValidator:AbstractValidator<DoctorRefreshTokenCommand>
{
    public DoctorRefreshTokenCommandValidator()
    {
        RuleFor(x => x.refreshToken).NotEmpty();
    }
}