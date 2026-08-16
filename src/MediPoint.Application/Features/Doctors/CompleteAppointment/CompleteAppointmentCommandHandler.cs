using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Doctors.AppointmentsQuery.DTOs;
using MediPoint.Domain.Entities.Appointments.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Doctors.CompleteAppointment;

public class CompleteAppointmentCommandHandler(IAppDbContext dbContext) : IRequestHandler<CompleteAppointmentCommand, AppointmentResponse>
{
    public async Task<AppointmentResponse> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await dbContext.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId);

        if (appointment is null || appointment.DoctorId != request.DoctorId)
            throw new NotFoundException("Appointment", request.AppointmentId.ToString());

        if (appointment.Status != AppointmentStatus.Confirmed)
            throw new ConflictException("Only a confirmed appointment can be marked completed");

        appointment.Status = AppointmentStatus.Completed;
        if (request.Notes is not null)
            appointment.Notes = request.Notes;
        await dbContext.SaveChangesAsync(cancellationToken);

        return appointment.Adapt<AppointmentResponse>();
    }
}
