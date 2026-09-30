using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Domain.Entities.PasswordReset;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace MediPoint.Application.Features.Admins.ForgotPassword;

public class AdminForgotPasswordCommandHandler(IAppDbContext dbContext, IMailer mailer, IAppUrlProvider urlProvider)
    : IRequestHandler<AdminForgotPasswordCommand, MessageResponse>
{
    private const string GenericMessage = "If an account with that email exists, a password reset link has been sent.";

    public async Task<MessageResponse> Handle(AdminForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Email.Equals(request.Request.Email), cancellationToken);

        if (admin is not null)
        {
            var existingTokens = await dbContext.AdminPasswordResetTokens.Where(t => t.AdminId == admin.Id).ToListAsync(cancellationToken);
            dbContext.AdminPasswordResetTokens.RemoveRange(existingTokens);

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            await dbContext.AdminPasswordResetTokens.AddAsync(new AdminPasswordResetToken
            {
                AdminId = admin.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            }, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var resetUrl = urlProvider.BuildPasswordResetUrl("Admin", token);
            await mailer.SendEmailAsync(admin.Email, "Reset your MediPoint password",
                $"Hi {admin.FirstName}, click the link below to reset your password. This link expires in 30 minutes.<br/><a href=\"{resetUrl}\">{resetUrl}</a><br/>If you didn't request this, you can safely ignore this email.");
        }

        return new MessageResponse { Message = GenericMessage };
    }
}
