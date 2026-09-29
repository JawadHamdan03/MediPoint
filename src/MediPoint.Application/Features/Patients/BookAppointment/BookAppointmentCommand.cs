using MediatR;
using MediPoint.Application.Features.Patients.DTOs;

namespace MediPoint.Application.Features.Patients.BookAppointment;

public sealed record BookAppointmentCommand(BookAppointmentRequest Request) : IRequest<AppointmentDTO>;
