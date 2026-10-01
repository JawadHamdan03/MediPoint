using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Users.GetPrescriptionPdf;

public class GetPrescriptionPdfQueryHandler(
    IAppDbContext dbContext,
    IMedicineService medicineService,
    ILabResultService labResultService,
    IPrescriptionPdfGenerator pdfGenerator)
    : IRequestHandler<GetPrescriptionPdfQuery, byte[]>
{
    public async Task<byte[]> Handle(GetPrescriptionPdfQuery request, CancellationToken cancellationToken)
    {
        var prescription = await dbContext.Prescriptions
            .Include(p => p.Doctor)
            .Include(p => p.Patient)
            .Include(p => p.Appointment)
            .FirstOrDefaultAsync(p => p.Id == request.PrescriptionId, cancellationToken);

        var owned = prescription is not null && request.Role switch
        {
            "Patient" => prescription.PatientId == request.UserId,
            "Doctor" => prescription.DoctorId == request.UserId,
            _ => false,
        };

        if (!owned)
            throw new NotFoundException("Prescription", request.PrescriptionId.ToString());

        var medicines = (await medicineService.GetAsync())
            .Where(m => m.PrescriptionId == prescription!.Id)
            .Select(m => new PrescriptionPdfMedicine
            {
                Name = m.Name,
                Dosage = m.Dosage,
                Frequency = m.Frequency,
                DurationDays = m.DurationDays,
                Instructions = m.Instructions,
            })
            .ToList();

        var labResults = (await labResultService.GetAsync())
            .Where(l => l.PrescriptionId == prescription!.Id)
            .Select(l => new PrescriptionPdfLabResult
            {
                TestName = l.TestName,
                Result = l.Result,
                Unit = l.Unit,
                ReferenceRange = l.ReferenceRange,
            })
            .ToList();

        var model = new PrescriptionPdfModel
        {
            DoctorName = $"{prescription!.Doctor.FirstName} {prescription.Doctor.LastName}",
            DoctorSpecialty = prescription.Doctor.Specialty,
            DoctorLicenseNumber = prescription.Doctor.LicenseNumber,
            PatientName = $"{prescription.Patient.FirstName} {prescription.Patient.LastName}",
            PatientDateOfBirth = prescription.Patient.DateOfBirth,
            AppointmentDate = prescription.Appointment.AppointmentDate,
            IssuedAt = prescription.CreatedAt,
            Diagnosis = prescription.Diagnosis,
            Notes = prescription.Notes,
            Medicines = medicines,
            LabResults = labResults,
        };

        return pdfGenerator.Generate(model);
    }
}
