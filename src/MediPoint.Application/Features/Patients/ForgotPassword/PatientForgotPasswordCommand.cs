using MediatR;
using MediPoint.Application.Features.Patients.DTOs;

namespace MediPoint.Application.Features.Patients.ForgotPassword;

public sealed record PatientForgotPasswordCommand(ForgotPasswordRequest Request) : IRequest<MessageResponse>;
