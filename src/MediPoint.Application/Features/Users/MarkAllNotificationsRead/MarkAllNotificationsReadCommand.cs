using MediatR;

namespace MediPoint.Application.Features.Users.MarkAllNotificationsRead;

public record MarkAllNotificationsReadCommand(Guid UserId, string Role) : IRequest;
