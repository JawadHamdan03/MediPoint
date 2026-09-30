using MediatR;
using MediPoint.Application.Features.Users.UploadProfileImage;
using MediPoint.Application.Features.Users.GetNotifications;
using MediPoint.Application.Features.Users.MarkNotificationRead;
using MediPoint.Application.Features.Users.MarkAllNotificationsRead;
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

    [Authorize]
    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        var res = await mediator.Send(new GetNotificationsQuery(Guid.Parse(userId!), role!));
        return Ok(res);
    }

    [Authorize]
    [HttpPost("notifications/{notificationId}/read")]
    public async Task<IActionResult> MarkNotificationRead(Guid notificationId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        await mediator.Send(new MarkNotificationReadCommand(Guid.Parse(userId!), role!, notificationId));
        return NoContent();
    }

    [Authorize]
    [HttpPost("notifications/read-all")]
    public async Task<IActionResult> MarkAllNotificationsRead()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        await mediator.Send(new MarkAllNotificationsReadCommand(Guid.Parse(userId!), role!));
        return NoContent();
    }
}
