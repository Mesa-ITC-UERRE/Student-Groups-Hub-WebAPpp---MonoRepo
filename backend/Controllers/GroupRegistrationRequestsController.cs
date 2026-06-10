using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentGroupsHub.DTOs.Requests;
using StudentGroupsHub.Extensions;
using StudentGroupsHub.Services;

namespace StudentGroupsHub.Controllers;

[ApiController]
[Route("api/group-registration-requests")]
[Authorize]
public class GroupRegistrationRequestsController(
    GroupRegistrationRequestService requestService,
    UserService userService) : ControllerBase
{
    // POST /api/group-registration-requests
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupRegistrationRequest request)
    {
        var oid = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();

        var req = await requestService.CreateAsync(
            user.Id,
            request.ProposedGroupName,
            request.ProposedDescription,
            request.ContactEmail);

        return CreatedAtAction(nameof(GetById), new { id = req.Id },
            GroupRegistrationRequestService.ToResponse(req));
    }

    // GET /api/group-registration-requests/mine
    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var oid = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();

        var reqs = await requestService.GetByUserAsync(user.Id);
        return Ok(reqs.Select(GroupRegistrationRequestService.ToResponse));
    }

    // GET /api/group-registration-requests — admin only
    [HttpGet]
    public async Task<IActionResult> GetAllPending()
    {
        var oid = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null || user.Role != "admin") return Forbid();

        var reqs = await requestService.GetAllPendingAsync();
        return Ok(reqs.Select(GroupRegistrationRequestService.ToResponse));
    }

    // GET /api/group-registration-requests/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var oid = User.GetEntraOid();
        var currentUser = await userService.GetByEntraOidAsync(oid);
        if (currentUser is null) return Unauthorized();

        var req = await requestService.GetByIdAsync(id);
        if (req is null) return NotFound();

        // Only the requester or an admin can see it
        if (req.RequestedByUserId != currentUser.Id && currentUser.Role != "admin")
            return Forbid();

        return Ok(GroupRegistrationRequestService.ToResponse(req));
    }

    // PATCH /api/group-registration-requests/{id}/approve — admin only
    [HttpPatch("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ReviewDecisionRequest? body)
    {
        var oid = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null || user.Role != "admin") return Forbid();

        var req = await requestService.ApproveAsync(id, user.Id, body?.DecisionNotes);
        if (req is null) return NotFound(new { status = 404, message = "Solicitud no encontrada o ya procesada." });

        return Ok(GroupRegistrationRequestService.ToResponse(req));
    }

    // PATCH /api/group-registration-requests/{id}/reject — admin only
    [HttpPatch("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] ReviewDecisionRequest? body)
    {
        var oid = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null || user.Role != "admin") return Forbid();

        var req = await requestService.RejectAsync(id, user.Id, body?.DecisionNotes);
        if (req is null) return NotFound(new { status = 404, message = "Solicitud no encontrada o ya procesada." });

        return Ok(GroupRegistrationRequestService.ToResponse(req));
    }
}
