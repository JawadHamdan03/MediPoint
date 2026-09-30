using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Patients.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Doctors.ResetPassword;

public class DoctorResetPasswordCommandHandler(IAppDbContext dbContext, IMailer mailer)
    : IRequestHandler<DoctorResetPasswordCommand, MessageResponse>
{
    public async Task<MessageResponse> Handle(DoctorResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var resetToken = await dbContext.DoctorPasswordResetTokens
            .FirstOrDefaultAsync(t => t.Token == request.Request.Token, cancellationToken);

        if (resetToken is null || resetToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid or expired reset token");
        }

        var doctor = await dbContext.Doctors.FirstOrDefaultAsync(d => d.Id == resetToken.DoctorId, cancellationToken);
        if (doctor is null)
        {
            throw new UnauthorizedException("Invalid or expired reset token");
        }

        doctor.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.NewPassword);

        var resetTokens = await dbContext.DoctorPasswordResetTokens.Where(t => t.DoctorId == doctor.Id).ToListAsync(cancellationToken);
        dbContext.DoctorPasswordResetTokens.RemoveRange(resetTokens);
        var refreshTokens = await dbContext.DoctorRefreshTokens.Where(t => t.DoctorId == doctor.Id).ToListAsync(cancellationToken);
        dbContext.DoctorRefreshTokens.RemoveRange(refreshTokens);
        await dbContext.SaveChangesAsync(cancellationToken);

        await mailer.SendEmailAsync(doctor.Email, "Your MediPoint password was changed",
            $"Hi Dr. {doctor.LastName}, your password was just changed. If you didn't do this, please contact support immediately.");

        return new MessageResponse { Message = "Your password has been reset. You can now log in with your new password." };
    }
}
