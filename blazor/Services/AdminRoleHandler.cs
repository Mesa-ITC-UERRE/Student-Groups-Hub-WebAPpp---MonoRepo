using Microsoft.AspNetCore.Authorization;
using StudentGroupsHub.Extensions;

namespace StudentGroupsHub.Services;

public class AdminRoleRequirement : IAuthorizationRequirement { }

public class ActiveUserRequirement : IAuthorizationRequirement { }

public class ActiveUserHandler(IServiceScopeFactory scopeFactory)
    : AuthorizationHandler<ActiveUserRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ActiveUserRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true) return;

        try
        {
            using var scope = scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<UserService>();
            var user = await userService.UpsertFromTokenAsync(
                context.User.GetEntraOid(),
                context.User.GetEmail(),
                context.User.GetDisplayName());
            if (UserService.IsActive(user))
                context.Succeed(requirement);
        }
        catch
        {
            // A missing/invalid identity or unavailable local user store denies access.
        }
    }
}

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
        if (UserService.IsActive(user) && user!.Role == "admin")
            context.Succeed(requirement);
    }
}
