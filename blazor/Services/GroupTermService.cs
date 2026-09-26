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

    /// <summary>Lightweight lookups so the API layer can authorize by group without loading the full term/member graph.</summary>
    public async Task<Guid?> GetGroupIdForTermAsync(Guid termId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.GroupTerms.Where(t => t.Id == termId)
            .Select(t => (Guid?)t.GroupId).FirstOrDefaultAsync();
    }

    public async Task<Guid?> GetGroupIdForMemberAsync(Guid memberId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.TermMembers.Where(m => m.Id == memberId)
            .Select(m => (Guid?)m.Term!.GroupId).FirstOrDefaultAsync();
    }

    // ─── Commands ─────────────────────────────────────────────────────────────

    public async Task<GroupTerm> CreateTermAsync(
        Guid groupId, string label, DateOnly startDate,
        DateOnly? endDate, string? notes)
    {
        if (endDate.HasValue && endDate < startDate)
            throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de inicio.");

        using var db = dbFactory.CreateDbContext();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        // "Current" is a date fact, not a creation-time choice — a term with a
        // future end date (e.g. Feb 2026 – Jan 2027) is still the current
        // administration today, not "past" just because it has a defined end.
        var isNewTermCurrent = endDate is null || endDate >= today;

        if (isNewTermCurrent)
        {
            // Retire every other still-current term for this group — by date,
            // not by the (possibly stale) Status column — so creating a new
            // administration never leaves two terms simultaneously "current".
            var stillCurrentTerms = await db.GroupTerms
                .Where(t => t.GroupId == groupId && (t.EndDate == null || t.EndDate >= today))
                .ToListAsync();
            foreach (var existing in stillCurrentTerms)
            {
                existing.Status = "past";
                existing.EndDate = startDate.AddDays(-1);
            }
        }

        var term = new GroupTerm
        {
            Id        = Guid.NewGuid(),
            GroupId   = groupId,
            Label     = label,
            StartDate = startDate,
            EndDate   = endDate,
            Status    = isNewTermCurrent ? "active" : "past",
            Notes     = notes,
            CreatedAt = DateTime.UtcNow,
        };
        db.GroupTerms.Add(term);
        await db.SaveChangesAsync();
        return term;
    }

    /// <summary>
    /// Adding to the current administration requires an existing accepted group member
    /// (userId must resolve to one) — a leader can't invent a name that isn't actually in
    /// the group. Past/historical administrations keep allowing free-text entries for
    /// people who documented history but were never on the platform (userId stays null).
    /// </summary>
    public async Task<TermMember> AddMemberAsync(
        Guid termId, string displayName, string roleLabel,
        int sortOrder = 0, Guid? userId = null, string? avatarUrl = null)
    {
        using var db = dbFactory.CreateDbContext();

        var term = await db.GroupTerms.FirstOrDefaultAsync(t => t.Id == termId);
        if (term is null)
            throw new InvalidOperationException("Administración no encontrada.");

        if (term.IsCurrent)
        {
            if (userId is null)
                throw new InvalidOperationException(
                    "Solo puedes agregar miembros actuales del grupo a la administración vigente. Selecciona uno de la lista.");

            var isMember = await db.Memberships.AnyAsync(m =>
                m.GroupId == term.GroupId && m.UserId == userId.Value && m.Status == "accepted");
            if (!isMember)
                throw new InvalidOperationException("Esa persona no es miembro actual de este grupo.");
        }

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
