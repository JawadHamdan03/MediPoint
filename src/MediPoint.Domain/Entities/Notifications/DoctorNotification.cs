using MediPoint.Domain.Entities.User;

namespace MediPoint.Domain.Entities.Notifications;

public class DoctorNotification : Notification
{
    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;
}
