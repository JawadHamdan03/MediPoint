using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Doctors.AddAppointment;
using MediPoint.Application.Features.Doctors.AddAppointment.DTOs;
using MediPoint.Common;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Doctors.AddAppointment;

public class AddAppointmentCommandHandlerTests
{
    [Fact]
    public async Task Handle_NoOverlap_CreatesAppointment()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.SaveChangesAsync();
        var handler = new AddAppointmentCommandHandler(dbContext);
        var slotStart = new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

        var result = await handler.Handle(
            new AddAppointmentCommand(new ApponitmentDTO { DoctorId = doctor.Id, AppointmentDate = slotStart, Duration = 30 }),
            CancellationToken.None);

        Assert.Equal(doctor.Id, result.DoctorId);
        Assert.Single(await dbContext.Appointments.ToListAsync());
    }

    [Fact]
    public async Task Handle_OverlappingSlotForSameDoctor_ThrowsConflictException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var slotStart = new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);
        var existing = Appointment.Create(doctor.Id, slotStart, 30);
        existing.Doctor = doctor;
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Appointments.AddAsync(existing);
        await dbContext.SaveChangesAsync();
        var handler = new AddAppointmentCommandHandler(dbContext);

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(
            new AddAppointmentCommand(new ApponitmentDTO { DoctorId = doctor.Id, AppointmentDate = slotStart.AddMinutes(15), Duration = 30 }),
            CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancelledOverlappingSlot_DoesNotBlockNewAppointment()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var slotStart = new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);
        var existing = Appointment.Create(doctor.Id, slotStart, 30);
        existing.Doctor = doctor;
        existing.Cancel(null);
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Appointments.AddAsync(existing);
        await dbContext.SaveChangesAsync();
        var handler = new AddAppointmentCommandHandler(dbContext);

        var result = await handler.Handle(
            new AddAppointmentCommand(new ApponitmentDTO { DoctorId = doctor.Id, AppointmentDate = slotStart, Duration = 30 }),
            CancellationToken.None);

        Assert.Equal(doctor.Id, result.DoctorId);
    }
}
