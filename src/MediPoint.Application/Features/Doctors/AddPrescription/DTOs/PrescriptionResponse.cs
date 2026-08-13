using MediPoint.Domain.Entities.Prescriptions.LabRes;
using MediPoint.Domain.Entities.Prescriptions.Med;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Doctors.AddPrescription.DTOs;

public class PrescriptionResponse
{
    public string Notes { get; set; } = "";

    public List<Medicine> Medicines { get; set; } = [];

    public List<LabResult> LabResults { get; set; } = [];


    public Guid PatientId { get; set; }
   
    public Guid AppointmentId { get; set; }
}
