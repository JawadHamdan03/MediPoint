using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Patients.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Admins.ResetPassword;

public class AdminResetPasswordCommandHandler(IAppDbContext dbContext, IMailer mailer)
    : IRequestHandler<AdminResetPasswordCommand, MessageResponse>
{
    public async Task<MessageResponse> Handle(AdminResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var resetToken = await dbContext.AdminPasswordResetTokens
            .FirstOrDefaultAsync(t => t.Token == request.Request.Token, cancellationToken);

        if (resetToken is null || resetToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid or expired reset token");
        }

        var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == resetToken.AdminId, cancellationToken);
        if (admin is null)
        {
            throw new UnauthorizedException("Invalid or expired reset token");
        }

        admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.NewPassword);

        var resetTokens = await dbContext.AdminPasswordResetTokens.Where(t => t.AdminId == admin.Id).ToListAsync(cancellationToken);
        dbContext.AdminPasswordResetTokens.RemoveRange(resetTokens);
        var refreshTokens = await dbContext.AdminRefreshTokens.Where(t => t.AdminId == admin.Id).ToListAsync(cancellationToken);
        dbContext.AdminRefreshTokens.RemoveRange(refreshTokens);
        await dbContext.SaveChangesAsync(cancellationToken);

        await mailer.SendEmailAsync(admin.Email, "Your MediPoint password was changed",
            $"Hi {admin.FirstName}, your password was just changed. If you didn't do this, please contact support immediately.");

        return new MessageResponse { Message = "Your password has been reset. You can now log in with your new password." };
    }
}
