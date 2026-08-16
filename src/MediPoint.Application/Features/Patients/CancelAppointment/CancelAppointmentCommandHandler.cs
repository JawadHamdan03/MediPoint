using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Domain.Entities.Appointments.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Patients.CancelAppointment;

public class CancelAppointmentCommandHandler(IAppDbContext dbContext) : IRequestHandler<CancelAppointmentCommand, AppointmentDTO>
{
    public async Task<AppointmentDTO> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await dbContext.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId);

        if (appointment is null || appointment.PatientId != request.PatientId)
            throw new NotFoundException("Appointment", request.AppointmentId.ToString());

        if (appointment.Status == AppointmentStatus.Cancelled)
            throw new ConflictException("Appointment is already cancelled");

        if (appointment.Status == AppointmentStatus.Completed)
            throw new ConflictException("A completed appointment cannot be cancelled");

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.CancellationReason = request.CancellationReason;
        await dbContext.SaveChangesAsync(cancellationToken);

        return appointment.Adapt<AppointmentDTO>();
    }
}
