using MediPoint.Application.Features.Patients.GetDoctorReviews;
using MediPoint.Common;
using MediPoint.Domain.Entities.Reviews;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Patients.GetDoctorReviews;

public class GetDoctorReviewsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOnlyThatDoctorsReviewsNewestFirstWithMaskedPatientName()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var otherDoctor = TestEntities.NewDoctor(email: "other.doctor@example.com");
        var patient = TestEntities.NewPatient(firstName: "Jane", lastName: "Doe");
        await dbContext.Doctors.AddRangeAsync(doctor, otherDoctor);
        await dbContext.Patients.AddAsync(patient);
        await dbContext.Reviews.AddRangeAsync(
            new Review { DoctorId = doctor.Id, PatientId = patient.Id, AppointmentId = Guid.NewGuid(), Rating = 4, Comment = "Older", CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new Review { DoctorId = doctor.Id, PatientId = patient.Id, AppointmentId = Guid.NewGuid(), Rating = 5, Comment = "Newer", CreatedAt = DateTime.UtcNow },
            new Review { DoctorId = otherDoctor.Id, PatientId = patient.Id, AppointmentId = Guid.NewGuid(), Rating = 1, Comment = "NotMine" });
        await dbContext.SaveChangesAsync();
        var handler = new GetDoctorReviewsQueryHandler(dbContext);

        var result = await handler.Handle(new GetDoctorReviewsQuery(doctor.Id), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("Newer", result[0].Comment);
        Assert.Equal("Older", result[1].Comment);
        Assert.Equal("Jane D.", result[0].PatientName);
    }

    [Fact]
    public async Task Handle_NoReviews_ReturnsEmptyList()
    {
        using var dbContext = TestDbContextFactory.Create();
        var handler = new GetDoctorReviewsQueryHandler(dbContext);

        var result = await handler.Handle(new GetDoctorReviewsQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Empty(result);
    }
}
