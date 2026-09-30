using MediPoint.Domain.Entities.User;

namespace MediPoint.Domain.Entities.Notifications;

public class AdminNotification : Notification
{
    public Guid AdminId { get; set; }
    public Admin Admin { get; set; } = null!;
}
