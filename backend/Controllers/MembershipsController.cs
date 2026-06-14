using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentGroupsHub.DTOs.Requests;
using StudentGroupsHub.Extensions;
using StudentGroupsHub.Services;

namespace StudentGroupsHub.Controllers;

[ApiController]
[Route("api/groups/{groupId:guid}/memberships")]
[Authorize]
public class MembershipsController(
    MembershipService membershipService,
    GroupService groupService,
    UserService userService,
    NotificationService notificationService) : ControllerBase
{
    // POST /api/groups/{groupId}/join
    [HttpPost("/api/groups/{groupId:guid}/join")]
    public async Task<IActionResult> Join(Guid groupId)
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();

        var membership = await membershipService.JoinAsync(user.Id, groupId);

        // Notify group leaders
        var leaders = await groupService.GetLeaderIdsAsync(groupId);
        var group   = await groupService.GetByIdAsync(groupId);
        foreach (var leaderId in leaders)
            await notificationService.CreateAsync(leaderId,
                "new_membership_request",
                $"Nueva solicitud de membresía en {group?.Name}",
                $"{user.DisplayName ?? user.Email} quiere unirse al grupo.",
                $"/leader/groups/{groupId}/members",
                user.Id, "membership");

        return CreatedAtAction(nameof(Join), MembershipService.ToResponse(membership!));
    }

    // GET /api/groups/{groupId}/members
    [HttpGet("/api/groups/{groupId:guid}/members")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMembers(Guid groupId)
    {
        var members = await membershipService.GetAcceptedAsync(groupId);
        return Ok(members.Select(MembershipService.ToResponse));
    }

    // GET /api/groups/{groupId}/memberships/pending
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(Guid groupId)
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();

        var isAdmin  = user.Role == "admin";
        var isLeader = await groupService.IsLeaderOfGroupAsync(user.Id, groupId);
        if (!isAdmin && !isLeader) return Forbid();

        var pending = await membershipService.GetPendingAsync(groupId);
        return Ok(pending.Select(MembershipService.ToResponse));
    }

    // PATCH /api/groups/{groupId}/memberships/{membershipId}/approve
    [HttpPatch("{membershipId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid groupId, Guid membershipId,
        [FromBody] ReviewMembershipRequest? body)
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();

        var isAdmin  = user.Role == "admin";
        var isLeader = await groupService.IsLeaderOfGroupAsync(user.Id, groupId);
        if (!isAdmin && !isLeader) return Forbid();

        var m = await membershipService.ApproveAsync(membershipId, body?.Notes);
        if (m is null) return NotFound();

        var group = await groupService.GetByIdAsync(groupId);
        await notificationService.CreateAsync(m.UserId,
            "membership_approved",
            $"¡Tu solicitud fue aceptada en {group?.Name}!",
            body?.Notes,
            $"/groups/{group?.Slug}",
            groupId, "group");

        return Ok(MembershipService.ToResponse(m));
    }

    // PATCH /api/groups/{groupId}/memberships/{membershipId}/reject
    [HttpPatch("{membershipId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid groupId, Guid membershipId,
        [FromBody] ReviewMembershipRequest? body)
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();

        var isAdmin  = user.Role == "admin";
        var isLeader = await groupService.IsLeaderOfGroupAsync(user.Id, groupId);
        if (!isAdmin && !isLeader) return Forbid();

        var m = await membershipService.RejectAsync(membershipId, body?.Notes);
        if (m is null) return NotFound();

        var group = await groupService.GetByIdAsync(groupId);
        await notificationService.CreateAsync(m.UserId,
            "membership_rejected",
            $"Tu solicitud en {group?.Name} no fue aceptada.",
            body?.Notes,
            "/groups",
            groupId, "group");

        return Ok(MembershipService.ToResponse(m));
    }

    // DELETE /api/groups/{groupId}/members/{userId}
    [HttpDelete("/api/groups/{groupId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> Remove(Guid groupId, Guid userId)
    {
        var oid     = User.GetEntraOid();
        var current = await userService.GetByEntraOidAsync(oid);
        if (current is null) return Unauthorized();

        var isAdmin  = current.Role == "admin";
        var isLeader = await groupService.IsLeaderOfGroupAsync(current.Id, groupId);
        if (!isAdmin && !isLeader) return Forbid();

        var ok = await membershipService.RemoveAsync(groupId, userId);
        return ok ? NoContent() : NotFound();
    }
}
