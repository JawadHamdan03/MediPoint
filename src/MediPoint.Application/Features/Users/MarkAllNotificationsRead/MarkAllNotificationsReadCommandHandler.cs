using MediatR;
using MediPoint.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Users.MarkAllNotificationsRead;

public class MarkAllNotificationsReadCommandHandler(IAppDbContext dbContext) : IRequestHandler<MarkAllNotificationsReadCommand>
{
    public async Task Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        switch (request.Role)
        {
            case "Admin":
                var adminNotifications = await dbContext.AdminNotifications.Where(n => n.AdminId == request.UserId && !n.IsRead).ToListAsync(cancellationToken);
                foreach (var n in adminNotifications) n.IsRead = true;
                break;
            case "Doctor":
                var doctorNotifications = await dbContext.DoctorNotifications.Where(n => n.DoctorId == request.UserId && !n.IsRead).ToListAsync(cancellationToken);
                foreach (var n in doctorNotifications) n.IsRead = true;
                break;
            case "Patient":
                var patientNotifications = await dbContext.PatientNotifications.Where(n => n.PatientId == request.UserId && !n.IsRead).ToListAsync(cancellationToken);
                foreach (var n in patientNotifications) n.IsRead = true;
                break;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
