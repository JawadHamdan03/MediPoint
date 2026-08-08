using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Patients.DTOs;

public class BookAppointmentRequest
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
}
