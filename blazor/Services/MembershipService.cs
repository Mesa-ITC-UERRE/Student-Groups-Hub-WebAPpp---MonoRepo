using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Responses;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class MembershipService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<Membership?> JoinAsync(Guid userId, Guid groupId)
    {
        using var db = dbFactory.CreateDbContext();

        // Verify the group exists and is active
        var group = await db.Groups.FindAsync(groupId);
        if (group is null || group.Status != "active")
            throw new InvalidOperationException("El grupo no existe o no está activo.");

        var existing = await db.Memberships
            .FirstOrDefaultAsync(m => m.UserId == userId && m.GroupId == groupId);

        if (existing is not null)
        {
            if (existing.Status is "pending" or "accepted")
                throw new InvalidOperationException("Ya tienes una solicitud activa para este grupo.");
            existing.Status      = "pending";
            existing.RequestedAt = DateTime.UtcNow;
            existing.RespondedAt = null;
            await db.SaveChangesAsync();
            return existing;
        }

        var membership = new Membership
        {
            Id          = Guid.NewGuid(),
            UserId      = userId,
            GroupId     = groupId,
            Status      = "pending",
            RequestedAt = DateTime.UtcNow,
        };
        db.Memberships.Add(membership);
        await db.SaveChangesAsync();
        return membership;
    }

    public async Task<Membership?> GetByUserAndGroupAsync(Guid userId, Guid groupId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Memberships
            .FirstOrDefaultAsync(m => m.UserId == userId && m.GroupId == groupId);
    }

    public async Task<List<Membership>> GetAcceptedAsync(Guid groupId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Memberships
            .Include(m => m.User)
            .Where(m => m.GroupId == groupId && m.Status == "accepted")
            .OrderBy(m => m.RespondedAt)
            .ToListAsync();
    }

    public async Task<List<Membership>> GetPendingAsync(Guid groupId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Memberships
            .Include(m => m.User)
            .Where(m => m.GroupId == groupId && m.Status == "pending")
            .OrderBy(m => m.RequestedAt)
            .ToListAsync();
    }

    public async Task<List<Membership>> GetForUserAsync(Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Memberships
            .Include(m => m.Group)
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.RequestedAt)
            .ToListAsync();
    }

    public async Task<Membership?> ApproveAsync(Guid groupId, Guid membershipId, string? notes)
    {
        using var db = dbFactory.CreateDbContext();
        var m = await db.Memberships.Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == membershipId && m.GroupId == groupId);
        if (m is null || m.Status != "pending") return null;
        m.Status = "accepted"; m.Notes = notes; m.RespondedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return m;
    }

    public async Task<Membership?> RejectAsync(Guid groupId, Guid membershipId, string? notes)
    {
        using var db = dbFactory.CreateDbContext();
        var m = await db.Memberships.Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == membershipId && m.GroupId == groupId);
        if (m is null || m.Status != "pending") return null;
        m.Status = "rejected"; m.Notes = notes; m.RespondedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return m;
    }

    public async Task<bool> RemoveAsync(Guid groupId, Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        var m = await db.Memberships.FirstOrDefaultAsync(m =>
            m.GroupId == groupId && m.UserId == userId && m.Status == "accepted");
        if (m is null) return false;
        m.Status = "removed"; m.RespondedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public static MembershipResponse ToResponse(Membership m) => new(
        m.Id, m.UserId, m.GroupId, m.User?.Email ?? "", m.User?.DisplayName,
        m.User?.AvatarUrl, m.Status, m.RequestedAt, m.RespondedAt);
}
