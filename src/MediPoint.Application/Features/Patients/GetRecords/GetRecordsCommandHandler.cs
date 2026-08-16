using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Features.Patients.GetRecords.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Patients.GetRecords;

public class GetRecordsCommandHandler(IAppDbContext dbContext,ILabResultService labResultService,IMedicineService medicineService) 
    : IRequestHandler<GetRecordsCommand,List<MedicalRecordResponse>>
{
    public async Task<List<MedicalRecordResponse>> Handle(GetRecordsCommand request, CancellationToken cancellationToken)
    {
        var patientId = request.PatientId;

        var prescriptions= await dbContext.Prescriptions.Include(p=>p.Doctor).Where(p=>p.PatientId == patientId).ToListAsync();

        List<MedicalRecordResponse> medicalRecordResponses = new List<MedicalRecordResponse>();
        foreach (var pres in prescriptions)
        {
            var labresults = (await labResultService.GetAsync()).Where(x => x.PrescriptionId == pres.Id);
            var medicines = (await medicineService.GetAsync()).Where(x => x.PrescriptionId == pres.Id);
            pres.LabResults = labresults.ToList();
            pres.Medicines = medicines.ToList();

            var medRes = pres.Adapt<MedicalRecordResponse>();
            medRes.DoctorName = pres.Doctor.FirstName+" "+pres.Doctor.LastName;
            
            medicalRecordResponses.Add(medRes);
        }
        
        return  medicalRecordResponses;
        
    }
}