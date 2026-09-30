using MediatR;
using MediPoint.Application.Features.Patients.GetMyAppointments.DTOs;

namespace MediPoint.Application.Features.Patients.GetMyAppointments;

public record GetMyAppointmentsQuery(Guid PatientId) : IRequest<List<MyAppointmentResponse>>;
