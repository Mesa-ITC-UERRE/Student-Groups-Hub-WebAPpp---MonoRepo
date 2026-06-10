using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Responses;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class GroupService(AppDbContext db)
{
    // ─── Queries ──────────────────────────────────────────────────────────────

    public async Task<(List<Group> Items, int Total)> GetAllActiveAsync(
        string? search, string? category, int page, int pageSize)
    {
        var query = db.Groups
            .Where(g => g.Status == "active")
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(g => EF.Functions.ILike(g.Name, $"%{search}%"));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(g => g.Category == category);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(g => g.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Group?> GetBySlugAsync(string slug)
        => await db.Groups.FirstOrDefaultAsync(g => g.Slug == slug && g.Status == "active");

    public async Task<Group?> GetByIdAsync(Guid id)
        => await db.Groups.FindAsync(id);

    public async Task<int> GetMemberCountAsync(Guid groupId)
        => await db.Memberships.CountAsync(m => m.GroupId == groupId && m.Status == "accepted");

    public async Task<List<string>> GetCategoriesAsync()
        => await db.Groups
            .Where(g => g.Status == "active" && g.Category != null)
            .Select(g => g.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

    // ─── Commands ─────────────────────────────────────────────────────────────

    public async Task<Group> CreateAsync(
        string name, string? description, string? category,
        string? contactEmail, Guid createdByUserId)
    {
        var slug = await GenerateUniqueSlugAsync(name);
        var group = new Group
        {
            Id = Guid.NewGuid(),
            Slug = slug,
            Name = name,
            Description = description,
            Category = category,
            ContactEmail = contactEmail,
            Status = "active",
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        db.Groups.Add(group);
        await db.SaveChangesAsync();
        return group;
    }

    public async Task<Group?> UpdateAsync(Guid id, string? name, string? description,
        string? category, string? logoUrl, string? bannerUrl,
        string? contactEmail, string? contactInfo)
    {
        var group = await db.Groups.FindAsync(id);
        if (group is null) return null;

        if (name is not null) { group.Name = name; group.Slug = await GenerateUniqueSlugAsync(name, id); }
        if (description is not null) group.Description = description;
        if (category is not null) group.Category = category;
        if (logoUrl is not null) group.LogoUrl = logoUrl;
        if (bannerUrl is not null) group.BannerUrl = bannerUrl;
        if (contactEmail is not null) group.ContactEmail = contactEmail;
        if (contactInfo is not null) group.ContactInfo = contactInfo;
        group.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return group;
    }

    public async Task<bool> SetStatusAsync(Guid id, string status)
    {
        var group = await db.Groups.FindAsync(id);
        if (group is null) return false;
        group.Status = status;
        group.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsLeaderOfGroupAsync(Guid userId, Guid groupId)
        => await db.RoleAssignments.AnyAsync(r =>
            r.UserId == userId && r.GroupId == groupId && r.PermissionRole == "leader");

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private async Task<string> GenerateUniqueSlugAsync(string name, Guid? excludeId = null)
    {
        var base_ = name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("á","a").Replace("é","e").Replace("í","i")
            .Replace("ó","o").Replace("ú","u").Replace("ñ","n")
            .Replace("ü","u");
        // Strip any character that is not alphanumeric or hyphen
        base_ = System.Text.RegularExpressions.Regex.Replace(base_, @"[^a-z0-9\-]", "");
        base_ = System.Text.RegularExpressions.Regex.Replace(base_, @"-{2,}", "-").Trim('-');

        var slug = base_;
        var counter = 1;
        while (await db.Groups.AnyAsync(g => g.Slug == slug && g.Id != (excludeId ?? Guid.Empty)))
        {
            slug = $"{base_}-{counter++}";
        }
        return slug;
    }

    // ─── Mapping ──────────────────────────────────────────────────────────────

    public static GroupResponse ToResponse(Group g, int memberCount = 0) => new(
        g.Id, g.Slug, g.Name, g.Description, g.Category,
        g.LogoUrl, g.BannerUrl, g.ContactEmail, g.ContactInfo,
        g.Status, memberCount, g.CreatedAt, g.UpdatedAt
    );
}
