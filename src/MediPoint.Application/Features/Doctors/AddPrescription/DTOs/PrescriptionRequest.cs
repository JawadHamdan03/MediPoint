using MediPoint.Domain.Entities.Prescriptions.LabRes;
using MediPoint.Domain.Entities.Prescriptions.Med;
using MediPoint.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Doctors.AddPrescription.DTOs;

public class PrescriptionRequest
{
    public string Notes { get; set; } = "";

    public List<Medicine> Medicines { get; set; } = [];

    public List<LabResult> LabResults { get; set; } = [];
    public string MedicineName { get; set; } = null!;

    public string Dosage { get; set; } = null!;

    public string Frequency { get; set; } = null!;

    public int DurationDays { get; set; }

    public string Instructions { get; set; } = "";

    public string TestName { get; set; } = null!;
    public string Result { get; set; } = null!;

    public string Unit { get; set; } = "";

    public string ReferenceRange { get; set; } = "";

    public Guid AppointmentId { get; set; }
}
