using MediatR;
using MediPoint.Application.Features.Admins.AdminAddsDoctor;
using MediPoint.Application.Features.Admins.AdminAddsDoctor.DTOs;
using MediPoint.Application.Features.Admins.AdminRefreshToken;
using MediPoint.Application.Features.Admins.Login;
using MediPoint.Application.Features.Patients.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediPoint.Api.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class AdminController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> login(LoginRequest loginRequest)
    {
        var res = await mediator.Send(new AdminLoginCommand(loginRequest));
        return Ok(res);
    }


    [HttpPost("refreshtoken")]
    public async Task<IActionResult> refreshToken([FromBody]string refreshToken)
    {
        var res = await mediator.Send(new AdminRefreshTokenCommand(refreshToken));
        return Ok(res);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("Add-doctor")]
    public async Task<IActionResult> addDoctor(DoctorDto doctor)
    {
        var res = await mediator.Send(new AddDoctorCommand(doctor));
        return Ok(res);
    }
    
    
}