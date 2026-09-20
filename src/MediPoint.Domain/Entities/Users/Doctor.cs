using MediPoint.Domain.Common.Exceptions;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.User.Shared;
using MediPoint.Domain.Entities.Prescriptions;
using System;
using System.Collections.Generic;
using System.Text;
using MediPoint.Domain.Entities.RefreshToken;

namespace MediPoint.Domain.Entities.User;

public class Doctor : BaseUser
{
    public string Specialty { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public int YearsOfExperience { get; set; }

    public decimal ConsultationFee { get; set; }

    public string Biography { get; set; } = "";

    public bool IsAvailable { get; private set; } = true;


    public List<Appointment> Appointments { get; set; } = new List<Appointment>();
    public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public List<DoctorRefreshToken> DoctorRefreshTokens { get; set; }

    public void Deactivate()
    {
        if (!IsAvailable)
            throw new DomainException("Doctor is already removed.");

        IsAvailable = false;
    }
}
