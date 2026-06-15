using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Requests;
using StudentGroupsHub.Extensions;
using StudentGroupsHub.Services;

namespace StudentGroupsHub.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class AdminController(
    IDbContextFactory<AppDbContext> dbFactory,
    UserService userService,
    DashboardService dashboardService) : ControllerBase
{
    private async Task<bool> IsAdminAsync()
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        return user?.Role == "admin";
    }

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
        using var db = dbFactory.CreateDbContext();
        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound();
        user.Role = request.Role; user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok(UserService.ToResponse(user));
    }

    // PATCH /api/admin/users/{id}/status
    [HttpPatch("users/{id:guid}/status")]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetUserStatusRequest request)
    {
        if (!await IsAdminAsync()) return Forbid();
        using var db = dbFactory.CreateDbContext();
        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound();
        user.Status = request.Status; user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok(UserService.ToResponse(user));
    }

    // GET /api/admin/metrics
    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics()
    {
        if (!await IsAdminAsync()) return Forbid();
        var data = await dashboardService.GetAdminDashboardAsync();
        return Ok(data);
    }
}

public record SetRoleRequest(string Role);
public record SetUserStatusRequest(string Status);
