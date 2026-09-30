using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Domain.Entities.Appointments.Enums;
using MediPoint.Domain.Entities.Reviews;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Patients.AddReview;

public class AddReviewCommandHandler(IAppDbContext dbContext) : IRequestHandler<AddReviewCommand, ReviewResponse>
{
    public async Task<ReviewResponse> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        var appointment = await dbContext.Appointments.FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);

        if (appointment is null || appointment.PatientId != request.PatientId)
            throw new NotFoundException("Appointment", request.AppointmentId.ToString());

        if (appointment.Status != AppointmentStatus.Completed)
            throw new ConflictException("Only a completed appointment can be reviewed.");

        var alreadyReviewed = await dbContext.Reviews.AnyAsync(r => r.AppointmentId == request.AppointmentId, cancellationToken);
        if (alreadyReviewed)
            throw new ConflictException("This appointment has already been reviewed.");

        var patient = await dbContext.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);

        var review = new Review
        {
            AppointmentId = appointment.Id,
            PatientId = request.PatientId,
            DoctorId = appointment.DoctorId,
            Rating = request.Request.Rating,
            Comment = request.Request.Comment,
        };

        await dbContext.Reviews.AddAsync(review, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ReviewResponse
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            PatientName = patient is not null ? $"{patient.FirstName} {patient.LastName[..1]}." : "Patient",
        };
    }
}
