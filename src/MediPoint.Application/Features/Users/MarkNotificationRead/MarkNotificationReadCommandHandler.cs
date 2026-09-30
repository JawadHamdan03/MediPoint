using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Users.MarkNotificationRead;

public class MarkNotificationReadCommandHandler(IAppDbContext dbContext) : IRequestHandler<MarkNotificationReadCommand>
{
    public async Task Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        switch (request.Role)
        {
            case "Admin":
                var admin = await dbContext.AdminNotifications.FirstOrDefaultAsync(n => n.Id == request.NotificationId && n.AdminId == request.UserId, cancellationToken);
                if (admin is null) throw new NotFoundException("Notification", request.NotificationId.ToString());
                admin.IsRead = true;
                break;
            case "Doctor":
                var doctor = await dbContext.DoctorNotifications.FirstOrDefaultAsync(n => n.Id == request.NotificationId && n.DoctorId == request.UserId, cancellationToken);
                if (doctor is null) throw new NotFoundException("Notification", request.NotificationId.ToString());
                doctor.IsRead = true;
                break;
            case "Patient":
                var patient = await dbContext.PatientNotifications.FirstOrDefaultAsync(n => n.Id == request.NotificationId && n.PatientId == request.UserId, cancellationToken);
                if (patient is null) throw new NotFoundException("Notification", request.NotificationId.ToString());
                patient.IsRead = true;
                break;
            default:
                throw new NotFoundException("Notification", request.NotificationId.ToString());
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
