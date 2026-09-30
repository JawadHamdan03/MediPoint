using MediatR;
using MediPoint.Application.Features.Patients.DTOs;

namespace MediPoint.Application.Features.Admins.ResetPassword;

public sealed record AdminResetPasswordCommand(ResetPasswordRequest Request) : IRequest<MessageResponse>;
