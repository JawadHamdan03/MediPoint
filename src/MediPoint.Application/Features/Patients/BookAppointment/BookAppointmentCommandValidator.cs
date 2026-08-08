using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Patients.BookAppointment;

public class BookAppointmentCommandValidator : AbstractValidator<BookAppointmentCommand>
{
    public BookAppointmentCommandValidator()
    {
        RuleFor(x=>x.Request.AppointmentId).NotEmpty();
        RuleFor(x=>x.Request.PatientId).NotEmpty();

    }
}
