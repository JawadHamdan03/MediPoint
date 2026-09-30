using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Features.Admins.GetDashboard.DTOs;
using MediPoint.Domain.Entities.Appointments.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Admins.GetDashboard;

public class GetDashboardQueryHandler(IAppDbContext dbContext) : IRequestHandler<GetDashboardQuery, AdminDashboardResponse>
{
    public async Task<AdminDashboardResponse> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var statusCounts = await dbContext.Appointments.AsNoTracking()
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        int CountFor(AppointmentStatus status) => statusCounts.FirstOrDefault(s => s.Status == status)?.Count ?? 0;

        var pending = CountFor(AppointmentStatus.Pending);
        var confirmed = CountFor(AppointmentStatus.Confirmed);
        var completed = CountFor(AppointmentStatus.Completed);
        var cancelled = CountFor(AppointmentStatus.Cancelled);
        var total = pending + confirmed + completed + cancelled;

        var missed = await dbContext.Appointments.AsNoTracking()
            .CountAsync(a => a.Status == AppointmentStatus.Confirmed && a.AppointmentDate < now, cancellationToken);

        var totalDoctors = await dbContext.Doctors.CountAsync(cancellationToken);
        var activeDoctors = await dbContext.Doctors.CountAsync(d => d.IsAvailable, cancellationToken);
        var totalPatients = await dbContext.Patients.CountAsync(cancellationToken);

        var revenueBySpecialty = await dbContext.Appointments.AsNoTracking()
            .Where(a => a.Status == AppointmentStatus.Completed)
            .GroupBy(a => a.Doctor.Specialty)
            .Select(g => new SpecialtyRevenue
            {
                Specialty = g.Key,
                Revenue = g.Sum(a => a.Doctor.ConsultationFee),
                CompletedAppointments = g.Count(),
            })
            .OrderByDescending(s => s.Revenue)
            .ToListAsync(cancellationToken);

        var thirtyDaysAgo = now.Date.AddDays(-29);
        var dailyCounts = await dbContext.Appointments.AsNoTracking()
            .Where(a => a.CreatedAt >= thirtyDaysAgo)
            .GroupBy(a => a.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        // Zero-fill every day in the window so the trend chart always renders 30 evenly-spaced
        // bars, regardless of how sparse actual appointment activity is.
        var appointmentsLast30Days = Enumerable.Range(0, 30)
            .Select(offset =>
            {
                var date = thirtyDaysAgo.AddDays(offset);
                var match = dailyCounts.FirstOrDefault(d => d.Date == date);
                return new DailyAppointmentCount { Date = DateOnly.FromDateTime(date), Count = match?.Count ?? 0 };
            })
            .ToList();

        var topRatedDoctors = await dbContext.Reviews.AsNoTracking()
            .GroupBy(r => new { r.DoctorId, r.Doctor.FirstName, r.Doctor.LastName, r.Doctor.Specialty })
            .Select(g => new TopRatedDoctor
            {
                DoctorId = g.Key.DoctorId,
                Name = "Dr. " + g.Key.FirstName + " " + g.Key.LastName,
                Specialty = g.Key.Specialty,
                AverageRating = g.Average(r => r.Rating),
                ReviewCount = g.Count(),
            })
            .OrderByDescending(d => d.AverageRating)
            .ThenByDescending(d => d.ReviewCount)
            .Take(5)
            .ToListAsync(cancellationToken);

        return new AdminDashboardResponse
        {
            TotalAppointments = total,
            PendingCount = pending,
            ConfirmedCount = confirmed,
            CompletedCount = completed,
            CancelledCount = cancelled,
            MissedCount = missed,
            CancellationRate = total == 0 ? 0 : Math.Round((double)cancelled / total, 3),
            CompletionRate = total == 0 ? 0 : Math.Round((double)completed / total, 3),
            TotalDoctors = totalDoctors,
            ActiveDoctors = activeDoctors,
            TotalPatients = totalPatients,
            TotalRevenue = revenueBySpecialty.Sum(s => s.Revenue),
            RevenueBySpecialty = revenueBySpecialty,
            AppointmentsLast30Days = appointmentsLast30Days,
            TopRatedDoctors = topRatedDoctors,
        };
    }
}
