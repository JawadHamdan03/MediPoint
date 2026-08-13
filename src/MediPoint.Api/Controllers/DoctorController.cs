using MediatR;
using MediPoint.Application.Features.Doctors.AddPrescription;
using MediPoint.Application.Features.Doctors.AddPrescription.DTOs;
using MediPoint.Application.Features.Doctors.AppointmentsQuery;
using MediPoint.Application.Features.Doctors.Login;
using MediPoint.Application.Features.Patients.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    [Authorize(Roles = "Doctor")]
    [HttpGet("get-Appointments-today")]
    public async Task<IActionResult> GetAppointmetnsToday()
    {
        var docId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var res = await mediator.Send(new GetTodaysAppointmentsQuery(Guid.Parse(docId!)));
        return Ok(res);
    }


    [HttpPost("add-prescription")]
    public async Task<IActionResult> addPrescription(PrescriptionRequest prescriptionRequest)
    {
        var res = await mediator.Send(new AddPrescriptionCommand(prescriptionRequest));
        return Ok(res);
    }


}
