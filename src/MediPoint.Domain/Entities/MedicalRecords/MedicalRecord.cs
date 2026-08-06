using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Domain.Entities.MedicalRecords;

public class MedicalRecord
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public Guid PatientId { get; set; }

    public Guid DoctorId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Diagnosis { get; set; } = null!;

    public string Notes { get; set; } = "";

    public string Treatment { get; set; } = "";
}
