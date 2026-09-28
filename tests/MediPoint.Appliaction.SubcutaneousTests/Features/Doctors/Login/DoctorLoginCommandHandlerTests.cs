using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Doctors.Login;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Common;
using MediPoint.Domain.Entities.User;
using MediPoint.Domain.Entities.User.Shared;
using MediPoint.Infrastructure.Data;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Doctors.Login;

public class DoctorLoginCommandHandlerTests
{
    private static async Task<Doctor> SeedDoctor(AppDbContext dbContext, string password = "Password1!")
    {
        var doctor = new Doctor
        {
            FirstName = "Greg",
            LastName = "House",
            Email = "dr.house@example.com",
            PhoneNumber = "+15551234567",
            Specialty = "Diagnostics",
            LicenseNumber = "LIC-001",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        };
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.SaveChangesAsync();
        return doctor;
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsJwtToken()
    {
        using var dbContext = TestDbContextFactory.Create();
        await SeedDoctor(dbContext);
        var jwt = Substitute.For<IJwtTokenServiceProvider>();
        var expectedResponse = new JwtTokenResponse { AccessToken = "token" };
        jwt.GenerateJwtToken(Arg.Any<BaseUser>()).Returns(expectedResponse);
        var handler = new DoctorLoginCommandHandler(dbContext, jwt, NullLogger<DoctorLoginCommandHandler>.Instance, Substitute.For<IMailer>());

        var result = await handler.Handle(new DoctorLoginCommand(new LoginRequest { Email = "dr.house@example.com", Password = "Password1!" }), CancellationToken.None);

        Assert.Same(expectedResponse, result);
    }

    [Fact]
    public async Task Handle_UnknownEmail_ThrowsUnauthorizedException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var handler = new DoctorLoginCommandHandler(dbContext, Substitute.For<IJwtTokenServiceProvider>(), NullLogger<DoctorLoginCommandHandler>.Instance, Substitute.For<IMailer>());

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new DoctorLoginCommand(new LoginRequest { Email = "missing@example.com", Password = "Password1!" }), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorizedException()
    {
        using var dbContext = TestDbContextFactory.Create();
        await SeedDoctor(dbContext);
        var handler = new DoctorLoginCommandHandler(dbContext, Substitute.For<IJwtTokenServiceProvider>(), NullLogger<DoctorLoginCommandHandler>.Instance, Substitute.For<IMailer>());

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new DoctorLoginCommand(new LoginRequest { Email = "dr.house@example.com", Password = "WrongPassword1!" }), CancellationToken.None));
    }
}
