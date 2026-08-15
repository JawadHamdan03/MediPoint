using System.Data;
using FluentValidation;

namespace MediPoint.Application.Features.Admins.Login;

public class AdminLoginCommandValidator :AbstractValidator<AdminLoginCommand>
{
    public AdminLoginCommandValidator()
    {
        RuleFor(x => x.loginRequest.Email).NotEmpty();
        RuleFor(x => x.loginRequest.Password).NotEmpty();
    }
}