using MediPoint.Domain.Common;
using MediPoint.Domain.Common.Exceptions;
using MediPoint.Domain.Entities.Appointments.Enums;
using MediPoint.Domain.Entities.Prescriptions;
using MediPoint.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Domain.Entities.Apointments;

public class Appointment : BaseEntity
{
    private Appointment() { }

    public DateTime AppointmentDate { get; private set; }
    public int Duration { get; private set; }

    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Pending;

    public string? Reason { get; private set; }

    public string? Notes { get; private set; }

    public string? CancellationReason { get; private set; }


    public Guid PatientId { get; private set; }
    public Patient? Patient { get; set; }

    public Guid DoctorId { get; private set; }
    public Doctor Doctor { get; set; }

    public Prescription? Prescription { get; set; }

    public static Appointment Create(Guid doctorId, DateTime appointmentDate, int duration, string? reason = null)
    {
        if (duration <= 0)
            throw new DomainException("Appointment duration must be greater than zero.");

        return new Appointment
        {
            DoctorId = doctorId,
            AppointmentDate = appointmentDate,
            Duration = duration,
            Reason = reason,
            Status = AppointmentStatus.Pending
        };
    }

    public void Confirm(Guid patientId)
    {
        if (Status == AppointmentStatus.Confirmed)
            throw new DomainException("This appointment slot is already booked. Please choose another one.");

        if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled)
            throw new DomainException("This appointment is no longer available for booking. Please choose another one.");

        PatientId = patientId;
        Status = AppointmentStatus.Confirmed;
    }

    public void Cancel(string? cancellationReason)
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new DomainException("Appointment is already cancelled.");

        if (Status == AppointmentStatus.Completed)
            throw new DomainException("A completed appointment cannot be cancelled.");

        Status = AppointmentStatus.Cancelled;
        CancellationReason = cancellationReason;
    }

    public void Complete(string? notes)
    {
        if (Status != AppointmentStatus.Confirmed)
            throw new DomainException("Only a confirmed appointment can be marked completed.");

        Status = AppointmentStatus.Completed;
        if (notes is not null)
            Notes = notes;
    }
}
