using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentGroupsHub.DTOs.Requests;
using StudentGroupsHub.Extensions;
using StudentGroupsHub.Services;

namespace StudentGroupsHub.Controllers;

[ApiController]
[Route("api")]
public class EventsController(
    EventService eventService,
    GroupService groupService,
    UserService userService,
    NotificationService notificationService) : ControllerBase
{
    // GET /api/events
    [HttpGet("events")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] Guid? groupId)
    {
        var events = await eventService.GetUpcomingAsync(search, groupId);
        var responses = await Task.WhenAll(events.Select(e => eventService.ToResponseAsync(e)));
        return Ok(responses);
    }

    // GET /api/events/{id}
    [HttpGet("events/{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ev = await eventService.GetByIdAsync(id);
        if (ev is null) return NotFound();
        return Ok(await eventService.ToResponseAsync(ev));
    }

    // GET /api/groups/{groupId}/events
    [HttpGet("groups/{groupId:guid}/events")]
    [AllowAnonymous]
    public async Task<IActionResult> GetForGroup(Guid groupId)
    {
        var events = await eventService.GetForGroupAsync(groupId);
        var responses = await Task.WhenAll(events.Select(e => eventService.ToResponseAsync(e)));
        return Ok(responses);
    }

    // POST /api/groups/{groupId}/events
    [HttpPost("groups/{groupId:guid}/events")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Create(Guid groupId, [FromBody] CreateEventRequest request)
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();
        if (!UserService.IsActive(user)) return Forbid();

        var isAdmin  = user.Role == "admin";
        var isLeader = await groupService.IsLeaderOfGroupAsync(user.Id, groupId);
        if (!isAdmin && !isLeader) return Forbid();

        var ev = await eventService.CreateAsync(groupId, user.Id, request);
        return CreatedAtAction(nameof(GetById), new { id = ev.Id },
            await eventService.ToResponseAsync(ev));
    }

    // PUT /api/groups/{groupId}/events/{eventId}
    [HttpPut("groups/{groupId:guid}/events/{eventId:guid}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Update(Guid groupId, Guid eventId,
        [FromBody] UpdateEventRequest request)
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();
        if (!UserService.IsActive(user)) return Forbid();

        var isAdmin  = user.Role == "admin";
        var isLeader = await groupService.IsLeaderOfGroupAsync(user.Id, groupId);
        if (!isAdmin && !isLeader) return Forbid();

        var ev = await eventService.UpdateAsync(eventId, request);
        if (ev is null) return NotFound();
        return Ok(await eventService.ToResponseAsync(ev));
    }

    // DELETE /api/groups/{groupId}/events/{eventId}
    [HttpDelete("groups/{groupId:guid}/events/{eventId:guid}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Cancel(Guid groupId, Guid eventId)
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();
        if (!UserService.IsActive(user)) return Forbid();

        var isAdmin  = user.Role == "admin";
        var isLeader = await groupService.IsLeaderOfGroupAsync(user.Id, groupId);
        if (!isAdmin && !isLeader) return Forbid();

        var ok = await eventService.CancelAsync(eventId);
        return ok ? NoContent() : NotFound();
    }

    // POST /api/events/{id}/rsvp
    [HttpPost("events/{id:guid}/rsvp")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Rsvp(Guid id, [FromBody] UpsertRsvpRequest request)
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();
        if (!UserService.IsActive(user)) return Forbid();

        var participation = await eventService.UpsertRsvpAsync(id, user.Id, request.Status);
        return Ok(new { participation!.Id, participation.EventId, participation.UserId,
                         participation.Status, participation.RegisteredAt });
    }

    // DELETE /api/events/{id}/rsvp
    [HttpDelete("events/{id:guid}/rsvp")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> CancelRsvp(Guid id)
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();
        if (!UserService.IsActive(user)) return Forbid();

        await eventService.RemoveRsvpAsync(id, user.Id);
        return NoContent();
    }

    // GET /api/events/{id}/rsvp/all
    [HttpGet("events/{id:guid}/rsvp/all")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetRsvps(Guid id)
    {
        var rsvps = await eventService.GetRsvpsAsync(id);
        return Ok(rsvps.Select(p => new {
            p.Id, p.EventId, p.UserId,
            UserDisplayName = p.User?.DisplayName ?? p.User?.Email,
            p.Status, p.RegisteredAt
        }));
    }
}
