using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Features.Doctors.AddAppointment.DTOs;
using MediPoint.Application.Features.Doctors.GenerateAppointmentSlots.DTOs;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.Appointments.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Doctors.GenerateAppointmentSlots;

public class GenerateAppointmentSlotsCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<GenerateAppointmentSlotsCommand, GenerateAppointmentSlotsResult>
{
    public async Task<GenerateAppointmentSlotsResult> Handle(GenerateAppointmentSlotsCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var rangeStart = req.StartDate.ToDateTime(TimeOnly.MinValue);
        var rangeEnd = req.EndDate.ToDateTime(TimeOnly.MaxValue);

        var existing = await dbContext.Appointments
            .Where(a => a.DoctorId == request.DoctorId
                        && a.Status != AppointmentStatus.Cancelled
                        && a.AppointmentDate >= rangeStart
                        && a.AppointmentDate <= rangeEnd)
            .Select(a => new { a.AppointmentDate, a.Duration })
            .ToListAsync(cancellationToken);

        var newAppointments = new List<Appointment>();
        var result = new GenerateAppointmentSlotsResult();

        for (var date = req.StartDate; date <= req.EndDate; date = date.AddDays(1))
        {
            if (!req.DaysOfWeek.Contains(date.DayOfWeek))
                continue;

            for (var time = req.StartTime; time.AddMinutes(req.SlotDurationMinutes) <= req.EndTime; time = time.AddMinutes(req.SlotDurationMinutes))
            {
                var slotStart = date.ToDateTime(time);
                var slotEnd = slotStart.AddMinutes(req.SlotDurationMinutes);

                var overlaps = existing.Any(a =>
                    slotStart < a.AppointmentDate.AddMinutes(a.Duration) && a.AppointmentDate < slotEnd);

                if (overlaps)
                {
                    result.Skipped++;
                    continue;
                }

                newAppointments.Add(Appointment.Create(request.DoctorId, slotStart, req.SlotDurationMinutes));
                result.Created++;
            }
        }

        await dbContext.Appointments.AddRangeAsync(newAppointments, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        result.Slots = newAppointments.Adapt<List<ApponitmentDTO>>();
        return result;
    }
}
