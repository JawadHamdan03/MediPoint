using MediPoint.Application.Features.Doctors.GenerateAppointmentSlots;
using MediPoint.Application.Features.Doctors.GenerateAppointmentSlots.DTOs;
using MediPoint.Common;
using MediPoint.Domain.Entities.Apointments;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Doctors.GenerateAppointmentSlots;

public class GenerateAppointmentSlotsCommandHandlerTests
{
    private static DateOnly NextMonday()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        while (date.DayOfWeek != DayOfWeek.Monday)
            date = date.AddDays(1);
        return date;
    }

    [Fact]
    public async Task Handle_WeekdayMorningRange_CreatesExpectedSlotCount()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.SaveChangesAsync();
        var handler = new GenerateAppointmentSlotsCommandHandler(dbContext);
        var monday = NextMonday();

        var request = new GenerateAppointmentSlotsRequest
        {
            StartDate = monday,
            EndDate = monday,
            DaysOfWeek = [DayOfWeek.Monday],
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(10, 0),
            SlotDurationMinutes = 30,
        };

        var result = await handler.Handle(new GenerateAppointmentSlotsCommand(doctor.Id, request), CancellationToken.None);

        Assert.Equal(2, result.Created);
        Assert.Equal(0, result.Skipped);
        Assert.Equal(2, await dbContext.Appointments.CountAsync());
    }

    [Fact]
    public async Task Handle_ExistingSlotInRange_IsSkippedNotDuplicated()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var monday = NextMonday();
        var existing = Appointment.Create(doctor.Id, monday.ToDateTime(new TimeOnly(9, 0)), 30);
        existing.Doctor = doctor;
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.Appointments.AddAsync(existing);
        await dbContext.SaveChangesAsync();
        var handler = new GenerateAppointmentSlotsCommandHandler(dbContext);

        var request = new GenerateAppointmentSlotsRequest
        {
            StartDate = monday,
            EndDate = monday,
            DaysOfWeek = [DayOfWeek.Monday],
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(10, 0),
            SlotDurationMinutes = 30,
        };

        var result = await handler.Handle(new GenerateAppointmentSlotsCommand(doctor.Id, request), CancellationToken.None);

        Assert.Equal(1, result.Created);
        Assert.Equal(1, result.Skipped);
        Assert.Equal(2, await dbContext.Appointments.CountAsync());
    }

    [Fact]
    public async Task Handle_DayNotInPattern_GeneratesNoSlotsForThatDay()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.SaveChangesAsync();
        var handler = new GenerateAppointmentSlotsCommandHandler(dbContext);
        var monday = NextMonday();

        var request = new GenerateAppointmentSlotsRequest
        {
            StartDate = monday,
            EndDate = monday.AddDays(1),
            DaysOfWeek = [DayOfWeek.Monday],
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(9, 30),
            SlotDurationMinutes = 30,
        };

        var result = await handler.Handle(new GenerateAppointmentSlotsCommand(doctor.Id, request), CancellationToken.None);

        Assert.Equal(1, result.Created);
    }
}
