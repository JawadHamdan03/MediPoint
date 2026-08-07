using MediatR;
using MediPoint.Api.Requests;
using MediPoint.Application.Features.Login;
using Microsoft.AspNetCore.Mvc;

namespace MediPoint.Api.Controllers;


[Route("/api/[controller]")]
[ApiController]
public class AccountController (IMediator mediator) : ControllerBase
{
    
    [HttpPost("login")]
    public async Task<IActionResult> login(LoginRequest loginRequest)
    {
        var res = await mediator.Send(new LoginCommand(loginRequest.email,loginRequest.password));
        return Ok(res);
    }
}