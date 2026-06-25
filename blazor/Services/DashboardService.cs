using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Responses;

namespace StudentGroupsHub.Services;

public class DashboardService(IDbContextFactory<AppDbContext> dbFactory, GroupService groupService, EventService eventService)
{
    public async Task<DashboardStudentResponse> GetStudentDashboardAsync(Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        var joinedGroupIds = await db.Memberships
            .Where(m => m.UserId == userId && m.Status == "accepted")
            .Select(m => m.GroupId)
            .ToListAsync();

        var joinedGroups = await db.Groups
            .Where(g => joinedGroupIds.Contains(g.Id))
            .ToListAsync();

        var pendingRequests = await db.Memberships
            .Include(m => m.Group)
            .Where(m => m.UserId == userId && m.Status == "pending")
            .ToListAsync();

        var upcomingEvents = await db.Events
            .Include(e => e.Group)
            .Where(e => joinedGroupIds.Contains(e.GroupId)
                     && e.Status == "published"
                     && e.StartAt >= DateTime.UtcNow)
            .OrderBy(e => e.StartAt)
            .Take(5)
            .ToListAsync();

        var leadershipRequests = await db.LeadershipRequests
            .Include(r => r.Group)
            .Include(r => r.RequestedBy)
            .Where(r => r.RequestedByUserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        // Sequential — EF Core does not support concurrent queries on same context
        var groupResponses = new List<GroupResponse>();
        foreach (var g in joinedGroups)
        {
            var count = await groupService.GetMemberCountAsync(g.Id);
            groupResponses.Add(GroupService.ToResponse(g, count));
        }

        return new DashboardStudentResponse(
            groupResponses,
            pendingRequests.Select(MembershipService.ToResponse).ToList(),
            upcomingEvents.Select(e => EventService.ToResponse(e)).ToList(),
            leadershipRequests.Select(LeadershipRequestService.ToResponse).ToList()
        );
    }

    public async Task<DashboardLeaderResponse> GetLeaderDashboardAsync(Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        var managedGroupIds = await db.RoleAssignments
            .Where(r => r.UserId == userId && r.PermissionRole == "leader")
            .Select(r => r.GroupId)
            .ToListAsync();

        var managedGroups = await db.Groups
            .Where(g => managedGroupIds.Contains(g.Id))
            .ToListAsync();

        var pendingRequests = await db.Memberships
            .Include(m => m.User)
            .Where(m => managedGroupIds.Contains(m.GroupId) && m.Status == "pending")
            .OrderBy(m => m.RequestedAt)
            .ToListAsync();

        var upcomingEvents = await db.Events
            .Include(e => e.Group)
            .Where(e => managedGroupIds.Contains(e.GroupId)
                     && e.Status == "published"
                     && e.StartAt >= DateTime.UtcNow)
            .OrderBy(e => e.StartAt)
            .Take(10)
            .ToListAsync();

        // Sequential — EF Core does not support concurrent queries on same context
        var groupResponses = new List<GroupResponse>();
        foreach (var g in managedGroups)
        {
            var count = await groupService.GetMemberCountAsync(g.Id);
            groupResponses.Add(GroupService.ToResponse(g, count));
        }

        return new DashboardLeaderResponse(
            groupResponses,
            pendingRequests.Select(MembershipService.ToResponse).ToList(),
            upcomingEvents.Select(e => EventService.ToResponse(e)).ToList()
        );
    }

    public async Task<DashboardAdminResponse> GetAdminDashboardAsync()
    {
        using var db = dbFactory.CreateDbContext();
        var totalUsers          = await db.Users.CountAsync();
        var totalGroups         = await db.Groups.CountAsync();
        var activeGroups        = await db.Groups.CountAsync(g => g.Status == "active");
        var pendingRequests     = await db.GroupRegistrationRequests.CountAsync(r => r.Status == "pending");
        var pendingLeadershipRequests = await db.LeadershipRequests.CountAsync(r => r.Status == "pending");
        var totalEvents         = await db.Events.CountAsync();
        var totalMemberships    = await db.Memberships.CountAsync();
        var totalParticipations = await db.EventParticipations.CountAsync();

        return new DashboardAdminResponse(
            totalUsers, totalGroups, activeGroups, pendingRequests, pendingLeadershipRequests,
            totalEvents, totalMemberships, totalParticipations);
    }
}
