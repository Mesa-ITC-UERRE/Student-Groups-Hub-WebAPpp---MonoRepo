using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Requests;
using StudentGroupsHub.Extensions;
using StudentGroupsHub.Models;
using StudentGroupsHub.Services;

namespace StudentGroupsHub.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class AdminController(
    IDbContextFactory<AppDbContext> dbFactory,
    UserService userService,
    DashboardService dashboardService,
    GroupSeasonService groupSeasonService,
    NotificationService notificationService) : ControllerBase
{
    private async Task<bool> IsAdminAsync()
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        return UserService.IsActive(user) && user!.Role == "admin";
    }

    private async Task<User?> GetCurrentUserAsync()
        => await userService.GetByEntraOidAsync(User.GetEntraOid());

    // GET /api/admin/users?search=&role=&status=&page=1&pageSize=20
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? search, [FromQuery] string? role,
        [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (!await IsAdminAsync()) return Forbid();

        using var db = dbFactory.CreateDbContext();
        var q = db.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(u => EF.Functions.ILike(u.Email, $"%{search}%")
                          || EF.Functions.ILike(u.DisplayName ?? "", $"%{search}%"));
        if (!string.IsNullOrWhiteSpace(role))   q = q.Where(u => u.Role == role);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(u => u.Status == status);

        var total = await q.CountAsync();
        var items = await q.OrderBy(u => u.Email)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Ok(new { data = items.Select(UserService.ToResponse), page, pageSize, total });
    }

    // PATCH /api/admin/users/{id}/role
    [HttpPatch("users/{id:guid}/role")]
    public async Task<IActionResult> SetRole(Guid id, [FromBody] SetRoleRequest request)
    {
        if (!await IsAdminAsync()) return Forbid();
        var actingUser = await GetCurrentUserAsync();
        if (!UserService.IsActive(actingUser)) return Forbid();

        try
        {
            var user = await userService.SetRoleAsync(
                actingUser!.Id, id, request.Role, request.GroupId);
            return user is null ? NotFound() : Ok(UserService.ToResponse(user));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PATCH /api/admin/users/{id}/status
    [HttpPatch("users/{id:guid}/status")]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetUserStatusRequest request)
    {
        if (!await IsAdminAsync()) return Forbid();
        var actingUser = await GetCurrentUserAsync();
        if (!UserService.IsActive(actingUser)) return Forbid();
        try
        {
            var user = await userService.SetStatusAsync(actingUser!.Id, id, request.Status);
            return user is null ? NotFound() : Ok(UserService.ToResponse(user));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET /api/admin/metrics
    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics()
    {
        if (!await IsAdminAsync()) return Forbid();
        var data = await dashboardService.GetAdminDashboardAsync();
        return Ok(data);
    }

    // POST /api/admin/groups/reset-season
    [HttpPost("groups/reset-season")]
    public async Task<IActionResult> ResetSeason([FromBody] ResetSeasonRequest? request)
    {
        if (!await IsAdminAsync()) return Forbid();
        var actingUser = await GetCurrentUserAsync();
        if (!UserService.IsActive(actingUser)) return Forbid();

        using var db = dbFactory.CreateDbContext();
        var skipIds = (request?.SkipGroupIds ?? []).Distinct().ToHashSet();
        var targetIds = await db.Groups
            .Where(g => g.Status == "active" && !skipIds.Contains(g.Id))
            .Select(g => g.Id)
            .ToListAsync();

        var affectedLeaders = await db.RoleAssignments
            .Include(r => r.Group)
            .Where(r => targetIds.Contains(r.GroupId) && r.PermissionRole == "leader")
            .ToListAsync();

        var affectedMemberships = await db.Memberships
            .Include(m => m.Group)
            .Where(m => targetIds.Contains(m.GroupId) && (m.Status == "accepted" || m.Status == "pending"))
            .ToListAsync();

        await groupSeasonService.ResetSeasonAsync(targetIds);

        foreach (var leader in affectedLeaders)
        {
            await notificationService.CreateAsync(leader.UserId,
                "group_season_reset",
                $"Tu periodo como líder de {leader.Group?.Name} ha concluido.",
                "El grupo fue reiniciado para el nuevo ciclo y requerirá nuevas solicitudes de liderazgo.",
                $"/groups/{leader.Group?.Slug}",
                leader.GroupId, "group");
        }

        foreach (var membership in affectedMemberships)
        {
            await notificationService.CreateAsync(membership.UserId,
                "group_membership_reset",
                $"{membership.Group?.Name} reinició su ciclo.",
                membership.Status == "accepted"
                    ? "Tu membresía fue cerrada y debes volver a unirte en el nuevo ciclo."
                    : "Tu solicitud fue cerrada por el reinicio anual del grupo.",
                $"/groups/{membership.Group?.Slug}",
                membership.GroupId, "group");
        }

        return NoContent();
    }

    // POST /api/admin/groups/{id}/reset-season
    [HttpPost("groups/{id:guid}/reset-season")]
    public async Task<IActionResult> ResetGroupSeason(Guid id)
    {
        if (!await IsAdminAsync()) return Forbid();
        var actingUser = await GetCurrentUserAsync();
        if (!UserService.IsActive(actingUser)) return Forbid();

        using var db = dbFactory.CreateDbContext();
        var affectedLeaders = await db.RoleAssignments
            .Include(r => r.Group)
            .Where(r => r.GroupId == id && r.PermissionRole == "leader")
            .ToListAsync();

        var affectedMemberships = await db.Memberships
            .Include(m => m.Group)
            .Where(m => m.GroupId == id && (m.Status == "accepted" || m.Status == "pending"))
            .ToListAsync();

        await groupSeasonService.ResetGroupAsync(id);

        foreach (var leader in affectedLeaders)
        {
            await notificationService.CreateAsync(leader.UserId,
                "group_season_reset",
                $"Tu periodo como líder de {leader.Group?.Name} ha concluido.",
                "El grupo fue reiniciado para el nuevo ciclo y requerirá nuevas solicitudes de liderazgo.",
                $"/groups/{leader.Group?.Slug}",
                leader.GroupId, "group");
        }

        foreach (var membership in affectedMemberships)
        {
            await notificationService.CreateAsync(membership.UserId,
                "group_membership_reset",
                $"{membership.Group?.Name} reinició su ciclo.",
                membership.Status == "accepted"
                    ? "Tu membresía fue cerrada y debes volver a unirte en el nuevo ciclo."
                    : "Tu solicitud fue cerrada por el reinicio anual del grupo.",
                $"/groups/{membership.Group?.Slug}",
                membership.GroupId, "group");
        }

        return NoContent();
    }
}

public record SetRoleRequest(string Role, Guid? GroupId = null);
public record SetUserStatusRequest(string Status);
public record ResetSeasonRequest(List<Guid> SkipGroupIds);
