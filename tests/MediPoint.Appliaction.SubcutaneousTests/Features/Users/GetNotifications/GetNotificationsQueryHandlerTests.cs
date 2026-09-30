using MediPoint.Application.Common;
using MediPoint.Application.Features.Users.GetNotifications;
using MediPoint.Common;
using MediPoint.Domain.Entities.Notifications;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Users.GetNotifications;

public class GetNotificationsQueryHandlerTests
{
    [Fact]
    public async Task Handle_Patient_ReturnsOnlyThatPatientsNotificationsNewestFirst()
    {
        using var dbContext = TestDbContextFactory.Create();
        var patient = TestEntities.NewPatient();
        var otherPatient = TestEntities.NewPatient(email: "other@example.com");
        await dbContext.Patients.AddRangeAsync(patient, otherPatient);
        await dbContext.PatientNotifications.AddAsync(new PatientNotification
        {
            PatientId = patient.Id,
            Title = "Older",
            Message = "m1",
            Type = NotificationTypes.AppointmentBooked,
            CreatedAt = DateTime.UtcNow.AddHours(-1),
        });
        await dbContext.PatientNotifications.AddAsync(new PatientNotification
        {
            PatientId = patient.Id,
            Title = "Newer",
            Message = "m2",
            Type = NotificationTypes.AppointmentConfirmed,
            CreatedAt = DateTime.UtcNow,
        });
        await dbContext.PatientNotifications.AddAsync(new PatientNotification
        {
            PatientId = otherPatient.Id,
            Title = "NotMine",
            Message = "m3",
            Type = NotificationTypes.AppointmentBooked,
        });
        await dbContext.SaveChangesAsync();
        var handler = new GetNotificationsQueryHandler(dbContext);

        var result = await handler.Handle(new GetNotificationsQuery(patient.Id, "Patient"), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("Newer", result[0].Title);
        Assert.Equal("Older", result[1].Title);
    }

    [Fact]
    public async Task Handle_Doctor_ReturnsOnlyThatDoctorsNotifications()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.DoctorNotifications.AddAsync(new DoctorNotification
        {
            DoctorId = doctor.Id,
            Title = "New booking",
            Message = "m",
            Type = NotificationTypes.AppointmentBooked,
        });
        await dbContext.SaveChangesAsync();
        var handler = new GetNotificationsQueryHandler(dbContext);

        var result = await handler.Handle(new GetNotificationsQuery(doctor.Id, "Doctor"), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("New booking", result[0].Title);
    }
}
