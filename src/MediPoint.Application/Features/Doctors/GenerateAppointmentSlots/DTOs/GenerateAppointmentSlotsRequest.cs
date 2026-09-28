namespace MediPoint.Application.Features.Doctors.GenerateAppointmentSlots.DTOs;

public class GenerateAppointmentSlotsRequest
{
    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public List<DayOfWeek> DaysOfWeek { get; set; } = new();

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int SlotDurationMinutes { get; set; }
}
