using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.RefreshToken;
using MediPoint.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Common;



public interface IAppDbContext
{
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    public DbSet<AdminRefreshToken> AdminRefreshTokens { get; set; }
    public DbSet<PatientRefreshToken> PatientRefreshTokens { get; set; }
    public DbSet<DoctorRefreshToken> DoctorRefreshTokens { get; set; }



    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}