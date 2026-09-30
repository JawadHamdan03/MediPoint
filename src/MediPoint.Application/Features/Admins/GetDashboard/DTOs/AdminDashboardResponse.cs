namespace MediPoint.Application.Features.Admins.GetDashboard.DTOs;

public class AdminDashboardResponse
{
    public int TotalAppointments { get; set; }
    public int PendingCount { get; set; }
    public int ConfirmedCount { get; set; }
    public int CompletedCount { get; set; }
    public int CancelledCount { get; set; }
    public int MissedCount { get; set; }

    public double CancellationRate { get; set; }
    public double CompletionRate { get; set; }

    public int TotalDoctors { get; set; }
    public int ActiveDoctors { get; set; }
    public int TotalPatients { get; set; }

    public decimal TotalRevenue { get; set; }

    public List<SpecialtyRevenue> RevenueBySpecialty { get; set; } = new();
    public List<DailyAppointmentCount> AppointmentsLast30Days { get; set; } = new();
    public List<TopRatedDoctor> TopRatedDoctors { get; set; } = new();
}

public class SpecialtyRevenue
{
    public string Specialty { get; set; } = null!;
    public decimal Revenue { get; set; }
    public int CompletedAppointments { get; set; }
}

public class DailyAppointmentCount
{
    public DateOnly Date { get; set; }
    public int Count { get; set; }
}

public class TopRatedDoctor
{
    public Guid DoctorId { get; set; }
    public string Name { get; set; } = null!;
    public string Specialty { get; set; } = null!;
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}
