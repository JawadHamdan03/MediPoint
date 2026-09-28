using MediPoint.Application.Features.Doctors.AddAppointment.DTOs;

namespace MediPoint.Application.Features.Doctors.GenerateAppointmentSlots.DTOs;

public class GenerateAppointmentSlotsResult
{
    public int Created { get; set; }

    public int Skipped { get; set; }

    public List<ApponitmentDTO> Slots { get; set; } = new();
}
