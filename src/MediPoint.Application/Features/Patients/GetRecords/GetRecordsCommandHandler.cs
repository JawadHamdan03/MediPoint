using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Features.Patients.GetRecords.DTOs;
using MediPoint.Domain.Entities.MedicalRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MediPoint.Application.Features.Patients.GetRecords;

public class GetRecordsCommandHandler(IAppDbContext dbContext,IMemoryCache memoryCache,ILabResultService labResultService,IMedicineService medicineService) 
    : IRequestHandler<GetRecordsCommand,List<MedicalRecordResponse>>
{
    public async Task<List<MedicalRecordResponse>> Handle(GetRecordsCommand request, CancellationToken cancellationToken)
    {
        var patientId = request.PatientId;

        string recordsCacheKey = "records-key";

        List<MedicalRecordResponse>? medicalRecordResponses;
        if (memoryCache.TryGetValue(recordsCacheKey,out medicalRecordResponses))
        {
            return medicalRecordResponses!;
        }
        
        
        var prescriptions= await dbContext.Prescriptions.Include(p=>p.Doctor).Where(p=>p.PatientId == patientId).ToListAsync();

        
        
        
        medicalRecordResponses = new List<MedicalRecordResponse>();
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
        memoryCache.Set(recordsCacheKey,medicalRecordResponses, new MemoryCacheEntryOptions()
        {
            Size = 1,
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });
        return  medicalRecordResponses;
        
    }
}