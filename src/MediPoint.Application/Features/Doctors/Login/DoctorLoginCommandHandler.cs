using DnsClient.Internal;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Doctors.Login;

public class DoctorLoginCommandHandler(IAppDbContext dbContext,IJwtTokenServiceProvider jwtTokenServiceProvider,
    ILogger<DoctorLoginCommandHandler> logger)
    : IRequestHandler<DoctorLoginCommand, JwtTokenResponse>
{
    public async Task<JwtTokenResponse> Handle(DoctorLoginCommand request, CancellationToken cancellationToken)
    {
        var doc =await dbContext.Doctors.FirstOrDefaultAsync(d => d.Email.Equals(request.Request.Email));
        if(doc is null)
        {
            logger.LogError("Doctor with email {Email} was not found",request.Request.Email);
            throw new NotFoundException("Doctor",request.Request.Email);
        }

        if(BCrypt.Net.BCrypt.Verify(request.Request.Password,doc.PasswordHash))
        {

            var jwtRes = await jwtTokenServiceProvider.GenerateJwtToken(doc);
            logger.LogInformation("Doctor with Id {DoctorId} logged in",doc.Id);
            return jwtRes;
        }
        logger.LogWarning("Wrong Password for Doctor with Id {DoctorId}.", doc.Id);
        throw new WrongPasswordException(request.Request.Password);
    }
}
