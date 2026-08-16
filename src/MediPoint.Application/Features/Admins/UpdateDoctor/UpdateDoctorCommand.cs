using MediatR;
using MediPoint.Application.Features.Admins.AdminAddsDoctor.DTOs;
using MediPoint.Application.Features.Admins.UpdateDoctor.DTOs;

namespace MediPoint.Application.Features.Admins.UpdateDoctor;

public record UpdateDoctorCommand(Guid DoctorId, UpdateDoctorDto Doctor) : IRequest<DoctorDto>;
