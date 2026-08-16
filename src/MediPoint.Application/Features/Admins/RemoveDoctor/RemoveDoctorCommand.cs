using MediatR;
using MediPoint.Application.Features.Admins.AdminAddsDoctor.DTOs;

namespace MediPoint.Application.Features.Admins.RemoveDoctor;

public record RemoveDoctorCommand(Guid DoctorId) : IRequest<DoctorDto>;
