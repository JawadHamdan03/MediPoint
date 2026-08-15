using MediatR;
using MediPoint.Application.Features.Admins.AdminAddsDoctor.DTOs;

namespace MediPoint.Application.Features.Admins.AdminAddsDoctor;

public record AddDoctorCommand(DoctorDto doctorRequest):IRequest<DoctorDto>;