using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Domain.Entities.Prescriptions.Med;

public class Medicine
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public Guid PrescriptionId { get; set; }
    public Guid PatientId { get; set; }
    public string Name { get; set; } = null!;

    public string Dosage { get; set; } = null!;

    public string Frequency { get; set; } = null!;

    public int DurationDays { get; set; }

    public string Instructions { get; set; } = "";
}
