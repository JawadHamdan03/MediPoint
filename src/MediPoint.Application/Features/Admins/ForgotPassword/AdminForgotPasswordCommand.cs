using MediatR;
using MediPoint.Application.Features.Patients.DTOs;

namespace MediPoint.Application.Features.Admins.ForgotPassword;

public sealed record AdminForgotPasswordCommand(ForgotPasswordRequest Request) : IRequest<MessageResponse>;
