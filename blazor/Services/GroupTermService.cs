using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class GroupTermService(IDbContextFactory<AppDbContext> dbFactory)
{
    // ─── Queries ──────────────────────────────────────────────────────────────

    public async Task<List<GroupTerm>> GetAllForGroupAsync(Guid groupId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.GroupTerms
            .Include(t => t.Members.OrderBy(m => m.SortOrder))
            .Where(t => t.GroupId == groupId)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync();
    }

    /// <summary>
    /// Returns ALL terms across ALL groups in a single query — for the timeline page.
    /// Avoids N+1 problem of querying per group.
    /// </summary>
    public async Task<List<GroupTerm>> GetAllTermsWithMembersAsync()
    {
        using var db = dbFactory.CreateDbContext();
        return await db.GroupTerms
            .Include(t => t.Members.OrderBy(m => m.SortOrder))
            .Include(t => t.Group)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync();
    }

    public async Task<GroupTerm?> GetByIdAsync(Guid termId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.GroupTerms
            .Include(t => t.Members.OrderBy(m => m.SortOrder))
            .Include(t => t.Group)
            .FirstOrDefaultAsync(t => t.Id == termId);
    }

    // ─── Commands ─────────────────────────────────────────────────────────────

    public async Task<GroupTerm> CreateTermAsync(
        Guid groupId, string label, DateOnly startDate,
        DateOnly? endDate, string? notes)
    {
        using var db = dbFactory.CreateDbContext();

        // Mark any existing active term as past
        if (endDate is null)
        {
            var activeTerm = await db.GroupTerms
                .Where(t => t.GroupId == groupId && t.Status == "active")
                .FirstOrDefaultAsync();
            if (activeTerm is not null)
            {
                activeTerm.Status = "past";
                activeTerm.EndDate = startDate.AddDays(-1);
            }
        }

        var term = new GroupTerm
        {
            Id        = Guid.NewGuid(),
            GroupId   = groupId,
            Label     = label,
            StartDate = startDate,
            EndDate   = endDate,
            Status    = endDate is null ? "active" : "past",
            Notes     = notes,
            CreatedAt = DateTime.UtcNow,
        };
        db.GroupTerms.Add(term);
        await db.SaveChangesAsync();
        return term;
    }

    public async Task<TermMember> AddMemberAsync(
        Guid termId, string displayName, string roleLabel,
        int sortOrder = 0, Guid? userId = null, string? avatarUrl = null)
    {
        using var db = dbFactory.CreateDbContext();
        var member = new TermMember
        {
            Id          = Guid.NewGuid(),
            TermId      = termId,
            UserId      = userId,
            DisplayName = displayName,
            RoleLabel   = roleLabel,
            SortOrder   = sortOrder,
            AvatarUrl   = avatarUrl,
            CreatedAt   = DateTime.UtcNow,
        };
        db.TermMembers.Add(member);
        await db.SaveChangesAsync();
        return member;
    }

    public async Task<bool> RemoveMemberAsync(Guid memberId)
    {
        using var db = dbFactory.CreateDbContext();
        var m = await db.TermMembers.FindAsync(memberId);
        if (m is null) return false;
        db.TermMembers.Remove(m);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTermAsync(Guid termId)
    {
        using var db = dbFactory.CreateDbContext();
        var t = await db.GroupTerms.FindAsync(termId);
        if (t is null) return false;
        db.GroupTerms.Remove(t);
        await db.SaveChangesAsync();
        return true;
    }
}
