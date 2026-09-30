using MediPoint.Domain.Entities.User;

namespace MediPoint.Domain.Entities.PasswordReset;

public class AdminPasswordResetToken : PasswordResetToken
{
    public Guid AdminId { get; set; }
    public Admin Admin { get; set; } = null!;
}
