using MediatR;
using MediPoint.Application.Features.Patients.Chat.DTOs;

namespace MediPoint.Application.Features.Patients.Chat;

public sealed record ChatCommand(string Message, Guid PatientId) : IRequest<ChatResult>;
