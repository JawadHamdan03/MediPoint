using MediPoint.Domain.Entities.Reviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediPoint.Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasIndex(x => x.AppointmentId).IsUnique();

        builder.HasOne(x => x.Appointment).WithOne(x => x.Review)
            .HasForeignKey<Review>(x => x.AppointmentId).OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Doctor).WithMany(x => x.Reviews).HasForeignKey(x => x.DoctorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Patient).WithMany(x => x.Reviews).HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Restrict);
    }
}
