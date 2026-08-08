using MediPoint.Domain.Entities.Appointments.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Patients.DTOs;

public class AppointmentDTO
{
    public Guid Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int Duration { get; set; }

    public AppointmentStatus Status { get; set; }

}
