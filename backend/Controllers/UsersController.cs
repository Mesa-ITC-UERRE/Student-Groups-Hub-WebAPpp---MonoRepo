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
    // GET /api/users/me — upserts user from Supabase JWT on first call
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var supabaseId = User.GetSupabaseUserId();
        var email = User.GetEmail();
        var user = await userService.UpsertFromTokenAsync(supabaseId, email);
        return Ok(UserService.ToResponse(user));
    }

    // PATCH /api/users/me
    [HttpPatch("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateUserRequest request)
    {
        var supabaseId = User.GetSupabaseUserId();
        var existing = await userService.GetBySupabaseIdAsync(supabaseId);
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
}
