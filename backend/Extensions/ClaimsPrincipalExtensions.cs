using System.Security.Claims;

namespace StudentGroupsHub.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Returns the Entra ID Object ID from the 'oid' claim.</summary>
    public static string GetEntraOid(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("oid")
            ?? principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
            ?? throw new InvalidOperationException("OID claim not found in token.");
    }

    /// <summary>Returns the email from preferred_username or email claim.</summary>
    public static string GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("preferred_username")
            ?? principal.FindFirstValue("email")
            ?? principal.FindFirstValue(ClaimTypes.Email)
            ?? throw new InvalidOperationException("Email claim not found in token.");
    }

    /// <summary>Returns the display name from the name claim.</summary>
    public static string? GetDisplayName(this ClaimsPrincipal principal)
        => principal.FindFirstValue("name") ?? principal.FindFirstValue(ClaimTypes.Name);
}
