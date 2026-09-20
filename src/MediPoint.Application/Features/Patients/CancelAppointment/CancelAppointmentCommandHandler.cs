using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Domain.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Patients.CancelAppointment;

public class CancelAppointmentCommandHandler(IAppDbContext dbContext,IMailer mailer) : IRequestHandler<CancelAppointmentCommand, AppointmentDTO>
{
    public async Task<AppointmentDTO> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await dbContext.Appointments.Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId);

        if (appointment is null || appointment.PatientId != request.PatientId)
            throw new NotFoundException("Appointment", request.AppointmentId.ToString());

        try
        {
            appointment.Cancel(request.CancellationReason);
        }
        catch (DomainException ex)
        {
            throw new ConflictException(ex.Message);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        await mailer.SendEmailAsync(appointment.Doctor.Email, "Appointment Cancelled",
            $"Hi Dr. {appointment.Doctor.FirstName}, your appointment on {appointment.AppointmentDate:f} was cancelled by the patient.");

        return appointment.Adapt<AppointmentDTO>();
    }
}
