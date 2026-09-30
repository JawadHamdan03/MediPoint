using MediatR;
using MediPoint.Application.Features.Patients.DTOs;

namespace MediPoint.Application.Features.Doctors.ForgotPassword;

public sealed record DoctorForgotPasswordCommand(ForgotPasswordRequest Request) : IRequest<MessageResponse>;
