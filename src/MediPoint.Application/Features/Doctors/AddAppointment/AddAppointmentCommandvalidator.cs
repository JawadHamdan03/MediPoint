using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Doctors.AddAppointment;

public class AddAppointmentCommandvalidator : AbstractValidator<AddAppointmentCommandHandler>
{
    public AddAppointmentCommandvalidator()
    {
        
    }
}
