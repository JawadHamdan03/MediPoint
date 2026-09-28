using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Doctors.CompleteAppointment;
using MediPoint.Common;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.Appointments.Enums;
using MediPoint.Domain.Entities.User;
using NSubstitute;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Doctors.CompleteAppointment;

public class CompleteAppointmentCommandHandlerTests
{
    [Fact]
    public async Task Handle_OwnConfirmedAppointment_MarksCompleted()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var patient = TestEntities.NewPatient();
        var appointment = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(-1), 30);
        appointment.Doctor = doctor;
        appointment.Confirm(patient.Id);
        appointment.Patient = patient;
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Patients.AddAsync(patient);
        await dbContext.Appointments.AddAsync(appointment);
        await dbContext.SaveChangesAsync();
        var handler = new CompleteAppointmentCommandHandler(dbContext, Substitute.For<IMailer>());

        var result = await handler.Handle(new CompleteAppointmentCommand(appointment.Id, doctor.Id, "All good"), CancellationToken.None);

        Assert.Equal(AppointmentStatus.Completed, result.Status);
        Assert.Equal("All good", result.Notes);
    }

    [Fact]
    public async Task Handle_NotOwnAppointment_ThrowsNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var appointment = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(-1), 30);
        appointment.Doctor = doctor;
        appointment.Confirm(Guid.NewGuid());
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Appointments.AddAsync(appointment);
        await dbContext.SaveChangesAsync();
        var handler = new CompleteAppointmentCommandHandler(dbContext, Substitute.For<IMailer>());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new CompleteAppointmentCommand(appointment.Id, Guid.NewGuid(), null), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_PendingAppointment_ThrowsConflictException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var appointment = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(-1), 30);
        appointment.Doctor = doctor;
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Appointments.AddAsync(appointment);
        await dbContext.SaveChangesAsync();
        var handler = new CompleteAppointmentCommandHandler(dbContext, Substitute.For<IMailer>());

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(new CompleteAppointmentCommand(appointment.Id, doctor.Id, null), CancellationToken.None));
    }
}
