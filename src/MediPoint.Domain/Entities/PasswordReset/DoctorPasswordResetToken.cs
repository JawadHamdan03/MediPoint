using MediPoint.Domain.Entities.User;

namespace MediPoint.Domain.Entities.PasswordReset;

public class DoctorPasswordResetToken : PasswordResetToken
{
    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;
}
