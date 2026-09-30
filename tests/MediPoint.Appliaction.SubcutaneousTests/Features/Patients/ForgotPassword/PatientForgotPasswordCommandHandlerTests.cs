using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Application.Features.Patients.ForgotPassword;
using MediPoint.Common;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Patients.ForgotPassword;

public class PatientForgotPasswordCommandHandlerTests
{
    [Fact]
    public async Task Handle_KnownEmail_CreatesTokenAndSendsEmail()
    {
        using var dbContext = TestDbContextFactory.Create();
        var patient = TestEntities.NewPatient();
        await dbContext.Patients.AddAsync(patient);
        await dbContext.SaveChangesAsync();
        var mailer = Substitute.For<IMailer>();
        var urlProvider = Substitute.For<IAppUrlProvider>();
        urlProvider.BuildPasswordResetUrl(Arg.Is("Patient"), Arg.Any<string>()).Returns(ci => $"http://localhost/reset-password?token={ci.ArgAt<string>(1)}");
        var handler = new PatientForgotPasswordCommandHandler(dbContext, mailer, urlProvider);

        var result = await handler.Handle(new PatientForgotPasswordCommand(new ForgotPasswordRequest { Email = patient.Email }), CancellationToken.None);

        Assert.False(string.IsNullOrWhiteSpace(result.Message));
        Assert.Equal(1, await dbContext.PatientPasswordResetTokens.CountAsync());
        await mailer.Received(1).SendEmailAsync(patient.Email, Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_UnknownEmail_ReturnsGenericMessageWithoutSendingEmail()
    {
        using var dbContext = TestDbContextFactory.Create();
        var mailer = Substitute.For<IMailer>();
        var urlProvider = Substitute.For<IAppUrlProvider>();
        var handler = new PatientForgotPasswordCommandHandler(dbContext, mailer, urlProvider);

        var result = await handler.Handle(new PatientForgotPasswordCommand(new ForgotPasswordRequest { Email = "missing@example.com" }), CancellationToken.None);

        Assert.False(string.IsNullOrWhiteSpace(result.Message));
        Assert.Equal(0, await dbContext.PatientPasswordResetTokens.CountAsync());
        await mailer.DidNotReceive().SendEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_CalledTwice_InvalidatesPreviousToken()
    {
        using var dbContext = TestDbContextFactory.Create();
        var patient = TestEntities.NewPatient();
        await dbContext.Patients.AddAsync(patient);
        await dbContext.SaveChangesAsync();
        var mailer = Substitute.For<IMailer>();
        var urlProvider = Substitute.For<IAppUrlProvider>();
        var handler = new PatientForgotPasswordCommandHandler(dbContext, mailer, urlProvider);

        await handler.Handle(new PatientForgotPasswordCommand(new ForgotPasswordRequest { Email = patient.Email }), CancellationToken.None);
        await handler.Handle(new PatientForgotPasswordCommand(new ForgotPasswordRequest { Email = patient.Email }), CancellationToken.None);

        Assert.Equal(1, await dbContext.PatientPasswordResetTokens.CountAsync());
    }
}
