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
    ILogger<DoctorLoginCommandHandler> logger,IMailer mailer)
    : IRequestHandler<DoctorLoginCommand, JwtTokenResponse>
{
    public async Task<JwtTokenResponse> Handle(DoctorLoginCommand request, CancellationToken cancellationToken)
    {
        var doc = await dbContext.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.Email.Equals(request.Request.Email));
        if (doc is null)
        {
            logger.LogWarning("Login failed: no doctor with email {Email}", request.Request.Email);
            throw new UnauthorizedException("Invalid email or password");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Request.Password, doc.PasswordHash))
        {
            logger.LogWarning("Login failed: wrong password for doctor with Id {DoctorId}", doc.Id);
            throw new UnauthorizedException("Invalid email or password");
        }

        await mailer.SendEmailAsync(doc.Email, "Login Success", $"Welcome back Dr. {doc.FirstName} {doc.LastName}.");
        var jwtRes = await jwtTokenServiceProvider.GenerateJwtToken(doc);
        logger.LogInformation("Doctor with Id {DoctorId} logged in", doc.Id);
        return jwtRes;
    }
}
