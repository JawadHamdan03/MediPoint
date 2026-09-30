using MediPoint.Domain.Entities.User;

namespace MediPoint.Domain.Entities.PasswordReset;

public class PatientPasswordResetToken : PasswordResetToken
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
}
