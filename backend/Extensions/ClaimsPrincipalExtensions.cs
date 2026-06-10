using System.Security.Claims;

namespace StudentGroupsHub.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Returns the Supabase user UUID from the 'sub' claim.
    /// </summary>
    public static string GetSupabaseUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue("sub")
            ?? throw new InvalidOperationException("Sub claim not found in token.");
    }

    /// <summary>
    /// Returns the email from the token claims.
    /// </summary>
    public static string GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("email")
            ?? principal.FindFirstValue(ClaimTypes.Email)
            ?? throw new InvalidOperationException("Email claim not found in token.");
    }
}
