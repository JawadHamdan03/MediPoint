using MediatR;
using MediPoint.Application.Features.Patients.GetRecords.DTOs;

namespace MediPoint.Application.Features.Patients.GetRecords;

public sealed record GetRecordsCommand(Guid PatientId):IRequest<List<MedicalRecordResponse>>;