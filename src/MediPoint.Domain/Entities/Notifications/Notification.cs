using MediPoint.Domain.Common;

namespace MediPoint.Domain.Entities.Notifications;

public class Notification : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string Type { get; set; } = null!;
    public bool IsRead { get; set; }
}
