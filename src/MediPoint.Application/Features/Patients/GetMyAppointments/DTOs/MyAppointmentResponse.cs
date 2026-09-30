using MediPoint.Domain.Entities.Appointments.Enums;

namespace MediPoint.Application.Features.Patients.GetMyAppointments.DTOs;

public class MyAppointmentResponse
{
    public Guid Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int Duration { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = null!;
    public bool HasReview { get; set; }
}
