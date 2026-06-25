using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentGroupsHub.DTOs.Requests;
using StudentGroupsHub.Extensions;
using StudentGroupsHub.Services;

namespace StudentGroupsHub.Controllers;

[ApiController]
[Route("api/leadership-requests")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class LeadershipRequestsController(
    LeadershipRequestService leadershipRequestService,
    UserService userService,
    NotificationService notificationService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeadershipRequest request)
    {
        var oid = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();

        var req = await leadershipRequestService.CreateAsync(
            request.GroupId, user.Id, request.ContactEmail, request.Reason);

        return CreatedAtAction(nameof(GetMine), new { id = req.Id }, LeadershipRequestService.ToResponse(req));
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var oid = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();

        var reqs = await leadershipRequestService.GetByUserAsync(user.Id);
        return Ok(reqs.Select(LeadershipRequestService.ToResponse));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPending()
    {
        var oid = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null || user.Role != "admin") return Forbid();

        var reqs = await leadershipRequestService.GetAllPendingAsync();
        return Ok(reqs.Select(LeadershipRequestService.ToResponse));
    }

    [HttpPatch("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ReviewLeadershipRequest? body)
    {
        var oid = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null || user.Role != "admin") return Forbid();

        var req = await leadershipRequestService.ApproveAsync(id, user.Id, body?.DecisionNotes);
        if (req is null) return NotFound();

        await notificationService.CreateAsync(req.RequestedByUserId,
            "leadership_request_approved",
            $"Tu solicitud de liderazgo en {req.Group?.Name} fue aprobada.",
            body?.DecisionNotes,
            $"/groups/{req.Group?.Slug}",
            req.GroupId, "group");

        return Ok(LeadershipRequestService.ToResponse(req));
    }

    [HttpPatch("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] ReviewLeadershipRequest? body)
    {
        var oid = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null || user.Role != "admin") return Forbid();

        var req = await leadershipRequestService.RejectAsync(id, user.Id, body?.DecisionNotes);
        if (req is null) return NotFound();

        await notificationService.CreateAsync(req.RequestedByUserId,
            "leadership_request_rejected",
            $"Tu solicitud de liderazgo en {req.Group?.Name} no fue aceptada.",
            body?.DecisionNotes,
            $"/groups/{req.Group?.Slug}",
            req.GroupId, "group");

        return Ok(LeadershipRequestService.ToResponse(req));
    }
}
