using MediatR;
using MediPoint.Application.Features.Patients.BookAppointment;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Application.Features.Patients.FindDoctors;
using MediPoint.Application.Features.Patients.Login;
using MediPoint.Application.Features.Patients.RefreshPatientToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediPoint.Application.Features.Patients.GetRecords;

namespace MediPoint.Api.Controllers;


[Route("/patients")]
[ApiController]
public class PatientController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        var res = await mediator.Send(new PatientLoginCommand(loginRequest));

        return Ok(res);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> refreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        var res = await mediator.Send(new RefreshPatienttokenRequest(refreshTokenRequest.RefreshToken));
        return Ok(res);
    }

    [Authorize(Roles ="Patient")]
    [HttpGet("search-doctors/{speciality}")]
    public async Task<IActionResult> findDoctors(string speciality)
    {
        var res = await mediator.Send(new FindDoctorsQuery(speciality));
        return Ok(res);
    }

    [Authorize(Roles = "Patient")]
    [HttpPost("book-appointment/{appointmentId}")]
    public async Task<IActionResult> bookAppointment(Guid appointmentId)
    {
        var patientId = User.FindFirst(ClaimTypes.NameIdentifier);
        var res = await mediator.Send(new BookAppointmentCommand(
                new BookAppointmentRequest
                {
                    AppointmentId = appointmentId,
                    PatientId = Guid.Parse(patientId.Value)
                }));

        return Ok(res);
    }

    [Authorize(Roles = "Patient")]
    [HttpGet("get-medical-records")]
    public async Task<IActionResult> GetMedicalRecords()
    {
        var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var res = await mediator.Send(new GetRecordsCommand(Guid.Parse(patientId!)));
        return Ok(res);
    }
    

}
