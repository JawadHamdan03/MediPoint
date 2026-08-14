using MediatR;
using MediPoint.Application.Features.Doctors.AddAppointment.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using Mapster;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Domain.Entities.Apointments;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Doctors.AddAppointment;

public class AddAppointmentCommandHandler(IAppDbContext dbContext) : IRequestHandler<AddAppointmentCommand, ApponitmentDTO>
{
    public async Task<ApponitmentDTO> Handle(AddAppointmentCommand request, CancellationToken cancellationToken)
    {
        var app = request.appointment.Adapt<Appointment>();

        var doctorApp = await dbContext.Appointments.Include(a => a.Doctor)
            .Where(a => a.DoctorId.Equals(request.appointment.DoctorId)).ToListAsync();

        var lim = request.appointment.AppointmentDate.AddMinutes(request.appointment.Duration);
        foreach (var a in doctorApp)
        {
            if (a.AppointmentDate < lim)
                throw new ConflictException("Conflict in Appointment Dates");
        }

        await dbContext.Appointments.AddAsync(app);
        await dbContext.SaveChangesAsync(cancellationToken);
        return app.Adapt<ApponitmentDTO>();
    }
}
