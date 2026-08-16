using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using Microsoft.EntityFrameworkCore;


namespace MediPoint.Application.Features.Doctors.DoctorRefreshToken;

public class DoctorRefreshTokenCommandHandler(IAppDbContext dbContext,IJwtTokenServiceProvider jwtTokenServiceProvider):IRequestHandler<DoctorRefreshTokenCommand,JwtTokenResponse>
{
    public async Task<JwtTokenResponse> Handle(DoctorRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var rf = await dbContext.DoctorRefreshTokens
            .Include(r => r.Doctor)
            .FirstOrDefaultAsync(r => r.Token == request.refreshToken);
       if (rf is null)
       {
           throw new UnauthorizedException("Invalid refresh token, login again");
       }

       if (rf.ExpiresAt <= DateTime.UtcNow)
           throw new UnauthorizedException("Refresh token expired, login again");

       var tokenResponse = await jwtTokenServiceProvider.GenerateJwtToken(rf.Doctor);
       return tokenResponse;
       
    }
}