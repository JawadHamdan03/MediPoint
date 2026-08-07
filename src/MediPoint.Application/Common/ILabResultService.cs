using MediPoint.Domain.Entities.Prescriptions.LabRes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Common;

public interface ILabResultService
{
    Task<List<LabResult>> GetAsync();
      

    Task<LabResult?> GetAsync(string id);
     

    Task CreateAsync(LabResult medRecord);
       

    Task UpdateAsync(string id, LabResult updatedMedRecord);
       

     Task RemoveAsync(string id);
      
}
