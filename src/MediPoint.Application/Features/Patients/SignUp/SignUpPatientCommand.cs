using MediatR;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Features.Patients.SignUp.DTOs;

namespace MediPoint.Application.Features.Patients.SignUp;

public record SignUpPatientCommand(PatientSignUpDto Request) : IRequest<JwtTokenResponse>;
