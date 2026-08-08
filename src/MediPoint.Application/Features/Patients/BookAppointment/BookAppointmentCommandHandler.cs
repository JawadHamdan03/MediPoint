using DnsClient.Internal;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.Appointments.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Patients.BookAppointment;

public class BookAppointmentCommandHandler(IAppDbContext dbContext,ILogger<BookAppointmentCommandHandler>logger) : IRequestHandler<BookAppointmentCommand, Appointment>
{
    public async Task<Appointment> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await dbContext.Appointments.FindAsync(request.Request.AppointmentId);
        if (appointment is null)
        {
            logger.LogError("No Appointment with {AppointmentId} were found", request.Request.AppointmentId);
            throw new Exception("this appointment was not found");
        }
        if (appointment.Status == AppointmentStatus.Confirmed || appointment.Status == AppointmentStatus.Completed || appointment.Status == AppointmentStatus.Cancelled)
            throw new Exception("Can't book this Appointment choose another one");    
        var patient = request.Request.PatientId;
        appointment.PatientId = patient;
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Patient with id {PatientId} successfully booked Appointment {request.Request.AppointmentId}", request.Request.PatientId, request.Request.AppointmentId);
        return appointment;
    }
}
