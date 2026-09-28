using MediatR;
using MediPoint.Application.Features.Doctors.GenerateAppointmentSlots.DTOs;

namespace MediPoint.Application.Features.Doctors.GenerateAppointmentSlots;

public record GenerateAppointmentSlotsCommand(Guid DoctorId, GenerateAppointmentSlotsRequest Request) : IRequest<GenerateAppointmentSlotsResult>;
