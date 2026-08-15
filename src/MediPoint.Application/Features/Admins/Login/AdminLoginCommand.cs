using MediatR;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Features.Patients.DTOs;

namespace MediPoint.Application.Features.Admins.Login;

public sealed record AdminLoginCommand(LoginRequest loginRequest):IRequest<JwtTokenResponse>;