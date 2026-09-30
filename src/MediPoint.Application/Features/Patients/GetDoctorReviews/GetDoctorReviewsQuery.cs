using MediatR;
using MediPoint.Application.Features.Patients.DTOs;

namespace MediPoint.Application.Features.Patients.GetDoctorReviews;

public record GetDoctorReviewsQuery(Guid DoctorId) : IRequest<List<ReviewResponse>>;
