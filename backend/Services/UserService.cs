using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Responses;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class UserService(AppDbContext db)
{
    /// <summary>
    /// Returns the user matching the Entra OID, creating one on first login (upsert).
    /// Display name is synced from the JWT on every login.
    /// </summary>
    public async Task<User> UpsertFromTokenAsync(string entraOid, string email, string? displayName)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.EntraOid == entraOid);

        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                EntraOid = entraOid,
                Email = email,
                DisplayName = displayName,
                Role = "student",
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            db.Users.Add(user);
        }
        else
        {
            // Sync display name from Entra on every call (user may have changed it in M365)
            user.DisplayName = displayName ?? user.DisplayName;
            user.Email = email;
            user.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByIdAsync(Guid id)
        => await db.Users.FindAsync(id);

    public async Task<User?> GetByEntraOidAsync(string entraOid)
        => await db.Users.FirstOrDefaultAsync(u => u.EntraOid == entraOid);

    public async Task<User?> UpdateAsync(Guid id, string? displayName, string? avatarUrl)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return null;

        if (displayName is not null) user.DisplayName = displayName;
        if (avatarUrl is not null) user.AvatarUrl = avatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return user;
    }

    public static UserResponse ToResponse(User u) => new(
        u.Id, u.EntraOid, u.Email, u.DisplayName, u.AvatarUrl,
        u.Role, u.Status,
        IsPlatformAdmin: u.Role == "admin",
        u.CreatedAt, u.UpdatedAt
    );
}
