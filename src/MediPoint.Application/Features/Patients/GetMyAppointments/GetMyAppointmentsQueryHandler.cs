using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Features.Patients.GetMyAppointments.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Patients.GetMyAppointments;

public class GetMyAppointmentsQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetMyAppointmentsQuery, List<MyAppointmentResponse>>
{
    public async Task<List<MyAppointmentResponse>> Handle(GetMyAppointmentsQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Appointments.AsNoTracking()
            .Where(a => a.PatientId == request.PatientId)
            .OrderByDescending(a => a.AppointmentDate)
            .Select(a => new MyAppointmentResponse
            {
                Id = a.Id,
                AppointmentDate = a.AppointmentDate,
                Duration = a.Duration,
                Status = a.Status,
                Reason = a.Reason,
                Notes = a.Notes,
                CancellationReason = a.CancellationReason,
                DoctorId = a.DoctorId,
                DoctorName = "Dr. " + a.Doctor.FirstName + " " + a.Doctor.LastName,
                HasReview = dbContext.Reviews.Any(r => r.AppointmentId == a.Id),
            })
            .ToListAsync(cancellationToken);
    }
}
