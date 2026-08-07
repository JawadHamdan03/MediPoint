using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Login;



public class LoginCommandHandler(IAppDbContext dbContext, IJwtTokenServiceProvider jwtTokenServiceProvider) : IRequestHandler<LoginCommand, JwtTokenResponse>
{
    public async Task<JwtTokenResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user =await dbContext.Admins.FirstOrDefaultAsync(u=>u.Email.Equals(request.email));
        if(user is null)
        {
            throw new Exception("No user with this Credentials were found");
        }

       if(! BCrypt.Net.BCrypt.Verify(request.password,user.PasswordHash))
        {
            throw new Exception("wrong password");
        }

        var tokenRes =await jwtTokenServiceProvider.GenerateJwtToken(user); 
        
        return tokenRes;

    }
}