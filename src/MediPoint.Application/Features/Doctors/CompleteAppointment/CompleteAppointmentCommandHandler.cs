using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Doctors.AppointmentsQuery.DTOs;
using MediPoint.Domain.Common.Exceptions;
using MediPoint.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Doctors.CompleteAppointment;

public class CompleteAppointmentCommandHandler(IAppDbContext dbContext,IMailer mailer) : IRequestHandler<CompleteAppointmentCommand, AppointmentResponse>
{
    public async Task<AppointmentResponse> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await dbContext.Appointments.FirstOrDefaultAsync(a => a.Id == request.AppointmentId);

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

        var patient = await dbContext.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == appointment.PatientId, cancellationToken);
        if (patient is not null)
        {
            await mailer.SendEmailAsync(patient.Email, "Appointment Completed",
                $"Hi {patient.FirstName}, your appointment on {appointment.AppointmentDate:f} has been marked as completed.");

            await dbContext.PatientNotifications.AddAsync(new PatientNotification
            {
                PatientId = patient.Id,
                Title = "Appointment Completed",
                Message = $"Your appointment on {appointment.AppointmentDate:f} has been marked as completed.",
                Type = NotificationTypes.AppointmentCompleted,
            }, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return appointment.Adapt<AppointmentResponse>();
    }
}
