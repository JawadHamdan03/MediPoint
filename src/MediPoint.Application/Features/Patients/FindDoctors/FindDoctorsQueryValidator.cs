using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Patients.FindDoctors;

public class FindDoctorsQueryValidator : AbstractValidator<FindDoctorsQuery>
{
    public FindDoctorsQueryValidator()
    {
        RuleFor(x => x.speciality).NotEmpty().WithMessage("Speciality can't be empty");
    }
}
