using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Patients.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Patients.ResetPassword;

public class PatientResetPasswordCommandHandler(IAppDbContext dbContext, IMailer mailer)
    : IRequestHandler<PatientResetPasswordCommand, MessageResponse>
{
    public async Task<MessageResponse> Handle(PatientResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var resetToken = await dbContext.PatientPasswordResetTokens
            .FirstOrDefaultAsync(t => t.Token == request.Request.Token, cancellationToken);

        if (resetToken is null || resetToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid or expired reset token");
        }

        var patient = await dbContext.Patients.FirstOrDefaultAsync(p => p.Id == resetToken.PatientId, cancellationToken);
        if (patient is null)
        {
            throw new UnauthorizedException("Invalid or expired reset token");
        }

        patient.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.NewPassword);

        var resetTokens = await dbContext.PatientPasswordResetTokens.Where(t => t.PatientId == patient.Id).ToListAsync(cancellationToken);
        dbContext.PatientPasswordResetTokens.RemoveRange(resetTokens);
        var refreshTokens = await dbContext.PatientRefreshTokens.Where(t => t.PatientId == patient.Id).ToListAsync(cancellationToken);
        dbContext.PatientRefreshTokens.RemoveRange(refreshTokens);
        await dbContext.SaveChangesAsync(cancellationToken);

        await mailer.SendEmailAsync(patient.Email, "Your MediPoint password was changed",
            $"Hi {patient.FirstName}, your password was just changed. If you didn't do this, please contact support immediately.");

        return new MessageResponse { Message = "Your password has been reset. You can now log in with your new password." };
    }
}
