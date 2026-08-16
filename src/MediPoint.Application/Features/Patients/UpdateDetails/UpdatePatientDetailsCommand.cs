using MediatR;
using MediPoint.Application.Features.Patients.UpdateDetails.DTOs;

namespace MediPoint.Application.Features.Patients.UpdateDetails;

public record UpdatePatientDetailsCommand(Guid PatientId, UpdatePatientDto Details) : IRequest<UpdatePatientDto>;
