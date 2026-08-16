using MediPoint.Domain.Entities.Prescriptions.Med;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Common;

public interface IMedicineService
{
    Task<List<Medicine>> GetAsync();
     

    Task<Medicine?> GetAsync(string id);

    public Task<Medicine?> GetByPatientIdAsync(Guid id);
    
     Task CreateAsync(Medicine medRecord);
       

     Task UpdateAsync(string id, Medicine updatedMedRecord);
        

     Task RemoveAsync(string id);
        
}
