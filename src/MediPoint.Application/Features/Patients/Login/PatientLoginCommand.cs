using MediatR;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Features.Patients.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Patients.Login;

public sealed record PatientLoginCommand(LoginRequest LoginRequest) : IRequest<JwtTokenResponse>;
