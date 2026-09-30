using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Features.Users.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Users.GetNotifications;

public class GetNotificationsQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetNotificationsQuery, List<NotificationResponse>>
{
    private const int MaxResults = 50;

    public async Task<List<NotificationResponse>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        return request.Role switch
        {
            "Admin" => (await dbContext.AdminNotifications.AsNoTracking()
                .Where(n => n.AdminId == request.UserId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(MaxResults)
                .ToListAsync(cancellationToken)).Adapt<List<NotificationResponse>>(),
            "Doctor" => (await dbContext.DoctorNotifications.AsNoTracking()
                .Where(n => n.DoctorId == request.UserId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(MaxResults)
                .ToListAsync(cancellationToken)).Adapt<List<NotificationResponse>>(),
            "Patient" => (await dbContext.PatientNotifications.AsNoTracking()
                .Where(n => n.PatientId == request.UserId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(MaxResults)
                .ToListAsync(cancellationToken)).Adapt<List<NotificationResponse>>(),
            _ => new List<NotificationResponse>(),
        };
    }
}
