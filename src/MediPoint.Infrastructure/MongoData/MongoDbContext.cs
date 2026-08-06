using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Infrastructure.MongoData;

public class MongoDbContext
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string MedicalRecordsCollectionName { get; set; } = null!;
    public string MedicineCollectionName { get; set; } = null!;
    public string LabResultsCollectionName { get; set; } = null!;
}
