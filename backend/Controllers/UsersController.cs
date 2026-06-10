using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentGroupsHub.DTOs.Requests;
using StudentGroupsHub.Extensions;
using StudentGroupsHub.Services;

namespace StudentGroupsHub.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(UserService userService) : ControllerBase
{
    // GET /api/users/me
    // Creates the user record on first call (upsert by Entra OID).
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var oid = User.GetEntraOid();
        var email = User.GetEmail();
        var displayName = User.GetDisplayName();

        var user = await userService.UpsertFromTokenAsync(oid, email, displayName);
        return Ok(UserService.ToResponse(user));
    }

    // PATCH /api/users/me
    [HttpPatch("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateUserRequest request)
    {
        var oid = User.GetEntraOid();
        var existing = await userService.GetByEntraOidAsync(oid);
        if (existing is null) return NotFound();

        var updated = await userService.UpdateAsync(existing.Id, request.DisplayName, request.AvatarUrl);
        return Ok(UserService.ToResponse(updated!));
    }

    // GET /api/users/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await userService.GetByIdAsync(id);
        if (user is null) return NotFound();
        return Ok(UserService.ToResponse(user));
    }

    // GET /api/users/by-entra/{oid}
    [HttpGet("by-entra/{oid}")]
    public async Task<IActionResult> GetByEntraOid(string oid)
    {
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return NotFound();
        return Ok(UserService.ToResponse(user));
    }
}
