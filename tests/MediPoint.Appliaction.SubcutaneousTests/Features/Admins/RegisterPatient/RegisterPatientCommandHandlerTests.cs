using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Admins.RegisterPatient;
using MediPoint.Application.Features.Admins.RegisterPatient.DTOs;
using MediPoint.Common;
using MediPoint.Domain.Entities.User;
using MediPoint.Domain.Entities.User.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Admins.RegisterPatient;

public class RegisterPatientCommandHandlerTests
{
    private static PatientDto ValidRequest() => new()
    {
        FirstName = "Jane",
        LastName = "Doe",
        Password = "Password1!",
        Email = "jane.doe@example.com",
        PhoneNumber = "+15551234567",
        DateOfBirth = new DateOnly(1995, 1, 1),
        Gender = Gender.Female,
    };

    [Fact]
    public async Task Handle_NewEmail_CreatesPatient()
    {
        using var dbContext = TestDbContextFactory.Create();
        var handler = new RegisterPatientCommandHandler(dbContext);

        var result = await handler.Handle(new RegisterPatientCommand(ValidRequest()), CancellationToken.None);

        Assert.Equal("jane.doe@example.com", result.Email);
        var saved = await dbContext.Patients.SingleAsync();
        Assert.True(BCrypt.Net.BCrypt.Verify("Password1!", saved.PasswordHash));
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
        var handler = new RegisterPatientCommandHandler(dbContext);

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(new RegisterPatientCommand(ValidRequest()), CancellationToken.None));
    }
}
