using System.Security.Claims;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Extracts the Entra ID Object ID (oid claim) from the JWT.
    /// </summary>
    public static string GetEntraOid(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("oid")
            ?? principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
            ?? throw new InvalidOperationException("OID claim not found in token.");
    }

    /// <summary>
    /// Extracts the email from the JWT (preferred_username or email claim).
    /// </summary>
    public static string GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("preferred_username")
            ?? principal.FindFirstValue("email")
            ?? principal.FindFirstValue(ClaimTypes.Email)
            ?? throw new InvalidOperationException("Email claim not found in token.");
    }

    /// <summary>
    /// Extracts the display name from the JWT (name claim).
    /// </summary>
    public static string? GetDisplayName(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("name")
            ?? principal.FindFirstValue(ClaimTypes.Name);
    }
}
