using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Admins.Login;

public class AdminLoginCommandHandler(IAppDbContext dbContext,IJwtTokenServiceProvider jwtTokenServiceProvider) : IRequestHandler<AdminLoginCommand,JwtTokenResponse>
{
    public async Task<JwtTokenResponse> Handle(AdminLoginCommand request, CancellationToken cancellationToken)
    {
        var admin = await dbContext.Admins.AsNoTracking().FirstOrDefaultAsync(a=>a.Email.Equals(request.loginRequest.Email));

        if (admin is null)
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.loginRequest.Password, admin.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password");
        }
        var tokenResponse =await  jwtTokenServiceProvider.GenerateJwtToken(admin);
        return tokenResponse;

    }
}