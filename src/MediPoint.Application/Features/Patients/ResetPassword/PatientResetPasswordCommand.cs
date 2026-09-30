using MediatR;
using MediPoint.Application.Features.Patients.DTOs;

namespace MediPoint.Application.Features.Patients.ResetPassword;

public sealed record PatientResetPasswordCommand(ResetPasswordRequest Request) : IRequest<MessageResponse>;
