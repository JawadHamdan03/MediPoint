using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Doctors.AddAppointment;

public class AddAppointmentCommandvalidator : AbstractValidator<AddAppointmentCommand>
{
    public AddAppointmentCommandvalidator()
    {
        RuleFor(x=>x.appointment.Duration).NotEmpty();
        RuleFor(x=>x.appointment.AppointmentDate).NotEmpty();
        RuleFor(x=>x.appointment.DoctorId).NotEmpty();
    }
}
