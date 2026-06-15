using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentGroupsHub.Extensions;
using StudentGroupsHub.Services;

namespace StudentGroupsHub.Controllers;

[ApiController]
[Route("api")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class NotificationsController(
    NotificationService notificationService,
    UserService userService) : ControllerBase
{
    // GET /api/users/me/notifications
    [HttpGet("users/me/notifications")]
    public async Task<IActionResult> GetMine()
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();

        var notifications = await notificationService.GetForUserAsync(user.Id);
        return Ok(notifications.Select(NotificationService.ToResponse));
    }

    // PATCH /api/notifications/{id}/read
    [HttpPatch("notifications/{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();

        await notificationService.MarkReadAsync(id, user.Id);
        return NoContent();
    }

    // PATCH /api/users/me/notifications/read-all
    [HttpPatch("users/me/notifications/read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();

        await notificationService.MarkAllReadAsync(user.Id);
        return NoContent();
    }
}
