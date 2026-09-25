using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Responses;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class UserService(IDbContextFactory<AppDbContext> dbFactory)
{
    public const string InactiveActionMessage = "Tu cuenta está inactiva. Contacta a un administrador para recuperar el acceso.";

    public async Task<User> UpsertFromTokenAsync(string entraOid, string email, string? displayName)
    {
        using var db = dbFactory.CreateDbContext();
        var user = await db.Users.FirstOrDefaultAsync(u => u.EntraOid == entraOid);
        if (user is null)
        {
            user = new User
            {
                Id          = Guid.NewGuid(),
                EntraOid    = entraOid,
                Email       = email,
                DisplayName = displayName,
                Role        = "student",
                Status      = "active",
                CreatedAt   = DateTime.UtcNow,
                UpdatedAt   = DateTime.UtcNow,
            };
            db.Users.Add(user);
        }
        else
        {
            var resolvedName = displayName ?? user.DisplayName;
            if (user.DisplayName != resolvedName || user.Email != email)
            {
                user.DisplayName = resolvedName;
                user.Email       = email;
                user.UpdatedAt   = DateTime.UtcNow;
            }
        }
        await db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Users.FindAsync(id);
    }

    public async Task<User?> GetByEntraOidAsync(string entraOid)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Users.FirstOrDefaultAsync(u => u.EntraOid == entraOid);
    }

    public async Task<User?> UpdateAsync(Guid id, string? displayName, string? avatarUrl)
    {
        using var db = dbFactory.CreateDbContext();
        var user = await db.Users.FindAsync(id);
        if (user is null) return null;
        if (displayName is not null) user.DisplayName = displayName;
        if (avatarUrl   is not null) user.AvatarUrl   = avatarUrl;
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> AssignLeaderAsync(Guid userId, Guid groupId)
    {
        using var db = dbFactory.CreateDbContext();

        var user = await db.Users.FindAsync(userId);
        if (user is null) return null;

        var group = await db.Groups.FindAsync(groupId);
        if (group is null || group.Status != "active")
            throw new InvalidOperationException("El grupo seleccionado no está disponible.");

        var alreadyLeaderOfGroup = await db.RoleAssignments.AnyAsync(r =>
            r.GroupId == groupId && r.UserId == userId && r.PermissionRole == "leader");
        if (alreadyLeaderOfGroup)
            throw new InvalidOperationException("La persona seleccionada ya lidera ese grupo.");

        var groupHasAnotherLeader = await db.RoleAssignments.AnyAsync(r =>
            r.GroupId == groupId && r.PermissionRole == "leader");
        if (groupHasAnotherLeader)
            throw new InvalidOperationException("Este grupo ya tiene un liderazgo activo. Quita al líder actual antes de asignar otro.");

        db.RoleAssignments.Add(new RoleAssignment
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            UserId = userId,
            PermissionRole = "leader",
            DisplayRole = "Líder",
            CreatedAt = DateTime.UtcNow,
        });

        var membership = await db.Memberships.FirstOrDefaultAsync(m =>
            m.GroupId == groupId && m.UserId == userId);
        if (membership is null)
        {
            db.Memberships.Add(new Membership
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                UserId = userId,
                Status = "accepted",
                RequestedAt = DateTime.UtcNow,
                RespondedAt = DateTime.UtcNow,
            });
        }
        else
        {
            membership.Status = "accepted";
            membership.RespondedAt = DateTime.UtcNow;
            membership.Notes = null;
        }

        if (user.Role != "admin")
            user.Role = "group_leader";

        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> DemoteToStudentAsync(Guid userId)
    {
        using var db = dbFactory.CreateDbContext();

        var user = await db.Users.FindAsync(userId);
        if (user is null) return null;

        var leaderAssignments = await db.RoleAssignments
            .Where(r => r.UserId == userId && r.PermissionRole == "leader")
            .ToListAsync();

        if (leaderAssignments.Count > 0)
            db.RoleAssignments.RemoveRange(leaderAssignments);

        user.Role = "student";
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return user;
    }

    public static bool IsActive(User? user)
        => string.Equals(user?.Status, "active", StringComparison.OrdinalIgnoreCase);

    public static void EnsureCanAct(User? user)
    {
        if (user is null)
            throw new InvalidOperationException("No autenticado.");
        if (!IsActive(user))
            throw new InvalidOperationException(InactiveActionMessage);
    }

    public static UserResponse ToResponse(User u) => new(
        u.Id, u.EntraOid, u.Email, u.DisplayName, u.AvatarUrl,
        u.Role, u.Status, IsPlatformAdmin: u.Role == "admin",
        u.CreatedAt, u.UpdatedAt);
}
