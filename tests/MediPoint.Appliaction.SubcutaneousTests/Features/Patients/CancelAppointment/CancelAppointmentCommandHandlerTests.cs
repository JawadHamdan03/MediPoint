using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Patients.CancelAppointment;
using MediPoint.Common;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.Appointments.Enums;
using MediPoint.Domain.Entities.User;
using NSubstitute;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Patients.CancelAppointment;

public class CancelAppointmentCommandHandlerTests
{
    [Fact]
    public async Task Handle_OwnConfirmedAppointment_CancelsIt()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var patientId = Guid.NewGuid();
        var appointment = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(1), 30);
        appointment.Doctor = doctor;
        appointment.Confirm(patientId);
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Appointments.AddAsync(appointment);
        await dbContext.SaveChangesAsync();
        var handler = new CancelAppointmentCommandHandler(dbContext, Substitute.For<IMailer>());

        var result = await handler.Handle(new CancelAppointmentCommand(appointment.Id, patientId, "Can't make it"), CancellationToken.None);

        Assert.Equal(AppointmentStatus.Cancelled, result.Status);
    }

    [Fact]
    public async Task Handle_NotOwnAppointment_ThrowsNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var appointment = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(1), 30);
        appointment.Doctor = doctor;
        appointment.Confirm(Guid.NewGuid());
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Appointments.AddAsync(appointment);
        await dbContext.SaveChangesAsync();
        var handler = new CancelAppointmentCommandHandler(dbContext, Substitute.For<IMailer>());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new CancelAppointmentCommand(appointment.Id, Guid.NewGuid(), null), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AlreadyCancelled_ThrowsConflictException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var patientId = Guid.NewGuid();
        var appointment = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(1), 30);
        appointment.Doctor = doctor;
        appointment.Confirm(patientId);
        appointment.Cancel(null);
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Appointments.AddAsync(appointment);
        await dbContext.SaveChangesAsync();
        var handler = new CancelAppointmentCommandHandler(dbContext, Substitute.For<IMailer>());

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(new CancelAppointmentCommand(appointment.Id, patientId, null), CancellationToken.None));
    }
}
