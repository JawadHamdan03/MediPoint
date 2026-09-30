using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.Notifications;
using MediPoint.Domain.Entities.PasswordReset;
using MediPoint.Domain.Entities.Prescriptions;
using MediPoint.Domain.Entities.RefreshToken;
using MediPoint.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MediPoint.Application.Common;



public interface IAppDbContext
{
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<AdminRefreshToken> AdminRefreshTokens { get; set; }
    public DbSet<PatientRefreshToken> PatientRefreshTokens { get; set; }
    public DbSet<DoctorRefreshToken> DoctorRefreshTokens { get; set; }
    public DbSet<AdminPasswordResetToken> AdminPasswordResetTokens { get; set; }
    public DbSet<PatientPasswordResetToken> PatientPasswordResetTokens { get; set; }
    public DbSet<DoctorPasswordResetToken> DoctorPasswordResetTokens { get; set; }
    public DbSet<AdminNotification> AdminNotifications { get; set; }
    public DbSet<PatientNotification> PatientNotifications { get; set; }
    public DbSet<DoctorNotification> DoctorNotifications { get; set; }




    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}