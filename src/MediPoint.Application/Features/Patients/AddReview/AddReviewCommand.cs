using MediatR;
using MediPoint.Application.Features.Patients.AddReview.DTOs;
using MediPoint.Application.Features.Patients.DTOs;

namespace MediPoint.Application.Features.Patients.AddReview;

public record AddReviewCommand(Guid AppointmentId, Guid PatientId, AddReviewRequest Request) : IRequest<ReviewResponse>;
