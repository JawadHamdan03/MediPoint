using MediPoint.Domain.Common;

namespace MediPoint.Domain.Entities.PasswordReset;

public class PasswordResetToken : BaseEntity
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
