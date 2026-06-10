using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Responses;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class UserService(AppDbContext db)
{
    /// <summary>
    /// Returns the user matching the Supabase UUID (sub claim), creating one on first call.
    /// </summary>
    public async Task<User> UpsertFromTokenAsync(string supabaseId, string email)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.SupabaseId == supabaseId);

        if (user is null)
        {
            // Derive a display name from the email prefix on first login
            var displayName = email.Split('@')[0].Replace('.', ' ').Replace('_', ' ');
            user = new User
            {
                Id = Guid.NewGuid(),
                SupabaseId = supabaseId,
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
            user.Email = email;
            user.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByIdAsync(Guid id)
        => await db.Users.FindAsync(id);

    public async Task<User?> GetBySupabaseIdAsync(string supabaseId)
        => await db.Users.FirstOrDefaultAsync(u => u.SupabaseId == supabaseId);

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
        u.Id, u.SupabaseId, u.Email, u.DisplayName, u.AvatarUrl,
        u.Role, u.Status,
        IsPlatformAdmin: u.Role == "admin",
        u.CreatedAt, u.UpdatedAt
    );
}
