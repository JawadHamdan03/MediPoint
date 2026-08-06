using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.MedicalRecords;
using MediPoint.Domain.Entities.Prescriptions;
using MediPoint.Domain.Entities.Prescriptions.LabRes;
using MediPoint.Domain.Entities.Prescriptions.Med;
using MediPoint.Domain.Entities.RefreshToken;
using MediPoint.Domain.Entities.User;
using MediPoint.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MediPoint.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    public DbSet<AdminRefreshToken> AdminRefreshTokens { get; set; }
    public DbSet<PatientRefreshToken> PatientRefreshTokens { get; set; }
    public DbSet<DoctorRefreshToken> DoctorRefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Ignore<LabResult>();
        modelBuilder.Ignore<MedicalRecord>();
        modelBuilder.Ignore<Medicine>();


        

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrescriptionConfiguratoin).Assembly);
    }
}
