using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Patients.Login;

public class PatientLoginCommandValidator : AbstractValidator<PatientLoginCommand>
{
    public PatientLoginCommandValidator()
    {
        RuleFor(x => x.LoginRequest.Email).NotEmpty().WithMessage("Email Can't be Empty");
        RuleFor(x => x.LoginRequest.Password).NotEmpty().WithMessage("Password Can't be Empty");
    }
}
