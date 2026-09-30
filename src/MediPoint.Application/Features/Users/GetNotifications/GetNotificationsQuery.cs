using MediatR;
using MediPoint.Application.Features.Users.DTOs;

namespace MediPoint.Application.Features.Users.GetNotifications;

public record GetNotificationsQuery(Guid UserId, string Role) : IRequest<List<NotificationResponse>>;
