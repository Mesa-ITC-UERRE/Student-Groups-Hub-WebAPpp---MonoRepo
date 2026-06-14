using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentGroupsHub.Extensions;
using StudentGroupsHub.Services;

namespace StudentGroupsHub.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController(
    DashboardService dashboardService,
    UserService userService) : ControllerBase
{
    // GET /api/dashboard/student
    [HttpGet("student")]
    public async Task<IActionResult> Student()
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();
        var data = await dashboardService.GetStudentDashboardAsync(user.Id);
        return Ok(data);
    }

    // GET /api/dashboard/leader
    [HttpGet("leader")]
    public async Task<IActionResult> Leader()
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null) return Unauthorized();
        var data = await dashboardService.GetLeaderDashboardAsync(user.Id);
        return Ok(data);
    }

    // GET /api/dashboard/admin
    [HttpGet("admin")]
    public async Task<IActionResult> Admin()
    {
        var oid  = User.GetEntraOid();
        var user = await userService.GetByEntraOidAsync(oid);
        if (user is null || user.Role != "admin") return Forbid();
        var data = await dashboardService.GetAdminDashboardAsync();
        return Ok(data);
    }
}
