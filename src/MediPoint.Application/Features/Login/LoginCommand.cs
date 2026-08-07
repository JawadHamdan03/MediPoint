using MediatR;
using MediPoint.Application.Common.ServiceResponse;

namespace MediPoint.Application.Features.Login;


public sealed record LoginCommand(string email, string password) : IRequest<JwtTokenResponse>;