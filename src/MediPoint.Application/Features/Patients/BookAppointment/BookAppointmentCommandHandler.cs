using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using MediPoint.Domain.Common.Exceptions;
using MediPoint.Domain.Entities.Apointments;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Patients.BookAppointment;

public class BookAppointmentCommandHandler(IAppDbContext dbContext,ILogger<BookAppointmentCommandHandler>logger,IMailer mailer) : IRequestHandler<BookAppointmentCommand, Appointment>
{
    public async Task<Appointment> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await dbContext.Appointments.Include(a=>a.Doctor)
            .FirstOrDefaultAsync(a=>a.Id==request.Request.AppointmentId);
        if (appointment is null)
        {
            logger.LogError("No Appointment with {AppointmentId} were found", request.Request.AppointmentId);
            throw new NotFoundException("Appointment", request.Request.AppointmentId.ToString());
        }

        try
        {
            appointment.Confirm(request.Request.PatientId);
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Booking failed for appointment {AppointmentId} with Dr. {DoctorName}: {Reason}",
                request.Request.AppointmentId, appointment.Doctor.FirstName + " " + appointment.Doctor.LastName, ex.Message);
            throw new ConflictException(ex.Message);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Patient with id {PatientId} successfully booked Appointment {AppointmentId}", request.Request.PatientId, request.Request.AppointmentId);

        var patient = await dbContext.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.Request.PatientId, cancellationToken);
        if (patient is not null)
        {
            await mailer.SendEmailAsync(patient.Email, "Appointment Confirmed",
                $"Hi {patient.FirstName}, your appointment with Dr. {appointment.Doctor.FirstName} {appointment.Doctor.LastName} on {appointment.AppointmentDate:f} is confirmed.");
        }

        return appointment;
    }
}
