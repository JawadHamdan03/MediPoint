using MediatR;
using MediPoint.Application.Common.ServiceResponse;

namespace MediPoint.Application.Features.Doctors.DoctorRefreshToken;

public sealed record DoctorRefreshTokenCommand(string refreshToken):IRequest<JwtTokenResponse>;