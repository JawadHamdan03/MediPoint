using MediPoint.Application.Features.Admins.GetDashboard;
using MediPoint.Common;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.Reviews;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Admins.GetDashboard;

public class GetDashboardQueryHandlerTests
{
    [Fact]
    public async Task Handle_MixOfStatuses_ComputesCountsAndRatesCorrectly()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var patient = TestEntities.NewPatient();
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Patients.AddAsync(patient);

        var pending = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(1), 30);
        pending.Doctor = doctor;

        var confirmed = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(2), 30);
        confirmed.Doctor = doctor;
        confirmed.Confirm(patient.Id);

        var completed = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(-1), 30);
        completed.Doctor = doctor;
        completed.Confirm(patient.Id);
        completed.Complete(null);

        var cancelled = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(3), 30);
        cancelled.Doctor = doctor;
        cancelled.Confirm(patient.Id);
        cancelled.Cancel("changed mind");

        await dbContext.Appointments.AddRangeAsync(pending, confirmed, completed, cancelled);
        await dbContext.SaveChangesAsync();
        var handler = new GetDashboardQueryHandler(dbContext);

        var result = await handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        Assert.Equal(4, result.TotalAppointments);
        Assert.Equal(1, result.PendingCount);
        Assert.Equal(1, result.ConfirmedCount);
        Assert.Equal(1, result.CompletedCount);
        Assert.Equal(1, result.CancelledCount);
        Assert.Equal(0.25, result.CancellationRate);
        Assert.Equal(0.25, result.CompletionRate);
        Assert.Equal(1, result.TotalDoctors);
        Assert.Equal(1, result.ActiveDoctors);
        Assert.Equal(1, result.TotalPatients);
    }

    [Fact]
    public async Task Handle_ConfirmedAppointmentInPast_CountsAsMissed()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var patient = TestEntities.NewPatient();
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Patients.AddAsync(patient);

        var missed = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(-2), 30);
        missed.Doctor = doctor;
        missed.Confirm(patient.Id);

        await dbContext.Appointments.AddAsync(missed);
        await dbContext.SaveChangesAsync();
        var handler = new GetDashboardQueryHandler(dbContext);

        var result = await handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        Assert.Equal(1, result.MissedCount);
    }

    [Fact]
    public async Task Handle_CompletedAppointments_AggregatesRevenueBySpecialty()
    {
        using var dbContext = TestDbContextFactory.Create();
        var cardiologist = TestEntities.NewDoctor(email: "cardio@example.com", specialty: "Cardiology");
        cardiologist.ConsultationFee = 100m;
        var dermatologist = TestEntities.NewDoctor(email: "derm@example.com", specialty: "Dermatology");
        dermatologist.ConsultationFee = 50m;
        var patient = TestEntities.NewPatient();
        await dbContext.Doctors.AddRangeAsync(cardiologist, dermatologist);
        await dbContext.Patients.AddAsync(patient);

        var visit1 = Appointment.Create(cardiologist.Id, DateTime.UtcNow.AddDays(-1), 30);
        visit1.Doctor = cardiologist;
        visit1.Confirm(patient.Id);
        visit1.Complete(null);

        var visit2 = Appointment.Create(cardiologist.Id, DateTime.UtcNow.AddDays(-2), 30);
        visit2.Doctor = cardiologist;
        visit2.Confirm(patient.Id);
        visit2.Complete(null);

        var visit3 = Appointment.Create(dermatologist.Id, DateTime.UtcNow.AddDays(-1), 30);
        visit3.Doctor = dermatologist;
        visit3.Confirm(patient.Id);
        visit3.Complete(null);

        await dbContext.Appointments.AddRangeAsync(visit1, visit2, visit3);
        await dbContext.SaveChangesAsync();
        var handler = new GetDashboardQueryHandler(dbContext);

        var result = await handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        Assert.Equal(250m, result.TotalRevenue);
        var cardioRevenue = result.RevenueBySpecialty.Single(s => s.Specialty == "Cardiology");
        Assert.Equal(200m, cardioRevenue.Revenue);
        Assert.Equal(2, cardioRevenue.CompletedAppointments);
        var dermRevenue = result.RevenueBySpecialty.Single(s => s.Specialty == "Dermatology");
        Assert.Equal(50m, dermRevenue.Revenue);
    }

    [Fact]
    public async Task Handle_Reviews_ReturnsTopRatedDoctorsOrderedByAverage()
    {
        using var dbContext = TestDbContextFactory.Create();
        var betterDoctor = TestEntities.NewDoctor(email: "better@example.com", firstName: "Better", lastName: "Doc");
        var worseDoctor = TestEntities.NewDoctor(email: "worse@example.com", firstName: "Worse", lastName: "Doc");
        var patient = TestEntities.NewPatient();
        await dbContext.Doctors.AddRangeAsync(betterDoctor, worseDoctor);
        await dbContext.Patients.AddAsync(patient);
        await dbContext.Reviews.AddRangeAsync(
            new Review { DoctorId = betterDoctor.Id, PatientId = patient.Id, AppointmentId = Guid.NewGuid(), Rating = 5 },
            new Review { DoctorId = betterDoctor.Id, PatientId = patient.Id, AppointmentId = Guid.NewGuid(), Rating = 5 },
            new Review { DoctorId = worseDoctor.Id, PatientId = patient.Id, AppointmentId = Guid.NewGuid(), Rating = 2 });
        await dbContext.SaveChangesAsync();
        var handler = new GetDashboardQueryHandler(dbContext);

        var result = await handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        Assert.Equal(2, result.TopRatedDoctors.Count);
        Assert.Equal("Dr. Better Doc", result.TopRatedDoctors[0].Name);
        Assert.Equal(5, result.TopRatedDoctors[0].AverageRating);
        Assert.Equal(2, result.TopRatedDoctors[0].ReviewCount);
    }

    [Fact]
    public async Task Handle_SparseActivity_ZeroFillsAllThirtyDaysForTheTrendChart()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        await dbContext.Doctors.AddAsync(doctor);

        // Only one appointment created "today" — without zero-filling, the trend
        // series would contain a single entry instead of 30 evenly-spaced days.
        var appointment = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(1), 30);
        appointment.Doctor = doctor;
        await dbContext.Appointments.AddAsync(appointment);
        await dbContext.SaveChangesAsync();
        var handler = new GetDashboardQueryHandler(dbContext);

        var result = await handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        Assert.Equal(30, result.AppointmentsLast30Days.Count);
        Assert.Equal(1, result.AppointmentsLast30Days.Count(d => d.Count > 0));
        Assert.Equal(29, result.AppointmentsLast30Days.Count(d => d.Count == 0));
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow.Date), result.AppointmentsLast30Days[^1].Date);
    }

    [Fact]
    public async Task Handle_NoAppointments_ReturnsZeroedRatesWithoutDivideByZero()
    {
        using var dbContext = TestDbContextFactory.Create();
        var handler = new GetDashboardQueryHandler(dbContext);

        var result = await handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        Assert.Equal(0, result.TotalAppointments);
        Assert.Equal(0, result.CancellationRate);
        Assert.Equal(0, result.CompletionRate);
    }
}
