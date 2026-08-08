using BCrypt.Net;
using DnsClient.Internal;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Patients.Login;

public class PatientLoginCommandHandler(IAppDbContext dbContext,IJwtTokenServiceProvider jwtTokenServiceProvider,ILogger<PatientLoginCommandHandler>logger)
    : IRequestHandler<PatientLoginCommand, JwtTokenResponse>
{
    public async Task<JwtTokenResponse> Handle(PatientLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Patients.AsNoTracking()
            .FirstOrDefaultAsync(u=>u.Email.Equals(request.LoginRequest.Email));

        if (user is null)
        {
            logger.LogError("User with Email {Email} Not Found",request.LoginRequest.Email);
            throw new Exception("User were not found");
        }
        bool loginRes =  BCrypt.Net.BCrypt.Verify(request.LoginRequest.Password, user.PasswordHash);
        
        if (!loginRes)
        {
            logger.LogWarning("User Login with Email {Email} entered wrong password", request.LoginRequest.Email);
            throw new Exception("Wrong Password");
        }

        logger.LogInformation("User with Email {Email} just Logged in",request.LoginRequest.Email);
        var tokenres = await jwtTokenServiceProvider.GenerateJwtToken(user);
        return tokenres; 

    }
}