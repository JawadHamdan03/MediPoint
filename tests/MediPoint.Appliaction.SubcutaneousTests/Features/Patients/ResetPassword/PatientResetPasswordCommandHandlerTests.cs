using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Application.Features.Patients.ResetPassword;
using MediPoint.Common;
using MediPoint.Domain.Entities.PasswordReset;
using MediPoint.Domain.Entities.RefreshToken;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Patients.ResetPassword;

public class PatientResetPasswordCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidToken_UpdatesPasswordAndDeletesToken()
    {
        using var dbContext = TestDbContextFactory.Create();
        var patient = TestEntities.NewPatient();
        await dbContext.Patients.AddAsync(patient);
        await dbContext.PatientPasswordResetTokens.AddAsync(new PatientPasswordResetToken
        {
            PatientId = patient.Id,
            Token = "valid-token",
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
        });
        await dbContext.PatientRefreshTokens.AddAsync(new PatientRefreshToken
        {
            PatientId = patient.Id,
            Token = "some-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
        });
        await dbContext.SaveChangesAsync();
        var mailer = Substitute.For<IMailer>();
        var handler = new PatientResetPasswordCommandHandler(dbContext, mailer);

        await handler.Handle(new PatientResetPasswordCommand(new ResetPasswordRequest { Token = "valid-token", NewPassword = "NewPassword1!" }), CancellationToken.None);

        var saved = await dbContext.Patients.SingleAsync();
        Assert.True(BCrypt.Net.BCrypt.Verify("NewPassword1!", saved.PasswordHash));
        Assert.Equal(0, await dbContext.PatientPasswordResetTokens.CountAsync());
        Assert.Equal(0, await dbContext.PatientRefreshTokens.CountAsync());
        await mailer.Received(1).SendEmailAsync(patient.Email, Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_UnknownToken_ThrowsUnauthorizedException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var handler = new PatientResetPasswordCommandHandler(dbContext, Substitute.For<IMailer>());

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new PatientResetPasswordCommand(new ResetPasswordRequest { Token = "does-not-exist", NewPassword = "NewPassword1!" }), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ExpiredToken_ThrowsUnauthorizedException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var patient = TestEntities.NewPatient();
        await dbContext.Patients.AddAsync(patient);
        await dbContext.PatientPasswordResetTokens.AddAsync(new PatientPasswordResetToken
        {
            PatientId = patient.Id,
            Token = "expired-token",
            ExpiresAt = DateTime.UtcNow.AddMinutes(-1),
        });
        await dbContext.SaveChangesAsync();
        var handler = new PatientResetPasswordCommandHandler(dbContext, Substitute.For<IMailer>());

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new PatientResetPasswordCommand(new ResetPasswordRequest { Token = "expired-token", NewPassword = "NewPassword1!" }), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidToken_CannotBeReusedAfterwards()
    {
        using var dbContext = TestDbContextFactory.Create();
        var patient = TestEntities.NewPatient();
        await dbContext.Patients.AddAsync(patient);
        await dbContext.PatientPasswordResetTokens.AddAsync(new PatientPasswordResetToken
        {
            PatientId = patient.Id,
            Token = "one-time-token",
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
        });
        await dbContext.SaveChangesAsync();
        var handler = new PatientResetPasswordCommandHandler(dbContext, Substitute.For<IMailer>());
        await handler.Handle(new PatientResetPasswordCommand(new ResetPasswordRequest { Token = "one-time-token", NewPassword = "NewPassword1!" }), CancellationToken.None);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new PatientResetPasswordCommand(new ResetPasswordRequest { Token = "one-time-token", NewPassword = "AnotherPassword1!" }), CancellationToken.None));
    }
}
