using MediPoint.Domain.Common;
using MediPoint.Domain.Entities.Appointments.Enums;
using MediPoint.Domain.Entities.Prescriptions;
using MediPoint.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Domain.Entities.Apointments;

public class Appointment : BaseEntity
{
    public DateTime AppointmentDate { get; set; }
    public int Duration { get; set; }

    public AppointmentStatus Status { get; set; }

    public string? Reason { get; set; }

    public string? Notes { get; set; }

    public string? CancellationReason { get; set; }


    public Guid PatientId { get; set; }
    public Patient? Patient { get; set; }

    public Guid DoctorId{ get; set; }
    public Doctor Doctor { get; set; }


    public Prescription? Prescription { get; set; }

}
