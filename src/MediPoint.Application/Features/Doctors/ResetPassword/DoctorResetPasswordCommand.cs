using MediatR;
using MediPoint.Application.Features.Patients.DTOs;

namespace MediPoint.Application.Features.Doctors.ResetPassword;

public sealed record DoctorResetPasswordCommand(ResetPasswordRequest Request) : IRequest<MessageResponse>;
