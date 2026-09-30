namespace MediPoint.Application.Features.Patients.AddReview.DTOs;

public class AddReviewRequest
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
