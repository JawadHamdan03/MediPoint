using MediPoint.Domain.Entities.Prescriptions;
using MediPoint.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Infrastructure.Data.Configurations;

public class PrescriptionConfiguratoin : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.HasOne(x => x.Appointment).WithOne(x => x.Prescription)
            .HasForeignKey<Prescription>(x => x.AppointmentId).OnDelete(DeleteBehavior.Cascade);


        builder.HasOne(x => x.Doctor).WithMany(x => x.Prescriptions).HasForeignKey(x => x.DoctorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Patient).WithMany(x => x.Prescriptions).HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Restrict);
      
    }
}
