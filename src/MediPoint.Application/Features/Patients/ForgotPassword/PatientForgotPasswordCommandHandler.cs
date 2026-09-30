using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Domain.Entities.PasswordReset;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace MediPoint.Application.Features.Patients.ForgotPassword;

public class PatientForgotPasswordCommandHandler(IAppDbContext dbContext, IMailer mailer, IAppUrlProvider urlProvider)
    : IRequestHandler<PatientForgotPasswordCommand, MessageResponse>
{
    private const string GenericMessage = "If an account with that email exists, a password reset link has been sent.";

    public async Task<MessageResponse> Handle(PatientForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var patient = await dbContext.Patients.FirstOrDefaultAsync(p => p.Email.Equals(request.Request.Email), cancellationToken);

        if (patient is not null)
        {
            var existingTokens = await dbContext.PatientPasswordResetTokens.Where(t => t.PatientId == patient.Id).ToListAsync(cancellationToken);
            dbContext.PatientPasswordResetTokens.RemoveRange(existingTokens);

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            await dbContext.PatientPasswordResetTokens.AddAsync(new PatientPasswordResetToken
            {
                PatientId = patient.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            }, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var resetUrl = urlProvider.BuildPasswordResetUrl("Patient", token);
            await mailer.SendEmailAsync(patient.Email, "Reset your MediPoint password",
                $"Hi {patient.FirstName}, click the link below to reset your password. This link expires in 30 minutes.<br/><a href=\"{resetUrl}\">{resetUrl}</a><br/>If you didn't request this, you can safely ignore this email.");
        }

        return new MessageResponse { Message = GenericMessage };
    }
}
