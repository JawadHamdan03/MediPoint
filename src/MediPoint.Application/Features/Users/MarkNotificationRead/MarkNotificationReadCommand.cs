using MediatR;

namespace MediPoint.Application.Features.Users.MarkNotificationRead;

public record MarkNotificationReadCommand(Guid UserId, string Role, Guid NotificationId) : IRequest;
