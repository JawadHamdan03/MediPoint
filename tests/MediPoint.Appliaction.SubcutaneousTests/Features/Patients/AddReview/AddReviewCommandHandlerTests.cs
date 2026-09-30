using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Patients.AddReview;
using MediPoint.Application.Features.Patients.AddReview.DTOs;
using MediPoint.Common;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.Reviews;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Patients.AddReview;

public class AddReviewCommandHandlerTests
{
    [Fact]
    public async Task Handle_CompletedOwnAppointment_CreatesReview()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var patient = TestEntities.NewPatient();
        var appointment = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(-1), 30);
        appointment.Doctor = doctor;
        appointment.Confirm(patient.Id);
        appointment.Complete(null);
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Patients.AddAsync(patient);
        await dbContext.Appointments.AddAsync(appointment);
        await dbContext.SaveChangesAsync();
        var handler = new AddReviewCommandHandler(dbContext);

        var result = await handler.Handle(
            new AddReviewCommand(appointment.Id, patient.Id, new AddReviewRequest { Rating = 5, Comment = "Great!" }),
            CancellationToken.None);

        Assert.Equal(5, result.Rating);
        Assert.Equal("Great!", result.Comment);
        Assert.StartsWith(patient.FirstName, result.PatientName);
        var saved = await dbContext.Reviews.SingleAsync();
        Assert.Equal(doctor.Id, saved.DoctorId);
        Assert.Equal(patient.Id, saved.PatientId);
    }

    [Fact]
    public async Task Handle_NotOwnAppointment_ThrowsNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var appointment = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(-1), 30);
        appointment.Doctor = doctor;
        appointment.Confirm(Guid.NewGuid());
        appointment.Complete(null);
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Appointments.AddAsync(appointment);
        await dbContext.SaveChangesAsync();
        var handler = new AddReviewCommandHandler(dbContext);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(
            new AddReviewCommand(appointment.Id, Guid.NewGuid(), new AddReviewRequest { Rating = 4 }),
            CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NotCompletedAppointment_ThrowsConflictException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var patient = TestEntities.NewPatient();
        var appointment = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(1), 30);
        appointment.Doctor = doctor;
        appointment.Confirm(patient.Id);
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Patients.AddAsync(patient);
        await dbContext.Appointments.AddAsync(appointment);
        await dbContext.SaveChangesAsync();
        var handler = new AddReviewCommandHandler(dbContext);

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(
            new AddReviewCommand(appointment.Id, patient.Id, new AddReviewRequest { Rating = 4 }),
            CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AlreadyReviewed_ThrowsConflictException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var patient = TestEntities.NewPatient();
        var appointment = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(-1), 30);
        appointment.Doctor = doctor;
        appointment.Confirm(patient.Id);
        appointment.Complete(null);
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Patients.AddAsync(patient);
        await dbContext.Appointments.AddAsync(appointment);
        await dbContext.Reviews.AddAsync(new Review { AppointmentId = appointment.Id, PatientId = patient.Id, DoctorId = doctor.Id, Rating = 3 });
        await dbContext.SaveChangesAsync();
        var handler = new AddReviewCommandHandler(dbContext);

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(
            new AddReviewCommand(appointment.Id, patient.Id, new AddReviewRequest { Rating = 5 }),
            CancellationToken.None));
    }
}
