using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using StudentGroupsHub.Extensions;

namespace StudentGroupsHub.Services;

/// <summary>
/// Provides the current user's identity to Blazor components.
/// Caches the immutable authentication principal for the Blazor circuit, while
/// resolving the local user on every call so status and role changes take effect
/// immediately.
/// </summary>
public class CurrentUserService(AuthenticationStateProvider authStateProvider, UserService userService)
{
    private ClaimsPrincipal? _cachedPrincipal;
    private bool _authChecked;

    private async Task<ClaimsPrincipal?> GetPrincipalAsync()
    {
        // Only call GetAuthenticationStateAsync ONCE per circuit lifetime
        if (_authChecked) return _cachedPrincipal;
        _authChecked = true;
        try
        {
            var state = await authStateProvider.GetAuthenticationStateAsync();
            _cachedPrincipal = state.User.Identity?.IsAuthenticated == true
                ? state.User : null;
        }
        catch
        {
            _cachedPrincipal = null;
        }
        return _cachedPrincipal;
    }

    public async Task<StudentGroupsHub.Models.User?> GetUserAsync()
    {
        var principal = await GetPrincipalAsync();
        if (principal is null) return null;

        try
        {
            var oid   = principal.GetEntraOid();
            var email = principal.GetEmail();
            var name  = principal.GetDisplayName();
            return await userService.UpsertFromTokenAsync(oid, email, name);
        }
        catch { return null; }
    }

    public async Task<Guid> GetUserIdAsync()
    {
        var user = await GetUserAsync();
        return user?.Id ?? Guid.Empty;
    }

    public async Task<StudentGroupsHub.Models.User> RequireActiveUserAsync()
    {
        var user = await GetUserAsync();
        UserService.EnsureCanAct(user);
        return user!;
    }

    public async Task<Guid> GetActiveUserIdAsync()
        => (await RequireActiveUserAsync()).Id;

    public async Task<bool> IsAuthenticatedAsync()
    {
        var principal = await GetPrincipalAsync();
        return principal is not null;
    }

    public void Invalidate()
    {
        _cachedPrincipal = null;
        _authChecked     = false;
    }
}
