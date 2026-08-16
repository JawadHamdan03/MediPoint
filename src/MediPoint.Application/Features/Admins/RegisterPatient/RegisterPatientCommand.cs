using MediatR;
using MediPoint.Application.Features.Admins.RegisterPatient.DTOs;

namespace MediPoint.Application.Features.Admins.RegisterPatient;

public record RegisterPatientCommand(PatientDto patientRequest) : IRequest<PatientDto>;
