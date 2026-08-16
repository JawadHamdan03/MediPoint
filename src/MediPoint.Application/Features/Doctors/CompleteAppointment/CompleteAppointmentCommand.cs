using MediatR;
using MediPoint.Application.Features.Doctors.AppointmentsQuery.DTOs;

namespace MediPoint.Application.Features.Doctors.CompleteAppointment;

public record CompleteAppointmentCommand(Guid AppointmentId, Guid DoctorId, string? Notes) : IRequest<AppointmentResponse>;
