using MediatR;
using MediPoint.Application.Features.Doctors.AddPrescription;
using MediPoint.Application.Features.Doctors.AddPrescription.DTOs;
using MediPoint.Application.Features.Doctors.AppointmentsQuery;
using MediPoint.Application.Features.Doctors.Login;
using MediPoint.Application.Features.Patients.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediPoint.Application.Features.Doctors.AddAppointment;
using MediPoint.Application.Features.Doctors.DoctorRefreshToken;
using MediPoint.Application.Features.Doctors.CompleteAppointment;
using MediPoint.Application.Features.Doctors.CompleteAppointment.DTOs;

namespace MediPoint.Api.Controllers;



[Route("api/[controller]")]
[ApiController]

public class DoctorController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> login(LoginRequest loginRequest)
    {
        var res = await mediator.Send(new DoctorLoginCommand(loginRequest));
        return Ok(res);

    }


    [HttpPost("refreshToken")]
    public async Task<IActionResult> reffreshToken([FromBody] string refreshToken)
    {
        var res = await mediator.Send(new DoctorRefreshTokenCommand(refreshToken));
        return Ok(res);
    }

    [Authorize(Roles = "Doctor")]
    [HttpGet("get-Appointments-today")]
    public async Task<IActionResult> GetAppointmetnsToday()
    {
        var docId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var res = await mediator.Send(new GetTodaysAppointmentsQuery(Guid.Parse(docId!)));
        return Ok(res);
    }


    [Authorize(Roles = "Doctor")]
    [HttpPost("add-prescription")]
    public async Task<IActionResult> addPrescription(PrescriptionRequest prescriptionRequest)
    {
        var res = await mediator.Send(new AddPrescriptionCommand(prescriptionRequest));
        return Ok(res);
    }

    [Authorize(Roles = "Doctor")]
    [HttpPost("add-appointment")]
    public async Task<IActionResult> addAppointment(MediPoint.Application.Features.Doctors.AddAppointment.DTOs.ApponitmentDTO appointment)
    {
        var res = await mediator.Send(new AddAppointmentCommand(appointment));
        return Ok(res);
    }

    [Authorize(Roles = "Doctor")]
    [HttpPost("complete-appointment/{appointmentId}")]
    public async Task<IActionResult> CompleteAppointment(Guid appointmentId, CompleteAppointmentRequest request)
    {
        var docId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var res = await mediator.Send(new CompleteAppointmentCommand(appointmentId, Guid.Parse(docId!), request.Notes));
        return Ok(res);
    }


}
