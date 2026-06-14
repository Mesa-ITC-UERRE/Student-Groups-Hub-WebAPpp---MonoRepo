using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentGroupsHub.DTOs.Requests;
using StudentGroupsHub.Extensions;
using StudentGroupsHub.Services;

namespace StudentGroupsHub.Controllers;

[ApiController]
[Route("api/groups")]
public class GroupsController(GroupService groupService, UserService userService) : ControllerBase
{
    // GET /api/groups?search=&category=&page=1&pageSize=20
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, 50);
        var (items, total) = await groupService.GetAllActiveAsync(search, category, page, pageSize);

        // Fetch member counts in parallel
        var responses = await Task.WhenAll(items.Select(async g =>
        {
            var count = await groupService.GetMemberCountAsync(g.Id);
            return GroupService.ToResponse(g, count);
        }));

        return Ok(new
        {
            data = responses,
            page,
            pageSize,
            total,
            totalPages = (int)Math.Ceiling((double)total / pageSize)
        });
    }

    // GET /api/groups/categories
    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await groupService.GetCategoriesAsync();
        return Ok(categories);
    }

    // GET /api/groups/{slug}
    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var group = await groupService.GetBySlugAsync(slug);
        if (group is null) return NotFound(new { status = 404, message = "Grupo no encontrado." });

        var memberCount = await groupService.GetMemberCountAsync(group.Id);
        return Ok(GroupService.ToResponse(group, memberCount));
    }

    // PATCH /api/groups/{id} — group leader or admin only
    [HttpPatch("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGroupRequest request)
    {
        var oid = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();

        // Must be admin OR the leader of this group
        var isAdmin = user.Role == "admin";
        var isLeader = await groupService.IsLeaderOfGroupAsync(user.Id, id);
        if (!isAdmin && !isLeader) return Forbid();

        var updated = await groupService.UpdateAsync(
            id, request.Name, request.Description, request.Category,
            request.LogoUrl, request.BannerUrl, request.ContactEmail, request.ContactInfo);

        if (updated is null) return NotFound();
        var memberCount = await groupService.GetMemberCountAsync(id);
        return Ok(GroupService.ToResponse(updated, memberCount));
    }

    // PATCH /api/groups/{id}/status — admin only
    [HttpPatch("{id:guid}/status")]
    [Authorize]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetGroupStatusRequest request)
    {
        var oid = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null || user.Role != "admin") return Forbid();

        var ok = await groupService.SetStatusAsync(id, request.Status);
        if (!ok) return NotFound();
        return NoContent();
    }
}

public record SetGroupStatusRequest(string Status);
