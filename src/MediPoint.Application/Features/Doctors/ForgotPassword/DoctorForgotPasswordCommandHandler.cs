using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Domain.Entities.PasswordReset;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace MediPoint.Application.Features.Doctors.ForgotPassword;

public class DoctorForgotPasswordCommandHandler(IAppDbContext dbContext, IMailer mailer, IAppUrlProvider urlProvider)
    : IRequestHandler<DoctorForgotPasswordCommand, MessageResponse>
{
    private const string GenericMessage = "If an account with that email exists, a password reset link has been sent.";

    public async Task<MessageResponse> Handle(DoctorForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var doctor = await dbContext.Doctors.FirstOrDefaultAsync(d => d.Email.Equals(request.Request.Email), cancellationToken);

        if (doctor is not null)
        {
            var existingTokens = await dbContext.DoctorPasswordResetTokens.Where(t => t.DoctorId == doctor.Id).ToListAsync(cancellationToken);
            dbContext.DoctorPasswordResetTokens.RemoveRange(existingTokens);

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            await dbContext.DoctorPasswordResetTokens.AddAsync(new DoctorPasswordResetToken
            {
                DoctorId = doctor.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            }, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var resetUrl = urlProvider.BuildPasswordResetUrl("Doctor", token);
            await mailer.SendEmailAsync(doctor.Email, "Reset your MediPoint password",
                $"Hi Dr. {doctor.LastName}, click the link below to reset your password. This link expires in 30 minutes.<br/><a href=\"{resetUrl}\">{resetUrl}</a><br/>If you didn't request this, you can safely ignore this email.");
        }

        return new MessageResponse { Message = GenericMessage };
    }
}
