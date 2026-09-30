using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Features.Patients.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Patients.GetDoctorReviews;

public class GetDoctorReviewsQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetDoctorReviewsQuery, List<ReviewResponse>>
{
    public async Task<List<ReviewResponse>> Handle(GetDoctorReviewsQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Reviews.AsNoTracking()
            .Where(r => r.DoctorId == request.DoctorId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewResponse
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                PatientName = r.Patient.FirstName + " " + r.Patient.LastName.Substring(0, 1) + ".",
            })
            .ToListAsync(cancellationToken);
    }
}
