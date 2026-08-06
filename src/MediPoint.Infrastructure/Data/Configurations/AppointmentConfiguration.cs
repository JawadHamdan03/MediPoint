using MediPoint.Domain.Entities.Apointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPoint.Infrastructure.Data.Configurations;


public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.Property(x=>x.Status).HasConversion<string>();
    }
}