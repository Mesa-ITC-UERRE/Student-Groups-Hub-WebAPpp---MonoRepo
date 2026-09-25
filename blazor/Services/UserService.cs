using System.Data;
using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Responses;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class UserService(IDbContextFactory<AppDbContext> dbFactory)
{
    private static readonly HashSet<string> AllowedRoles = ["student", "group_leader", "admin"];
    private static readonly HashSet<string> AllowedStatuses = ["active", "inactive"];

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

    public async Task<User?> SetRoleAsync(
        Guid actingUserId,
        Guid userId,
        string role,
        Guid? groupId = null)
    {
        if (!AllowedRoles.Contains(role))
            throw new UserVisibleException("El rol seleccionado no es válido.");
        if (role == "group_leader" && !groupId.HasValue)
            throw new UserVisibleException("Debes seleccionar un grupo para asignar liderazgo.");

        await using var db = await dbFactory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        await EnsureActiveAdminAsync(db, actingUserId);

        var user = await db.Users.FindAsync(userId);
        if (user is null) return null;

        if (actingUserId == userId && role != "admin")
            throw new UserVisibleException("No puedes quitarte tu propio rol de administrador.");

        if (user.Role == "admin" && role != "admin")
            await EnsureAnotherActiveAdminAsync(db, userId);

        switch (role)
        {
            case "group_leader":
                await AssignLeaderAsync(db, user, groupId!.Value);
                break;
            case "student":
                await DemoteToStudentAsync(db, user);
                break;
            case "admin":
                user.Role = "admin";
                user.UpdatedAt = DateTime.UtcNow;
                break;
        }

        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return user;
    }

    public async Task<User?> SetStatusAsync(Guid actingUserId, Guid userId, string status)
    {
        if (!AllowedStatuses.Contains(status))
            throw new UserVisibleException("El estado seleccionado no es válido.");

        await using var db = await dbFactory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        await EnsureActiveAdminAsync(db, actingUserId);
        var user = await db.Users.FindAsync(userId);
        if (user is null) return null;

        if (actingUserId == userId && status == "inactive")
            throw new UserVisibleException("No puedes desactivar tu propia cuenta.");

        if (user.Role == "admin" && user.Status == "active" && status == "inactive")
            await EnsureAnotherActiveAdminAsync(db, userId);

        if (status == "inactive")
        {
            var leaderAssignments = await db.RoleAssignments
                .Where(r => r.UserId == userId && r.PermissionRole == "leader")
                .ToListAsync();
            db.RoleAssignments.RemoveRange(leaderAssignments);
            if (user.Role == "group_leader")
                user.Role = "student";
        }

        user.Status = status;
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return user;
    }

    private static async Task AssignLeaderAsync(AppDbContext db, User user, Guid groupId)
    {
        var group = await db.Groups.FindAsync(groupId);
        if (group is null || group.Status != "active")
            throw new UserVisibleException("El grupo seleccionado no está disponible.");

        var alreadyLeaderOfGroup = await db.RoleAssignments.AnyAsync(r =>
            r.GroupId == groupId && r.UserId == user.Id && r.PermissionRole == "leader");
        if (alreadyLeaderOfGroup)
            throw new UserVisibleException("La persona seleccionada ya lidera ese grupo.");

        var groupHasAnotherLeader = await db.RoleAssignments.AnyAsync(r =>
            r.GroupId == groupId && r.PermissionRole == "leader");
        if (groupHasAnotherLeader)
            throw new UserVisibleException("Este grupo ya tiene un liderazgo activo. Quita al líder actual antes de asignar otro.");

        db.RoleAssignments.Add(new RoleAssignment
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            UserId = user.Id,
            PermissionRole = "leader",
            DisplayRole = "Líder",
            CreatedAt = DateTime.UtcNow,
        });

        var membership = await db.Memberships.FirstOrDefaultAsync(m =>
            m.GroupId == groupId && m.UserId == user.Id);
        if (membership is null)
        {
            db.Memberships.Add(new Membership
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                UserId = user.Id,
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
    }

    private static async Task DemoteToStudentAsync(AppDbContext db, User user)
    {
        var leaderAssignments = await db.RoleAssignments
            .Where(r => r.UserId == user.Id && r.PermissionRole == "leader")
            .ToListAsync();

        if (leaderAssignments.Count > 0)
            db.RoleAssignments.RemoveRange(leaderAssignments);

        user.Role = "student";
        user.UpdatedAt = DateTime.UtcNow;
    }

    private static async Task EnsureActiveAdminAsync(AppDbContext db, Guid userId)
    {
        var canAdminister = await db.Users.AnyAsync(u =>
            u.Id == userId && u.Status == "active" && u.Role == "admin");
        if (!canAdminister)
            throw new UserVisibleException("No tienes permiso para administrar usuarios.");
    }

    private static async Task EnsureAnotherActiveAdminAsync(AppDbContext db, Guid excludedUserId)
    {
        var anotherAdminExists = await db.Users.AnyAsync(u =>
            u.Id != excludedUserId && u.Status == "active" && u.Role == "admin");
        if (!anotherAdminExists)
            throw new UserVisibleException("Debe permanecer al menos un administrador activo.");
    }

    public static bool IsActive(User? user)
        => string.Equals(user?.Status, "active", StringComparison.OrdinalIgnoreCase);

    public static void EnsureCanAct(User? user)
    {
        if (user is null)
            throw new UserVisibleException("No autenticado.");
        if (!IsActive(user))
            throw new UserVisibleException(InactiveActionMessage);
    }

    public static UserResponse ToResponse(User u) => new(
        u.Id, u.EntraOid, u.Email, u.DisplayName, u.AvatarUrl,
        u.Role, u.Status, IsPlatformAdmin: u.Role == "admin",
        u.CreatedAt, u.UpdatedAt);
}
