using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using MongoDB.Driver.Linq;

namespace MediPoint.Application.Features.Doctors.DoctorRefreshToken;

public class DoctorRefreshTokenCommandHandler(IAppDbContext dbContext,IJwtTokenServiceProvider jwtTokenServiceProvider):IRequestHandler<DoctorRefreshTokenCommand,JwtTokenResponse>
{
    public async Task<JwtTokenResponse> Handle(DoctorRefreshTokenCommand request, CancellationToken cancellationToken)
    {
       var rf = await dbContext.DoctorRefreshTokens.FirstOrDefaultAsync(r=>r.Token.Equals(request.refreshToken));
       if (rf is null)
       {
           throw new Exception("No refresh token for this user, login first");
       }
       
       var tokenResponse = await jwtTokenServiceProvider.GenerateJwtToken(rf.Doctor);
       return tokenResponse;
       
    }
}