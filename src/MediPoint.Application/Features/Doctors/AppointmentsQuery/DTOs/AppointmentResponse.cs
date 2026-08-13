using MediPoint.Domain.Entities.Appointments.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Doctors.AppointmentsQuery.DTOs;

public class AppointmentResponse
{
    public Guid Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int Duration { get; set; }

    public AppointmentStatus Status { get; set; }

    public string? Reason { get; set; }

    public string? Notes { get; set; }
    public Guid PatientId { get; set; }
}
