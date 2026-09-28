using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Admins.AdminAddsDoctor;
using MediPoint.Application.Features.Admins.AdminAddsDoctor.DTOs;
using MediPoint.Common;
using MediPoint.Domain.Entities.User;
using MediPoint.Domain.Entities.User.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Admins.AdminAddsDoctor;

public class AddDoctorCommandHandlerTests
{
    private static DoctorDto ValidRequest() => new()
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
    };

    [Fact]
    public async Task Handle_NewEmail_CreatesDoctor()
    {
        using var dbContext = TestDbContextFactory.Create();
        var handler = new AddDoctorCommandHandler(dbContext, Substitute.For<IMemoryCache>());

        var result = await handler.Handle(new AddDoctorCommand(ValidRequest()), CancellationToken.None);

        Assert.Equal("dr.house@example.com", result.Email);
        var saved = await dbContext.Doctors.SingleAsync();
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
        var handler = new AddDoctorCommandHandler(dbContext, Substitute.For<IMemoryCache>());

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(new AddDoctorCommand(ValidRequest()), CancellationToken.None));
    }
}
