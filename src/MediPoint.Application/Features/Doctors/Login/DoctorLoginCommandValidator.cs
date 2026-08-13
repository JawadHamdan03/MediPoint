using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Doctors.Login;

public class DoctorLoginCommandValidator : AbstractValidator<DoctorLoginCommand>
{
    public DoctorLoginCommandValidator()
    {
        RuleFor(x=>x.Request.Email).NotEmpty().WithMessage("Email can not be empty");
        RuleFor(x=>x.Request.Password).NotEmpty().WithMessage("Password can not be empty");
    }
}
