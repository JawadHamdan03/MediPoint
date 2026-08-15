using MediatR;
using MediPoint.Application.Common.ServiceResponse;

namespace MediPoint.Application.Features.Admins.AdminRefreshToken;

public record AdminRefreshTokenCommand(string refreshToken): IRequest<JwtTokenResponse>;