using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Doctors.SignUp;
using MediPoint.Application.Features.Doctors.SignUp.DTOs;
using MediPoint.Common;
using MediPoint.Domain.Entities.User;
using MediPoint.Domain.Entities.User.Shared;
using MediPoint.Domain.Entities.User.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Doctors.SignUp;

public class SignUpDoctorCommandHandlerTests
{
    private static DoctorSignUpDto ValidRequest() => new()
    {
        FirstName = "Greg",
        LastName = "House",
        Password = "Password1!",
        Email = "dr.house@example.com",
        PhoneNumber = "+15551234567",
        DateOfBirth = new DateOnly(1970, 1, 1),
        Gender = Gender.Male,
        Specialty = "Diagnostics",
        LicenseNumber = "LIC-001",
        YearsOfExperience = 20,
        ConsultationFee = 100,
        Biography = "",
    };

    [Fact]
    public async Task Handle_NewEmail_CreatesDoctorAndReturnsJwtToken()
    {
        using var dbContext = TestDbContextFactory.Create();
        var jwt = Substitute.For<IJwtTokenServiceProvider>();
        var expectedResponse = new JwtTokenResponse { AccessToken = "token", RefreshToken = "refresh" };
        jwt.GenerateJwtToken(Arg.Any<BaseUser>()).Returns(expectedResponse);
        var handler = new SignUpDoctorCommandHandler(dbContext, jwt);

        var result = await handler.Handle(new SignUpDoctorCommand(ValidRequest()), CancellationToken.None);

        Assert.Same(expectedResponse, result);
        var saved = await dbContext.Doctors.SingleAsync();
        Assert.Equal("dr.house@example.com", saved.Email);
        Assert.True(saved.IsAvailable);
        Assert.True(BCrypt.Net.BCrypt.Verify("Password1!", saved.PasswordHash));
    }

    [Fact]
    public async Task Handle_EmailAlreadyRegistered_ThrowsConflictException()
    {
        using var dbContext = TestDbContextFactory.Create();
        await dbContext.Doctors.AddAsync(new Doctor
        {
            Email = "dr.house@example.com",
            FirstName = "Existing",
            LastName = "Doctor",
            PhoneNumber = "+15559990000",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password1!"),
            Specialty = "General",
            LicenseNumber = "LIC-999",
        });
        await dbContext.SaveChangesAsync();
        var jwt = Substitute.For<IJwtTokenServiceProvider>();
        var handler = new SignUpDoctorCommandHandler(dbContext, jwt);

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(new SignUpDoctorCommand(ValidRequest()), CancellationToken.None));
        await jwt.DidNotReceive().GenerateJwtToken(Arg.Any<BaseUser>());
    }
}
