using Microsoft.AspNetCore.Authorization;
using StudentGroupsHub.Extensions;

namespace StudentGroupsHub.Services;

public class AdminRoleRequirement : IAuthorizationRequirement { }

public class AdminRoleHandler(IServiceScopeFactory scopeFactory)
    : AuthorizationHandler<AdminRoleRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminRoleRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return;

        string? entraOid;
        try { entraOid = context.User.GetEntraOid(); }
        catch { return; }

        if (string.IsNullOrEmpty(entraOid)) return;

        // Resolve scoped services inside a new scope (handler is singleton)
        using var scope = scopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<UserService>();

        var user = await userService.GetByEntraOidAsync(entraOid);
        if (user?.Role == "admin")
            context.Succeed(requirement);
    }
}
