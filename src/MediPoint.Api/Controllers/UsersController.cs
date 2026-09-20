using MediatR;
using MediPoint.Application.Features.Users.UploadProfileImage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MediPoint.Api.Controllers;

[Route("/users")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpPost("profile-image")]
    public async Task<IActionResult> UploadProfileImage(IFormFile image)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        await using var stream = image.OpenReadStream();
        var res = await mediator.Send(new UploadProfileImageCommand(
            Guid.Parse(userId!), role!, stream, image.FileName, image.ContentType, image.Length));

        return Ok(res);
    }
}
