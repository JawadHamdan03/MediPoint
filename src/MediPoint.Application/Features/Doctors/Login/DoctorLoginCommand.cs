using MediatR;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Features.Patients.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Doctors.Login;

public sealed record DoctorLoginCommand(LoginRequest Request) : IRequest<JwtTokenResponse>;
