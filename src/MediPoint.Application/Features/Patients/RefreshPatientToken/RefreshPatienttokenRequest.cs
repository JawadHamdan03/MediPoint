using MediatR;
using MediPoint.Application.Common.ServiceResponse;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Patients.RefreshPatientToken;

public sealed record RefreshPatienttokenRequest(string refreshToken) : IRequest<JwtTokenResponse>;