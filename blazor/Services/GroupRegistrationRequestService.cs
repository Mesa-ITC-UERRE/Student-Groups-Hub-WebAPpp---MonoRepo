using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Responses;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class GroupRegistrationRequestService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<GroupRegistrationRequest> CreateAsync(
        Guid requestedByUserId, string proposedGroupName,
        string? proposedDescription, string contactEmail,
        string? proposedCategory = null)
    {
        using var db = dbFactory.CreateDbContext();
        var req = new GroupRegistrationRequest
        {
            Id = Guid.NewGuid(),
            RequestedByUserId = requestedByUserId,
            ProposedGroupName = proposedGroupName,
            ProposedDescription = proposedDescription,
            ContactEmail = contactEmail,
            ProposedCategory = proposedCategory,
            Status = "pending",
            CreatedAt = DateTime.UtcNow,
        };
        db.GroupRegistrationRequests.Add(req);
        await db.SaveChangesAsync();
        return req;
    }

    public async Task<List<GroupRegistrationRequest>> GetByUserAsync(Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.GroupRegistrationRequests
            .Where(r => r.RequestedByUserId == userId)
            .Include(r => r.RequestedBy)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<GroupRegistrationRequest>> GetAllPendingAsync()
    {
        using var db = dbFactory.CreateDbContext();
        return await db.GroupRegistrationRequests
            .Where(r => r.Status == "pending")
            .Include(r => r.RequestedBy)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<GroupRegistrationRequest?> GetByIdAsync(Guid id)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.GroupRegistrationRequests
            .Include(r => r.RequestedBy)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <summary>
    /// Approves a group registration request atomically:
    /// creates the group, assigns the leader role, inserts an accepted membership
    /// for the creator, and optionally promotes them to group_leader — all in one
    /// transaction using a single DbContext.
    /// </summary>
    public async Task<GroupRegistrationRequest?> ApproveAsync(
        Guid requestId, Guid reviewedByUserId, string? decisionNotes,
        string? finalCategory = null)
    {
        using var db = dbFactory.CreateDbContext();
        var req = await db.GroupRegistrationRequests
            .Include(r => r.RequestedBy)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (req is null || req.Status != "pending") return null;

        // Generate slug within the same context (no separate factory call)
        var slug = await GenerateUniqueSlugAsync(db, req.ProposedGroupName);
        var effectiveCategory = finalCategory ?? req.ProposedCategory;

        // Create the group — inline for full atomicity
        var group = new Group
        {
            Id = Guid.NewGuid(),
            Slug = slug,
            Name = req.ProposedGroupName,
            Description = req.ProposedDescription,
            Category = effectiveCategory,
            ContactEmail = req.ContactEmail,
            Status = "active",
            CreatedByUserId = req.RequestedByUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        db.Groups.Add(group);

        // Assign the requester as leader
        db.RoleAssignments.Add(new RoleAssignment
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            UserId = req.RequestedByUserId,
            PermissionRole = "leader",
            DisplayRole = LeaderBadgeOptions.DefaultTitle,
            CreatedAt = DateTime.UtcNow,
        });

        // Add creator as accepted member (fixes member-count = 0 bug)
        db.Memberships.Add(new Membership
        {
            Id = Guid.NewGuid(),
            UserId = req.RequestedByUserId,
            GroupId = group.Id,
            Status = "accepted",
            RequestedAt = DateTime.UtcNow,
            RespondedAt = DateTime.UtcNow,
        });

        // Promote user to group_leader role if they are still a student
        var user = await db.Users.FindAsync(req.RequestedByUserId);
        if (user is not null && user.Role == "student")
        {
            user.Role = "group_leader";
            user.UpdatedAt = DateTime.UtcNow;
        }

        // Update the request
        req.Status = "approved";
        req.DecisionNotes = decisionNotes;
        req.ReviewedByUserId = reviewedByUserId;
        req.ReviewedAt = DateTime.UtcNow;
        req.CreatedGroupId = group.Id;

        await db.SaveChangesAsync();
        return req;
    }

    public async Task<GroupRegistrationRequest?> RejectAsync(
        Guid requestId, Guid reviewedByUserId, string? decisionNotes)
    {
        using var db = dbFactory.CreateDbContext();
        var req = await db.GroupRegistrationRequests
            .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == "pending");

        if (req is null) return null;

        req.Status = "rejected";
        req.DecisionNotes = decisionNotes;
        req.ReviewedByUserId = reviewedByUserId;
        req.ReviewedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return req;
    }

    public static GroupRegistrationRequestResponse ToResponse(GroupRegistrationRequest r) => new(
        r.Id,
        r.RequestedByUserId,
        r.RequestedBy?.DisplayName ?? r.RequestedBy?.Email,
        r.ProposedGroupName,
        r.ProposedDescription,
        r.ContactEmail,
        r.ProposedCategory,
        r.Status,
        r.DecisionNotes,
        r.CreatedAt,
        r.ReviewedAt
    );

    private static async Task<string> GenerateUniqueSlugAsync(AppDbContext db, string name, Guid? excludeId = null)
    {
        var base_ = name.ToLowerInvariant()
            .Replace(" ", "-").Replace("á", "a").Replace("é", "e")
            .Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
            .Replace("ñ", "n").Replace("ü", "u");
        base_ = System.Text.RegularExpressions.Regex.Replace(base_, @"[^a-z0-9\-]", "");
        base_ = System.Text.RegularExpressions.Regex.Replace(base_, @"-{2,}", "-").Trim('-');
        var slug = base_; var counter = 1;
        while (await db.Groups.AnyAsync(g => g.Slug == slug && g.Id != (excludeId ?? Guid.Empty)))
            slug = $"{base_}-{counter++}";
        return slug;
    }
}
