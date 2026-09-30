using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Patients.BookAppointment;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Common;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using System.Text.Json;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Patients.BookAppointment;

public class BookAppointmentCommandHandlerTests
{
    [Fact]
    public async Task Handle_OpenSlot_ConfirmsAppointmentForPatient()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var patient = TestEntities.NewPatient();
        var appointment = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(1), 30);
        appointment.Doctor = doctor;
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Patients.AddAsync(patient);
        await dbContext.Appointments.AddAsync(appointment);
        await dbContext.SaveChangesAsync();
        var handler = new BookAppointmentCommandHandler(dbContext, NullLogger<BookAppointmentCommandHandler>.Instance, Substitute.For<IMailer>());

        var result = await handler.Handle(
            new BookAppointmentCommand(new BookAppointmentRequest { AppointmentId = appointment.Id, PatientId = patient.Id }),
            CancellationToken.None);

        Assert.Equal(patient.Id, result.PatientId);

        // Regression guard: the controller returns this value straight to Ok(), so it must be
        // a plain serializable DTO, not a tracked entity with back-reference navigations (which
        // previously caused a JSON cycle exception after the DB write and email had already succeeded).
        var json = JsonSerializer.Serialize(result);
        Assert.Contains(patient.Id.ToString(), json);

        Assert.Equal(1, await dbContext.PatientNotifications.CountAsync(n => n.PatientId == patient.Id));
        Assert.Equal(1, await dbContext.DoctorNotifications.CountAsync(n => n.DoctorId == doctor.Id));
    }

    [Fact]
    public async Task Handle_AlreadyBookedSlot_ThrowsConflictException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var patient = TestEntities.NewPatient();
        var appointment = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(1), 30);
        appointment.Doctor = doctor;
        appointment.Confirm(Guid.NewGuid());
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Patients.AddAsync(patient);
        await dbContext.Appointments.AddAsync(appointment);
        await dbContext.SaveChangesAsync();
        var handler = new BookAppointmentCommandHandler(dbContext, NullLogger<BookAppointmentCommandHandler>.Instance, Substitute.For<IMailer>());

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(
            new BookAppointmentCommand(new BookAppointmentRequest { AppointmentId = appointment.Id, PatientId = patient.Id }),
            CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UnknownAppointment_ThrowsNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var handler = new BookAppointmentCommandHandler(dbContext, NullLogger<BookAppointmentCommandHandler>.Instance, Substitute.For<IMailer>());

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(
            new BookAppointmentCommand(new BookAppointmentRequest { AppointmentId = Guid.NewGuid(), PatientId = Guid.NewGuid() }),
            CancellationToken.None));
    }
}
