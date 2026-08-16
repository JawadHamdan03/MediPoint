using MediatR;
using MediPoint.Application.Features.Doctors.AddAppointment.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using Mapster;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.Appointments.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Doctors.AddAppointment;

public class AddAppointmentCommandHandler(IAppDbContext dbContext) : IRequestHandler<AddAppointmentCommand, ApponitmentDTO>
{
    public async Task<ApponitmentDTO> Handle(AddAppointmentCommand request, CancellationToken cancellationToken)
    {
        var req = request.appointment;
        var newStart = req.AppointmentDate;
        var newEnd = newStart.AddMinutes(req.Duration);

        
        var hasConflict = await dbContext.Appointments
            .Where(a => a.DoctorId == req.DoctorId && a.Status != AppointmentStatus.Cancelled)
            .AnyAsync(a =>
                newStart < a.AppointmentDate.AddMinutes(a.Duration) &&
                a.AppointmentDate < newEnd,
                cancellationToken);

        if (hasConflict)
            throw new ConflictException("Conflict in Appointment Dates");

        var app = req.Adapt<Appointment>();
        await dbContext.Appointments.AddAsync(app, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return app.Adapt<ApponitmentDTO>();
    }
}
