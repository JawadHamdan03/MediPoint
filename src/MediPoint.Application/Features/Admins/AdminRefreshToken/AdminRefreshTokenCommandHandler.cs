using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Admins.AdminRefreshToken;

public class AdminRefreshTokenCommandHandler(IAppDbContext dbContext,IJwtTokenServiceProvider jwtTokenServiceProvider): IRequestHandler<AdminRefreshTokenCommand,JwtTokenResponse>
{
    public async Task<JwtTokenResponse> Handle(AdminRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var rf = await dbContext.AdminRefreshTokens.Include(r=>r.Admin).
            FirstOrDefaultAsync(r=>r.Token.Equals(request.refreshToken));

        if (rf is null)
        {
            throw new Exception("No refresh token for this user, login first");
        }

        var tokenResponse = await jwtTokenServiceProvider.GenerateJwtToken(rf.Admin);
        return tokenResponse;

    }
    
    
}