using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Patients.RefreshPatientToken;

public class RefreshPatienttokenRequestValidator : AbstractValidator<RefreshPatienttokenRequest>
{
    public RefreshPatienttokenRequestValidator()
    {
        RuleFor(x=>x.refreshToken).NotEmpty();
    }
}
