using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.User.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Domain.Entities.User;

public class Doctor : BaseUser
{
    public string Specialty { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public int YearsOfExperience { get; set; }

    public decimal ConsultationFee { get; set; }

    public string Biography { get; set; } = "";

    public bool IsAvailable { get; set; } = true;


    public List<Appointment> Appointments { get; set; } = new List<Appointment>();
}
