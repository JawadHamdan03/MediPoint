using MediPoint.Domain.Common;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.User;

namespace MediPoint.Domain.Entities.Reviews;

public class Review : BaseEntity
{
    public int Rating { get; set; }
    public string? Comment { get; set; }

    public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;
}
