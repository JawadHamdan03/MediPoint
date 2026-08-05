using MediPoint.Domain.Common;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.Prescriptions.LabRes;
using MediPoint.Domain.Entities.Prescriptions.Med;
using MediPoint.Domain.Entities.User;
using Microsoft.AspNetCore.SignalR.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Domain.Entities.Prescriptions;

public class Prescription : BaseEntity
{

    public string Diagnosis { get; set; } = null!;

    public string Notes { get; set; } = "";

    public List<Medicine> Medicines { get; set; } = [];

    public List<LabResult> LabResults { get; set; } = [];


    public Guid PatientId{ get; set; }
    public Patient Patient { get; set; }

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; }

    public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; }
}
