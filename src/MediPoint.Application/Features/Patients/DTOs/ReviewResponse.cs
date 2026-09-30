namespace MediPoint.Application.Features.Patients.DTOs;

public class ReviewResponse
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PatientName { get; set; } = null!;
}
