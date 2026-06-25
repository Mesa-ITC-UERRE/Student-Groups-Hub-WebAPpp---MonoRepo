using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Responses;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class LeadershipRequestService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<LeadershipRequest> CreateAsync(
        Guid groupId,
        Guid requestedByUserId,
        string contactEmail,
        string? reason)
    {
        using var db = dbFactory.CreateDbContext();

        var group = await db.Groups.FindAsync(groupId);
        if (group is null || group.Status != "active")
            throw new InvalidOperationException("El grupo no existe o no está activo.");

        var alreadyLeader = await db.RoleAssignments.AnyAsync(r =>
            r.GroupId == groupId && r.UserId == requestedByUserId && r.PermissionRole == "leader");
        if (alreadyLeader)
            throw new InvalidOperationException("Ya eres líder de este grupo.");

        var groupHasLeader = await db.RoleAssignments.AnyAsync(r =>
            r.GroupId == groupId && r.PermissionRole == "leader");
        if (groupHasLeader)
            throw new InvalidOperationException("Este grupo ya tiene un liderazgo activo.");

        var existing = await db.LeadershipRequests.FirstOrDefaultAsync(r =>
            r.GroupId == groupId && r.RequestedByUserId == requestedByUserId && r.Status == "pending");
        if (existing is not null)
            throw new InvalidOperationException("Ya tienes una solicitud pendiente para este grupo.");

        var req = new LeadershipRequest
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            RequestedByUserId = requestedByUserId,
            ContactEmail = contactEmail,
            Reason = reason,
            Status = "pending",
            CreatedAt = DateTime.UtcNow,
        };
        db.LeadershipRequests.Add(req);
        await db.SaveChangesAsync();
        return await GetByIdAsync(req.Id) ?? req;
    }

    public async Task<List<LeadershipRequest>> GetByUserAsync(Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.LeadershipRequests
            .Include(r => r.Group)
            .Include(r => r.RequestedBy)
            .Where(r => r.RequestedByUserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<LeadershipRequest>> GetAllPendingAsync()
    {
        using var db = dbFactory.CreateDbContext();
        return await db.LeadershipRequests
            .Include(r => r.Group)
            .Include(r => r.RequestedBy)
            .Where(r => r.Status == "pending")
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<LeadershipRequest?> GetByIdAsync(Guid id)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.LeadershipRequests
            .Include(r => r.Group)
            .Include(r => r.RequestedBy)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<bool> CanRequestAsync(Guid groupId, Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        var group = await db.Groups.FindAsync(groupId);
        if (group is null || group.Status != "active") return false;

        var isLeader = await db.RoleAssignments.AnyAsync(r =>
            r.GroupId == groupId && r.UserId == userId && r.PermissionRole == "leader");
        if (isLeader) return false;

        var groupHasLeader = await db.RoleAssignments.AnyAsync(r =>
            r.GroupId == groupId && r.PermissionRole == "leader");
        if (groupHasLeader) return false;

        return !await db.LeadershipRequests.AnyAsync(r =>
            r.GroupId == groupId && r.RequestedByUserId == userId && r.Status == "pending");
    }

    public async Task<LeadershipRequest?> GetPendingForGroupAndUserAsync(Guid groupId, Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.LeadershipRequests
            .Include(r => r.Group)
            .Include(r => r.RequestedBy)
            .FirstOrDefaultAsync(r => r.GroupId == groupId
                && r.RequestedByUserId == userId
                && r.Status == "pending");
    }

    public async Task<LeadershipRequest?> ApproveAsync(
        Guid requestId,
        Guid reviewedByUserId,
        string? decisionNotes)
    {
        using var db = dbFactory.CreateDbContext();

        var req = await db.LeadershipRequests
            .Include(r => r.Group)
            .Include(r => r.RequestedBy)
            .FirstOrDefaultAsync(r => r.Id == requestId);
        if (req is null || req.Status != "pending") return null;

        var group = req.Group;
        if (group is null || group.Status != "active")
            throw new InvalidOperationException("El grupo no está disponible para asignar liderazgo.");

        var groupHasLeader = await db.RoleAssignments.AnyAsync(r =>
            r.GroupId == req.GroupId && r.PermissionRole == "leader");
        if (groupHasLeader)
            throw new InvalidOperationException("Este grupo ya tiene un liderazgo activo.");

        var hasLeader = await db.RoleAssignments.AnyAsync(r =>
            r.GroupId == req.GroupId && r.UserId == req.RequestedByUserId && r.PermissionRole == "leader");
        if (!hasLeader)
        {
            db.RoleAssignments.Add(new RoleAssignment
            {
                Id = Guid.NewGuid(),
                GroupId = req.GroupId,
                UserId = req.RequestedByUserId,
                PermissionRole = "leader",
                DisplayRole = "Líder",
                CreatedAt = DateTime.UtcNow,
            });
        }

        var user = await db.Users.FindAsync(req.RequestedByUserId);
        if (user is not null && user.Role != "admin")
        {
            user.Role = "group_leader";
            user.UpdatedAt = DateTime.UtcNow;
        }

        var membership = await db.Memberships.FirstOrDefaultAsync(m =>
            m.GroupId == req.GroupId && m.UserId == req.RequestedByUserId);
        if (membership is null)
        {
            db.Memberships.Add(new Membership
            {
                Id = Guid.NewGuid(),
                GroupId = req.GroupId,
                UserId = req.RequestedByUserId,
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

        req.Status = "approved";
        req.DecisionNotes = decisionNotes;
        req.ReviewedByUserId = reviewedByUserId;
        req.ReviewedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return req;
    }

    public async Task<LeadershipRequest?> RejectAsync(
        Guid requestId,
        Guid reviewedByUserId,
        string? decisionNotes)
    {
        using var db = dbFactory.CreateDbContext();
        var req = await db.LeadershipRequests
            .Include(r => r.Group)
            .Include(r => r.RequestedBy)
            .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == "pending");
        if (req is null) return null;

        req.Status = "rejected";
        req.DecisionNotes = decisionNotes;
        req.ReviewedByUserId = reviewedByUserId;
        req.ReviewedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return req;
    }

    public static LeadershipRequestResponse ToResponse(LeadershipRequest r) => new(
        r.Id,
        r.GroupId,
        r.Group?.Name ?? string.Empty,
        r.Group?.Slug ?? string.Empty,
        r.RequestedByUserId,
        r.RequestedBy?.DisplayName ?? r.RequestedBy?.Email,
        r.ContactEmail,
        r.Reason,
        r.Status,
        r.DecisionNotes,
        r.CreatedAt,
        r.ReviewedAt);
}
