using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Users.MarkNotificationRead;
using MediPoint.Common;
using MediPoint.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Users.MarkNotificationRead;

public class MarkNotificationReadCommandHandlerTests
{
    [Fact]
    public async Task Handle_OwnNotification_MarksAsRead()
    {
        using var dbContext = TestDbContextFactory.Create();
        var patient = TestEntities.NewPatient();
        await dbContext.Patients.AddAsync(patient);
        var notification = new PatientNotification
        {
            PatientId = patient.Id,
            Title = "t",
            Message = "m",
            Type = NotificationTypes.AppointmentBooked,
        };
        await dbContext.PatientNotifications.AddAsync(notification);
        await dbContext.SaveChangesAsync();
        var handler = new MarkNotificationReadCommandHandler(dbContext);

        await handler.Handle(new MarkNotificationReadCommand(patient.Id, "Patient", notification.Id), CancellationToken.None);

        var saved = await dbContext.PatientNotifications.SingleAsync();
        Assert.True(saved.IsRead);
    }

    [Fact]
    public async Task Handle_AnotherPatientsNotification_ThrowsNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var owner = TestEntities.NewPatient();
        var intruder = TestEntities.NewPatient(email: "intruder@example.com");
        await dbContext.Patients.AddRangeAsync(owner, intruder);
        var notification = new PatientNotification
        {
            PatientId = owner.Id,
            Title = "t",
            Message = "m",
            Type = NotificationTypes.AppointmentBooked,
        };
        await dbContext.PatientNotifications.AddAsync(notification);
        await dbContext.SaveChangesAsync();
        var handler = new MarkNotificationReadCommandHandler(dbContext);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new MarkNotificationReadCommand(intruder.Id, "Patient", notification.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UnknownNotification_ThrowsNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var handler = new MarkNotificationReadCommandHandler(dbContext);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new MarkNotificationReadCommand(Guid.NewGuid(), "Patient", Guid.NewGuid()), CancellationToken.None));
    }
}
