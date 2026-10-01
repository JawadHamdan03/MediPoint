namespace MediPoint.Application.Common.Services;

public interface IPrescriptionPdfGenerator
{
    byte[] Generate(PrescriptionPdfModel model);
}

public class PrescriptionPdfModel
{
    public string DoctorName { get; set; } = null!;
    public string DoctorSpecialty { get; set; } = null!;
    public string DoctorLicenseNumber { get; set; } = null!;

    public string PatientName { get; set; } = null!;
    public DateOnly PatientDateOfBirth { get; set; }

    public DateTime AppointmentDate { get; set; }
    public DateTime IssuedAt { get; set; }

    public string Diagnosis { get; set; } = null!;
    public string Notes { get; set; } = "";

    public List<PrescriptionPdfMedicine> Medicines { get; set; } = new();
    public List<PrescriptionPdfLabResult> LabResults { get; set; } = new();
}

public class PrescriptionPdfMedicine
{
    public string Name { get; set; } = null!;
    public string Dosage { get; set; } = null!;
    public string Frequency { get; set; } = null!;
    public int DurationDays { get; set; }
    public string Instructions { get; set; } = "";
}

public class PrescriptionPdfLabResult
{
    public string TestName { get; set; } = null!;
    public string Result { get; set; } = null!;
    public string Unit { get; set; } = "";
    public string ReferenceRange { get; set; } = "";
}
