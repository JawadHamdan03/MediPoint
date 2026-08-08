using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Patients.RefreshPatientToken;

public class RefreshPatienttokenRequestHandler(IAppDbContext dbContext,IJwtTokenServiceProvider jwtTokenServiceProvider) : IRequestHandler<RefreshPatienttokenRequest, JwtTokenResponse>
{
    public async Task<JwtTokenResponse> Handle(RefreshPatienttokenRequest request, CancellationToken cancellationToken)
    {
        var refTok = await dbContext.PatientRefreshTokens.Include(rf=>rf.Patient).FirstOrDefaultAsync(rf =>rf.Token.Equals(request.refreshToken));

        if (refTok is null)
            throw new Exception("No refresh token for this user, login first");

        var patient = refTok.Patient;
        var res = await jwtTokenServiceProvider.GenerateJwtToken(patient);
        return res;
    }
}
