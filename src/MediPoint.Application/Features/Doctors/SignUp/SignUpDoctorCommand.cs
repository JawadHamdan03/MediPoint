using MediatR;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Features.Doctors.SignUp.DTOs;

namespace MediPoint.Application.Features.Doctors.SignUp;

public record SignUpDoctorCommand(DoctorSignUpDto Request) : IRequest<JwtTokenResponse>;
