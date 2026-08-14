using MediPoint.Domain.Entities.Appointments.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Doctors.AddAppointment.DTOs;

public class ApponitmentDTO
{
    public Guid Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int Duration { get; set; }
    public Guid DoctorId { get; set; }
    public AppointmentStatus Status { get; set; }
}
