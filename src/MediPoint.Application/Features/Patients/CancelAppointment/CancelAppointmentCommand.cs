using MediatR;
using MediPoint.Application.Features.Patients.DTOs;

namespace MediPoint.Application.Features.Patients.CancelAppointment;

public record CancelAppointmentCommand(Guid AppointmentId, Guid PatientId, string? CancellationReason) : IRequest<AppointmentDTO>;
