using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Doctors.AppointmentsQuery;

public class GetTodaysAppointmentsQueryValidator : AbstractValidator<GetTodaysAppointmentsQuery>
{
    public GetTodaysAppointmentsQueryValidator()
    {
        RuleFor(x => x.DoctorId).NotEmpty();
    }
}
