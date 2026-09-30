using MediPoint.Application.Common;
using MediPoint.Application.Features.Users.MarkAllNotificationsRead;
using MediPoint.Common;
using MediPoint.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Users.MarkAllNotificationsRead;

public class MarkAllNotificationsReadCommandHandlerTests
{
    [Fact]
    public async Task Handle_MarksAllUnreadForThatPatientOnly()
    {
        using var dbContext = TestDbContextFactory.Create();
        var patient = TestEntities.NewPatient();
        var otherPatient = TestEntities.NewPatient(email: "other@example.com");
        await dbContext.Patients.AddRangeAsync(patient, otherPatient);
        await dbContext.PatientNotifications.AddRangeAsync(
            new PatientNotification { PatientId = patient.Id, Title = "a", Message = "m", Type = NotificationTypes.AppointmentBooked },
            new PatientNotification { PatientId = patient.Id, Title = "b", Message = "m", Type = NotificationTypes.AppointmentBooked, IsRead = true },
            new PatientNotification { PatientId = otherPatient.Id, Title = "c", Message = "m", Type = NotificationTypes.AppointmentBooked });
        await dbContext.SaveChangesAsync();
        var handler = new MarkAllNotificationsReadCommandHandler(dbContext);

        await handler.Handle(new MarkAllNotificationsReadCommand(patient.Id, "Patient"), CancellationToken.None);

        var mineRead = await dbContext.PatientNotifications.Where(n => n.PatientId == patient.Id).AllAsync(n => n.IsRead);
        var othersUnaffected = await dbContext.PatientNotifications.Where(n => n.PatientId == otherPatient.Id).AllAsync(n => !n.IsRead);
        Assert.True(mineRead);
        Assert.True(othersUnaffected);
    }
}
