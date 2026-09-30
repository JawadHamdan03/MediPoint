using MediPoint.Application.Features.Patients.FindDoctors;
using MediPoint.Common;
using MediPoint.Domain.Entities.Reviews;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Patients.FindDoctors;

public class FindDoctorsQueryHandlerTests
{
    [Fact]
    public async Task Handle_DoctorWithReviews_AttachesAverageRatingAndCount()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var patient = TestEntities.NewPatient();
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Patients.AddAsync(patient);
        await dbContext.Reviews.AddRangeAsync(
            new Review { DoctorId = doctor.Id, PatientId = patient.Id, AppointmentId = Guid.NewGuid(), Rating = 4 },
            new Review { DoctorId = doctor.Id, PatientId = patient.Id, AppointmentId = Guid.NewGuid(), Rating = 2 });
        await dbContext.SaveChangesAsync();
        var handler = new FindDoctorsQueryHandler(dbContext, NullLogger<FindDoctorsQueryHandler>.Instance, new MemoryCache(new MemoryCacheOptions()));

        var result = await handler.Handle(new FindDoctorsQuery(doctor.Specialty), CancellationToken.None);

        var found = Assert.Single(result);
        Assert.Equal(3.0, found.AverageRating);
        Assert.Equal(2, found.ReviewCount);
    }

    [Fact]
    public async Task Handle_DoctorWithNoReviews_HasNullAverageAndZeroCount()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.SaveChangesAsync();
        var handler = new FindDoctorsQueryHandler(dbContext, NullLogger<FindDoctorsQueryHandler>.Instance, new MemoryCache(new MemoryCacheOptions()));

        var result = await handler.Handle(new FindDoctorsQuery(doctor.Specialty), CancellationToken.None);

        var found = Assert.Single(result);
        Assert.Null(found.AverageRating);
        Assert.Equal(0, found.ReviewCount);
    }
}
