using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Application.Features.Patients.Login;
using MediPoint.Common;
using MediPoint.Domain.Entities.User;
using MediPoint.Domain.Entities.User.Shared;
using MediPoint.Infrastructure.Data;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Patients.Login;

public class PatientLoginCommandHandlerTests
{
    private static async Task<Patient> SeedPatient(AppDbContext dbContext, string password = "Password1!")
    {
        var patient = new Patient
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            PhoneNumber = "+15551234567",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        };
        await dbContext.Patients.AddAsync(patient);
        await dbContext.SaveChangesAsync();
        return patient;
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsJwtToken()
    {
        using var dbContext = TestDbContextFactory.Create();
        await SeedPatient(dbContext);
        var jwt = Substitute.For<IJwtTokenServiceProvider>();
        var expectedResponse = new JwtTokenResponse { AccessToken = "token" };
        jwt.GenerateJwtToken(Arg.Any<BaseUser>()).Returns(expectedResponse);
        var handler = new PatientLoginCommandHandler(dbContext, jwt, NullLogger<PatientLoginCommandHandler>.Instance, Substitute.For<IMailer>());

        var result = await handler.Handle(new PatientLoginCommand(new LoginRequest { Email = "jane.doe@example.com", Password = "Password1!" }), CancellationToken.None);

        Assert.Same(expectedResponse, result);
    }

    [Fact]
    public async Task Handle_UnknownEmail_ThrowsUnauthorizedException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var handler = new PatientLoginCommandHandler(dbContext, Substitute.For<IJwtTokenServiceProvider>(), NullLogger<PatientLoginCommandHandler>.Instance, Substitute.For<IMailer>());

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new PatientLoginCommand(new LoginRequest { Email = "missing@example.com", Password = "Password1!" }), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorizedException()
    {
        using var dbContext = TestDbContextFactory.Create();
        await SeedPatient(dbContext);
        var handler = new PatientLoginCommandHandler(dbContext, Substitute.For<IJwtTokenServiceProvider>(), NullLogger<PatientLoginCommandHandler>.Instance, Substitute.For<IMailer>());

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new PatientLoginCommand(new LoginRequest { Email = "jane.doe@example.com", Password = "WrongPassword1!" }), CancellationToken.None));
    }
}
