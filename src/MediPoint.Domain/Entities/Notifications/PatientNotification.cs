using MediPoint.Domain.Entities.User;

namespace MediPoint.Domain.Entities.Notifications;

public class PatientNotification : Notification
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
}
