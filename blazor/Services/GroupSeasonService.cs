using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;

namespace StudentGroupsHub.Services;

public record GroupSeasonResetResult(
    int Groups,
    int LeaderAssignments,
    int Memberships,
    int ActiveTerms);

public class GroupSeasonService(
    IDbContextFactory<AppDbContext> dbFactory,
    ILogger<GroupSeasonService> logger)
{
    public async Task<GroupSeasonResetResult> ResetSeasonAsync(
        IEnumerable<Guid> groupIds,
        Guid performedByUserId)
    {
        var targetIds = groupIds.Distinct().ToList();
        if (targetIds.Count == 0)
            return new GroupSeasonResetResult(0, 0, 0, 0);

        using var db = dbFactory.CreateDbContext();
        using var tx = await db.Database.BeginTransactionAsync();

        var leaderAssignments = await db.RoleAssignments
            .Where(r => targetIds.Contains(r.GroupId) && r.PermissionRole == "leader")
            .ToListAsync();
        var affectedLeaderIds = leaderAssignments.Select(r => r.UserId).Distinct().ToList();
        var leaderAssignmentCount = leaderAssignments.Count;

        if (leaderAssignments.Count > 0)
            db.RoleAssignments.RemoveRange(leaderAssignments);

        var memberships = await db.Memberships
            .Where(m => targetIds.Contains(m.GroupId) && (m.Status == "accepted" || m.Status == "pending"))
            .ToListAsync();
        var membershipCount = memberships.Count;
        foreach (var membership in memberships)
        {
            membership.Status = membership.Status == "accepted" ? "removed" : "rejected";
            membership.RespondedAt = DateTime.UtcNow;
            membership.Notes = membership.Status == "removed"
                ? "Reinicio anual del grupo. Debes volver a unirte en el nuevo ciclo."
                : "Solicitud cerrada por reinicio anual del grupo.";
        }

        var activeTerms = await db.GroupTerms
            .Where(t => targetIds.Contains(t.GroupId) && t.Status == "active" && t.EndDate == null)
            .ToListAsync();
        var activeTermCount = activeTerms.Count;
        var today = DateOnly.FromDateTime(DateTime.Today);
        foreach (var term in activeTerms)
        {
            term.Status = "past";
            term.EndDate = today;
        }

        await db.SaveChangesAsync();

        if (affectedLeaderIds.Count > 0)
        {
            var users = await db.Users.Where(u => affectedLeaderIds.Contains(u.Id)).ToListAsync();
            foreach (var user in users)
            {
                var stillLeads = await db.RoleAssignments.AnyAsync(r =>
                    r.UserId == user.Id && r.PermissionRole == "leader");
                if (!stillLeads && user.Role == "group_leader")
                {
                    user.Role = "student";
                    user.UpdatedAt = DateTime.UtcNow;
                }
            }
            await db.SaveChangesAsync();
        }

        await tx.CommitAsync();

        var result = new GroupSeasonResetResult(
            targetIds.Count,
            leaderAssignmentCount,
            membershipCount,
            activeTermCount);
        logger.LogInformation(
            "Group season reset by {ActorUserId}. Groups={GroupIds}; Leaders={LeaderAssignments}; Memberships={Memberships}; Terms={ActiveTerms}",
            performedByUserId,
            targetIds,
            result.LeaderAssignments,
            result.Memberships,
            result.ActiveTerms);
        return result;
    }

    public Task<GroupSeasonResetResult> ResetGroupAsync(Guid groupId, Guid performedByUserId)
        => ResetSeasonAsync([groupId], performedByUserId);
}
