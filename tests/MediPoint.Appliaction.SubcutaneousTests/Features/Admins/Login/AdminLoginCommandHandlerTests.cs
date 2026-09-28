using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Admins.Login;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Common;
using MediPoint.Domain.Entities.User;
using MediPoint.Domain.Entities.User.Shared;
using MediPoint.Infrastructure.Data;
using NSubstitute;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Admins.Login;

public class AdminLoginCommandHandlerTests
{
    private static async Task<Admin> SeedAdmin(AppDbContext dbContext, string password = "Password1!")
    {
        var admin = new Admin
        {
            FirstName = "Ada",
            LastName = "Admin",
            Email = "admin@example.com",
            PhoneNumber = "+15551234567",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        };
        await dbContext.Admins.AddAsync(admin);
        await dbContext.SaveChangesAsync();
        return admin;
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsJwtToken()
    {
        using var dbContext = TestDbContextFactory.Create();
        await SeedAdmin(dbContext);
        var jwt = Substitute.For<IJwtTokenServiceProvider>();
        var expectedResponse = new JwtTokenResponse { AccessToken = "token" };
        jwt.GenerateJwtToken(Arg.Any<BaseUser>()).Returns(expectedResponse);
        var handler = new AdminLoginCommandHandler(dbContext, Substitute.For<IMailer>(), jwt);

        var result = await handler.Handle(new AdminLoginCommand(new LoginRequest { Email = "admin@example.com", Password = "Password1!" }), CancellationToken.None);

        Assert.Same(expectedResponse, result);
    }

    [Fact]
    public async Task Handle_UnknownEmail_ThrowsUnauthorizedException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var handler = new AdminLoginCommandHandler(dbContext, Substitute.For<IMailer>(), Substitute.For<IJwtTokenServiceProvider>());

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new AdminLoginCommand(new LoginRequest { Email = "missing@example.com", Password = "Password1!" }), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorizedException()
    {
        using var dbContext = TestDbContextFactory.Create();
        await SeedAdmin(dbContext);
        var handler = new AdminLoginCommandHandler(dbContext, Substitute.For<IMailer>(), Substitute.For<IJwtTokenServiceProvider>());

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new AdminLoginCommand(new LoginRequest { Email = "admin@example.com", Password = "WrongPassword1!" }), CancellationToken.None));
    }
}
