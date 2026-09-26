using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Responses;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class GroupService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<(List<Group> Items, int Total)> GetAllActiveAsync(
        string? search, string? category, int page, int pageSize)
    {
        using var db = dbFactory.CreateDbContext();
        var query = db.Groups.Where(g => g.Status == "active").AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(g => EF.Functions.ILike(g.Name, $"%{search}%"));
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(g => g.Category == category);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(g => g.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    /// <summary>Admin version — returns ALL groups regardless of status, with optional filters.</summary>
    public async Task<(List<Group> Items, int Total)> GetAllAdminAsync(
        string? search, string? status, int page, int pageSize)
    {
        using var db = dbFactory.CreateDbContext();
        var query = db.Groups.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(g => EF.Functions.ILike(g.Name, $"%{search}%"));
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(g => g.Status == status);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(g => g.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<Group?> GetBySlugAsync(string slug)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Groups.FirstOrDefaultAsync(g => g.Slug == slug && g.Status == "active");
    }

    public async Task<Group?> GetByIdAsync(Guid id)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Groups.FindAsync(id);
    }

    public async Task<int> GetMemberCountAsync(Guid groupId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Memberships.CountAsync(m => m.GroupId == groupId && m.Status == "accepted");
    }

    /// <summary>
    /// Batch version — fetches member counts for multiple groups in ONE query.
    /// Eliminates the N+1 problem when listing groups.
    /// </summary>
    public async Task<Dictionary<Guid, int>> GetMemberCountsAsync(IEnumerable<Guid> groupIds)
    {
        using var db = dbFactory.CreateDbContext();
        var ids = groupIds.ToList();
        return await db.Memberships
            .Where(m => ids.Contains(m.GroupId) && m.Status == "accepted")
            .GroupBy(m => m.GroupId)
            .Select(g => new { GroupId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.GroupId, x => x.Count);
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Groups
            .Where(g => g.Status == "active" && g.Category != null)
            .Select(g => g.Category!)
            .Distinct().OrderBy(c => c).ToListAsync();
    }

    public async Task<Group> CreateAsync(string name, string? description, string? category,
        string? contactEmail, Guid createdByUserId)
    {
        using var db = dbFactory.CreateDbContext();
        var slug = await GenerateUniqueSlugAsync(name);
        var group = new Group
        {
            Id = Guid.NewGuid(), Slug = slug, Name = name,
            Description = description, Category = category,
            ContactEmail = contactEmail, Status = "active",
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow,
        };
        db.Groups.Add(group);
        await db.SaveChangesAsync();
        return group;
    }

    public async Task<Group?> UpdateAsync(Guid id, string? name, string? description,
        string? category, string? logoUrl, string? bannerUrl,
        string? contactEmail, string? contactInfo)
    {
        using var db = dbFactory.CreateDbContext();
        var group = await db.Groups.FindAsync(id);
        if (group is null) return null;
        if (name        is not null) { group.Name = name; group.Slug = await GenerateUniqueSlugAsync(name, id); }
        if (description is not null) group.Description = description;
        if (category    is not null) group.Category    = category;
        if (logoUrl     is not null) group.LogoUrl     = logoUrl;
        if (bannerUrl   is not null) group.BannerUrl   = bannerUrl;
        if (contactEmail  is not null) group.ContactEmail  = contactEmail;
        if (contactInfo   is not null) group.ContactInfo   = contactInfo;
        group.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return group;
    }

    public async Task<bool> SetStatusAsync(Guid id, string status)
    {
        using var db = dbFactory.CreateDbContext();
        var group = await db.Groups.FindAsync(id);
        if (group is null) return false;
        group.Status = status; group.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsLeaderOfGroupAsync(Guid userId, Guid groupId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.RoleAssignments.AnyAsync(r =>
            r.UserId == userId && r.GroupId == groupId && r.PermissionRole == "leader");
    }

    public async Task<List<Guid>> GetLeaderIdsAsync(Guid groupId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.RoleAssignments
            .Where(r => r.GroupId == groupId && r.PermissionRole == "leader")
            .Select(r => r.UserId).ToListAsync();
    }

    /// <summary>One user's chosen badge title (e.g. Presidente/Presidenta) per group they lead.</summary>
    public async Task<Dictionary<Guid, string?>> GetMyLeaderTitlesAsync(Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.RoleAssignments
            .Where(r => r.UserId == userId && r.PermissionRole == "leader")
            .ToDictionaryAsync(r => r.GroupId, r => r.DisplayRole);
    }

    /// <summary>Member-card customization for anyone with a role assignment (leader OR assigned officer badge), keyed by user.</summary>
    public async Task<Dictionary<Guid, (string? DisplayRole, string? BadgeColor, bool IsLeader)>> GetMemberBadgesAsync(Guid groupId)
    {
        using var db = dbFactory.CreateDbContext();
        var rows = await db.RoleAssignments
            .Where(r => r.GroupId == groupId && (r.PermissionRole == "leader" || r.PermissionRole == "officer"))
            .Select(r => new { r.UserId, r.DisplayRole, r.BadgeColor, r.PermissionRole })
            .ToListAsync();
        return rows.ToDictionary(r => r.UserId, r => (r.DisplayRole, r.BadgeColor, r.PermissionRole == "leader"));
    }

    public async Task UpdateLeaderBadgeAsync(Guid groupId, Guid userId, string? badgeColor, string? displayRole)
    {
        using var db = dbFactory.CreateDbContext();
        var assignment = await db.RoleAssignments.FirstOrDefaultAsync(r =>
            r.GroupId == groupId && r.UserId == userId && r.PermissionRole == "leader");
        if (assignment is null)
            throw new InvalidOperationException("No tienes un rol de liderazgo en este grupo.");
        assignment.BadgeColor = badgeColor;
        assignment.DisplayRole = displayRole;
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Assigns a cosmetic role title + border color to a group member. If the target already has a
    /// role assignment (leader or a previously-assigned officer badge), updates it in place; otherwise
    /// creates a new "officer" row. "officer" carries no authorization — only "leader" does — so this
    /// can never grant management permissions to a regular member.
    /// </summary>
    public async Task SetMemberBadgeAsync(Guid groupId, Guid targetUserId, string badgeColor, string displayRole)
    {
        using var db = dbFactory.CreateDbContext();
        var isMember = await db.Memberships.AnyAsync(m =>
            m.GroupId == groupId && m.UserId == targetUserId && m.Status == "accepted");
        if (!isMember)
            throw new InvalidOperationException("Ese usuario no es miembro de este grupo.");

        var assignment = await db.RoleAssignments.FirstOrDefaultAsync(r =>
            r.GroupId == groupId && r.UserId == targetUserId);
        if (assignment is null)
        {
            assignment = new RoleAssignment
            {
                GroupId = groupId, UserId = targetUserId, PermissionRole = "officer",
            };
            db.RoleAssignments.Add(assignment);
        }
        assignment.BadgeColor = badgeColor;
        assignment.DisplayRole = displayRole;
        await db.SaveChangesAsync();
    }

    /// <summary>Removes an assigned officer badge. Never touches actual "leader" role assignments.</summary>
    public async Task RemoveMemberBadgeAsync(Guid groupId, Guid targetUserId)
    {
        using var db = dbFactory.CreateDbContext();
        var assignment = await db.RoleAssignments.FirstOrDefaultAsync(r =>
            r.GroupId == groupId && r.UserId == targetUserId && r.PermissionRole == "officer");
        if (assignment is null) return;
        db.RoleAssignments.Remove(assignment);
        await db.SaveChangesAsync();
    }

    /// <summary>Groups where the user has an accepted membership.</summary>
    public async Task<List<Group>> GetJoinedGroupsAsync(Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Memberships
            .Where(m => m.UserId == userId && m.Status == "accepted")
            .Include(m => m.Group)
            .Select(m => m.Group!)
            .Where(g => g.Status == "active")
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    /// <summary>Groups where the user has a leader role assignment.</summary>
    public async Task<List<Group>> GetLedGroupsAsync(Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.RoleAssignments
            .Where(r => r.UserId == userId && r.PermissionRole == "leader")
            .Include(r => r.Group)
            .Select(r => r.Group!)
            .Where(g => g.Status == "active")
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    /// <summary>Returns accepted group IDs for a user (for calendar filtering).</summary>
    public async Task<List<Guid>> GetJoinedGroupIdsAsync(Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        var memberIds = await db.Memberships
            .Where(m => m.UserId == userId && m.Status == "accepted")
            .Select(m => m.GroupId).ToListAsync();
        var leaderIds = await db.RoleAssignments
            .Where(r => r.UserId == userId && r.PermissionRole == "leader")
            .Select(r => r.GroupId).ToListAsync();
        return memberIds.Union(leaderIds).Distinct().ToList();
    }

    private async Task<string> GenerateUniqueSlugAsync(string name, Guid? excludeId = null)
    {
        using var db = dbFactory.CreateDbContext();
        var base_ = name.ToLowerInvariant()
            .Replace(" ","-").Replace("á","a").Replace("é","e")
            .Replace("í","i").Replace("ó","o").Replace("ú","u")
            .Replace("ñ","n").Replace("ü","u");
        base_ = System.Text.RegularExpressions.Regex.Replace(base_, @"[^a-z0-9\-]", "");
        base_ = System.Text.RegularExpressions.Regex.Replace(base_, @"-{2,}", "-").Trim('-');
        var slug = base_; var counter = 1;
        while (await db.Groups.AnyAsync(g => g.Slug == slug && g.Id != (excludeId ?? Guid.Empty)))
            slug = $"{base_}-{counter++}";
        return slug;
    }

    public static GroupResponse ToResponse(Group g, int memberCount = 0) => new(
        g.Id, g.Slug, g.Name, g.Description, g.Category,
        g.LogoUrl, g.BannerUrl, g.ContactEmail, g.ContactInfo,
        g.Status, memberCount, g.CreatedAt, g.UpdatedAt);
}
