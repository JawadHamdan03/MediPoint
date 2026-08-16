using DnsClient.Internal;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.Appointments.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Patients.BookAppointment;

public class BookAppointmentCommandHandler(IAppDbContext dbContext,ILogger<BookAppointmentCommandHandler>logger) : IRequestHandler<BookAppointmentCommand, Appointment>
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

        if (appointment.Status == AppointmentStatus.Confirmed)
        {
            logger.LogWarning("Double booking attempt prevented for Dr. {DoctorName} at 10:00 AM",appointment.Doctor.FirstName+" "+appointment.Doctor.LastName);
            throw new ConflictException("This appointment slot is already booked. Please choose another one.");
        }

        if ( appointment.Status == AppointmentStatus.Completed || appointment.Status == AppointmentStatus.Cancelled)
        {
            throw new ConflictException("This appointment is no longer available for booking. Please choose another one.");
        }
            
        var patient = request.Request.PatientId;
        appointment.PatientId = patient;
        appointment.Status = AppointmentStatus.Confirmed;
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Patient with id {PatientId} successfully booked Appointment {request.Request.AppointmentId}", request.Request.PatientId, request.Request.AppointmentId);
        return appointment;
    }
}
