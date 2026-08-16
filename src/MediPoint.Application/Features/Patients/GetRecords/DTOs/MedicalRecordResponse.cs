using MediPoint.Domain.Entities.Prescriptions.LabRes;
using MediPoint.Domain.Entities.Prescriptions.Med;

namespace MediPoint.Application.Features.Patients.GetRecords.DTOs;

public class MedicalRecordResponse
{

    public Guid PrescriptionId { get; set; }
    
    public string Diagnosis { get; set; } = null!;

    public string Notes { get; set; } = "";

    public List<Medicine> Medicines { get; set; } = [];

    public List<LabResult> LabResults { get; set; } = [];
    
    public string DoctorName { get; set; }
    
}