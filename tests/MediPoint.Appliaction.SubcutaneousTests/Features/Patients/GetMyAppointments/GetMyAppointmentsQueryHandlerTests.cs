using MediPoint.Application.Features.Patients.GetMyAppointments;
using MediPoint.Common;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.Reviews;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Patients.GetMyAppointments;

public class GetMyAppointmentsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOwnAppointmentsWithDoctorNameAndReviewFlag()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor(firstName: "Greg", lastName: "House");
        var patient = TestEntities.NewPatient();
        var otherPatient = TestEntities.NewPatient(email: "other@example.com");
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Patients.AddRangeAsync(patient, otherPatient);

        var reviewed = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(-2), 30);
        reviewed.Doctor = doctor;
        reviewed.Confirm(patient.Id);
        reviewed.Complete(null);

        var unreviewed = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(-1), 30);
        unreviewed.Doctor = doctor;
        unreviewed.Confirm(patient.Id);
        unreviewed.Complete(null);

        var notMine = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(1), 30);
        notMine.Doctor = doctor;
        notMine.Confirm(otherPatient.Id);

        await dbContext.Appointments.AddRangeAsync(reviewed, unreviewed, notMine);
        await dbContext.Reviews.AddAsync(new Review { AppointmentId = reviewed.Id, DoctorId = doctor.Id, PatientId = patient.Id, Rating = 5 });
        await dbContext.SaveChangesAsync();
        var handler = new GetMyAppointmentsQueryHandler(dbContext);

        var result = await handler.Handle(new GetMyAppointmentsQuery(patient.Id), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.All(result, a => Assert.Equal("Dr. Greg House", a.DoctorName));
        Assert.True(result.Single(a => a.Id == reviewed.Id).HasReview);
        Assert.False(result.Single(a => a.Id == unreviewed.Id).HasReview);
    }
}
