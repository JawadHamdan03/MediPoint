using MediPoint.Application.Common;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.MedicalRecords;
using MediPoint.Domain.Entities.Notifications;
using MediPoint.Domain.Entities.PasswordReset;
using MediPoint.Domain.Entities.Prescriptions;
using MediPoint.Domain.Entities.Prescriptions.LabRes;
using MediPoint.Domain.Entities.Prescriptions.Med;
using MediPoint.Domain.Entities.RefreshToken;
using MediPoint.Domain.Entities.Reviews;
using MediPoint.Domain.Entities.User;
using MediPoint.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MediPoint.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options),IAppDbContext
{
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<AdminRefreshToken> AdminRefreshTokens { get; set; }
    public DbSet<PatientRefreshToken> PatientRefreshTokens { get; set; }
    public DbSet<DoctorRefreshToken> DoctorRefreshTokens { get; set; }
    public DbSet<AdminPasswordResetToken> AdminPasswordResetTokens { get; set; }
    public DbSet<PatientPasswordResetToken> PatientPasswordResetTokens { get; set; }
    public DbSet<DoctorPasswordResetToken> DoctorPasswordResetTokens { get; set; }
    public DbSet<AdminNotification> AdminNotifications { get; set; }
    public DbSet<PatientNotification> PatientNotifications { get; set; }
    public DbSet<DoctorNotification> DoctorNotifications { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Ignore<LabResult>();
        modelBuilder.Ignore<MedicalRecord>();
        modelBuilder.Ignore<Medicine>();


        

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrescriptionConfiguratoin).Assembly);
    }
}
