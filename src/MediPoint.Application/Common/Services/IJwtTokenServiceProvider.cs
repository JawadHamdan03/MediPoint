using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Domain.Entities.User.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Common.Services;

public interface IJwtTokenServiceProvider
{
    Task<JwtTokenResponse> GenerateJwtToken(BaseUser user);
    Task<string> GenerateRefreshToken(BaseUser user);
}


