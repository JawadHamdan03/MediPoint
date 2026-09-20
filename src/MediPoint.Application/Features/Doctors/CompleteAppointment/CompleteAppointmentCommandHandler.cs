using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Doctors.AppointmentsQuery.DTOs;
using MediPoint.Domain.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Doctors.CompleteAppointment;

public class CompleteAppointmentCommandHandler(IAppDbContext dbContext,IMailer mailer) : IRequestHandler<CompleteAppointmentCommand, AppointmentResponse>
{
    public async Task<AppointmentResponse> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await dbContext.Appointments.Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId);

        if (appointment is null || appointment.DoctorId != request.DoctorId)
            throw new NotFoundException("Appointment", request.AppointmentId.ToString());

        try
        {
            appointment.Complete(request.Notes);
        }
        catch (DomainException ex)
        {
            throw new ConflictException(ex.Message);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        if (appointment.Patient is not null)
        {
            await mailer.SendEmailAsync(appointment.Patient.Email, "Appointment Completed",
                $"Hi {appointment.Patient.FirstName}, your appointment on {appointment.AppointmentDate:f} has been marked as completed.");
        }

        return appointment.Adapt<AppointmentResponse>();
    }
}
