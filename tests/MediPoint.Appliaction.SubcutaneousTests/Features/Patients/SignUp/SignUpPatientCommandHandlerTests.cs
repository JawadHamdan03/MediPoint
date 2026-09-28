using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Patients.SignUp;
using MediPoint.Application.Features.Patients.SignUp.DTOs;
using MediPoint.Common;
using MediPoint.Domain.Entities.User;
using MediPoint.Domain.Entities.User.Shared;
using MediPoint.Domain.Entities.User.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Patients.SignUp;

public class SignUpPatientCommandHandlerTests
{
    private static PatientSignUpDto ValidRequest() => new()
    {
        FirstName = "Jane",
        LastName = "Doe",
        Password = "Password1!",
        Email = "jane.doe@example.com",
        PhoneNumber = "+15551234567",
        DateOfBirth = new DateOnly(1995, 1, 1),
        Gender = Gender.Female,
        BloodType = "O+",
        Address = "1 Main St",
        EmergencyContactName = "John Doe",
        EmergencyContactPhone = "+15557654321",
    };

    [Fact]
    public async Task Handle_NewEmail_CreatesPatientAndReturnsJwtToken()
    {
        using var dbContext = TestDbContextFactory.Create();
        var jwt = Substitute.For<IJwtTokenServiceProvider>();
        var expectedResponse = new JwtTokenResponse { AccessToken = "token", RefreshToken = "refresh" };
        jwt.GenerateJwtToken(Arg.Any<BaseUser>()).Returns(expectedResponse);
        var handler = new SignUpPatientCommandHandler(dbContext, jwt);

        var result = await handler.Handle(new SignUpPatientCommand(ValidRequest()), CancellationToken.None);

        Assert.Same(expectedResponse, result);
        var saved = await dbContext.Patients.SingleAsync();
        Assert.Equal("jane.doe@example.com", saved.Email);
        Assert.True(BCrypt.Net.BCrypt.Verify("Password1!", saved.PasswordHash));
        await jwt.Received(1).GenerateJwtToken(Arg.Is<Patient>(p => p.Email == "jane.doe@example.com"));
    }

    [Fact]
    public async Task Handle_EmailAlreadyRegistered_ThrowsConflictException()
    {
        using var dbContext = TestDbContextFactory.Create();
        await dbContext.Patients.AddAsync(new Patient
        {
            Email = "jane.doe@example.com",
            FirstName = "Existing",
            LastName = "Patient",
            PhoneNumber = "+15559990000",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password1!"),
        });
        await dbContext.SaveChangesAsync();
        var jwt = Substitute.For<IJwtTokenServiceProvider>();
        var handler = new SignUpPatientCommandHandler(dbContext, jwt);

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(new SignUpPatientCommand(ValidRequest()), CancellationToken.None));
        await jwt.DidNotReceive().GenerateJwtToken(Arg.Any<BaseUser>());
    }
}
