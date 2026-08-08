using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Patients.DTOs;

public class DoctorResponse
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Specialty { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public int YearsOfExperience { get; set; }

    public decimal ConsultationFee { get; set; }

    public string Biography { get; set; } = "";

    public List<AppointmentDTO> AppointmentDTOs { get; set; }
}
