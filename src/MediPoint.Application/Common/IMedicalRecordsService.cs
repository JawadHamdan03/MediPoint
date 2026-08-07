using MediPoint.Domain.Entities.MedicalRecords;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Common;

public interface IMedicalRecordsService
{
    Task<List<MedicalRecord>> GetAsync();


    Task<MedicalRecord?> GetAsync(string id);


     Task CreateAsync(MedicalRecord medRecord);


     Task UpdateAsync(string id, MedicalRecord updatedMedRecord);


     Task RemoveAsync(string id);
        
}
