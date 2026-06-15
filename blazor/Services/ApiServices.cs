using StudentGroupsHub.DTOs.Responses;
using StudentGroupsHub.Models;
using StudentGroupsHub.Services;

namespace StudentGroupsHub.Services;

// ─────────────────────────────────────────────────────────────────────────────
// Blazor wrapper services — inject domain services directly.
// No HttpClient. No token acquisition. In-process calls only.
// DbSafe<T>: wraps every DB call to return null/empty on connection failure
// instead of crashing the Blazor prerender pipeline.
// ─────────────────────────────────────────────────────────────────────────────

internal static class DbSafe
{
    internal static async Task<T> TryAsync<T>(Func<Task<T>> fn, T fallback)
    {
        try { return await fn(); }
        catch (Exception ex) when (IsDbError(ex)) { return fallback; }
    }

    private static bool IsDbError(Exception ex)
    {
        var msg = ex.Message + (ex.InnerException?.Message ?? "");
        return ex is Npgsql.NpgsqlException
            || ex is Npgsql.PostgresException
            || msg.Contains("ENOTFOUND")
            || msg.Contains("tenant/user")
            || msg.Contains("Connection refused")
            || msg.Contains("Network is unreachable")
            || (ex.InnerException is not null && IsDbError(ex.InnerException));
    }
}

// ─── Group API Service ────────────────────────────────────────────────────────

public class GroupApiService(
    GroupService groupService,
    MembershipService membershipService,
    CurrentUserService currentUser)
{
    public async Task<PaginatedResponse<GroupModel>?> GetAllAsync(
        string? search = null, string? category = null, int page = 1, int pageSize = 20)
    {
        return await DbSafe.TryAsync(async () =>
        {
            var (items, total) = await groupService.GetAllActiveAsync(search, category, page, pageSize);
            // Single batch query for all member counts — eliminates N+1
            var counts = await groupService.GetMemberCountsAsync(items.Select(g => g.Id));
            var responses = items.Select(g =>
                MapGroup(g, counts.GetValueOrDefault(g.Id, 0))).ToList();
            return new PaginatedResponse<GroupModel>(
                responses, page, pageSize, total,
                (int)Math.Ceiling((double)total / pageSize));
        }, null);
    }

    public async Task<List<string>> GetCategoriesAsync()
        => await DbSafe.TryAsync(() => groupService.GetCategoriesAsync(), []);

    public async Task<GroupModel?> GetBySlugAsync(string slug)
        => await DbSafe.TryAsync(async () =>
        {
            var g = await groupService.GetBySlugAsync(slug);
            if (g is null) return null;
            var count = await groupService.GetMemberCountAsync(g.Id);
            return MapGroup(g, count);
        }, null);

    public async Task<List<GroupMemberModel>> GetMembersAsync(Guid groupId)
        => await DbSafe.TryAsync(async () =>
        {
            var members = await membershipService.GetAcceptedAsync(groupId);
            return members.Select(m => new GroupMemberModel(
                m.Id, m.UserId,
                m.User?.Email ?? "",
                m.User?.DisplayName,
                m.User?.AvatarUrl,
                m.Status,
                m.RespondedAt)).ToList();
        }, []);

    public async Task<List<MembershipModel>> GetPendingMembershipsAsync(Guid groupId)
    {
        var pending = await membershipService.GetPendingAsync(groupId);
        return pending.Select(MapMembership).ToList();
    }

    public async Task<JoinGroupResponse?> JoinAsync(Guid groupId)
    {
        var userId = await currentUser.GetUserIdAsync();
        if (userId == Guid.Empty) throw new InvalidOperationException("No autenticado.");
        var m = await membershipService.JoinAsync(userId, groupId);
        if (m is null) return null;
        return new JoinGroupResponse(m.Id, m.GroupId, m.UserId, m.Status, m.RequestedAt);
    }

    public async Task<MembershipModel?> ApproveMembershipAsync(Guid groupId, Guid membershipId, string? notes = null)
    {
        var m = await membershipService.ApproveAsync(membershipId, notes);
        return m is null ? null : MapMembership(m);
    }

    public async Task<MembershipModel?> RejectMembershipAsync(Guid groupId, Guid membershipId, string? notes = null)
    {
        var m = await membershipService.RejectAsync(membershipId, notes);
        return m is null ? null : MapMembership(m);
    }

    public async Task RemoveMemberAsync(Guid groupId, Guid userId)
        => await membershipService.RemoveAsync(groupId, userId);

    private static GroupModel MapGroup(Group g, int memberCount) => new(
        g.Id, g.Slug, g.Name, g.Description, g.Category,
        g.LogoUrl, g.BannerUrl, g.ContactEmail, g.ContactInfo,
        g.Status, memberCount, g.CreatedAt, g.UpdatedAt);

    private static MembershipModel MapMembership(Membership m) => new(
        m.Id, m.UserId,
        m.User?.Email ?? "",
        m.User?.DisplayName,
        m.User?.AvatarUrl,
        m.Status, m.RequestedAt, m.RespondedAt);
}

// ─── Event API Service ────────────────────────────────────────────────────────

public class EventApiService(EventService eventService, CurrentUserService currentUser)
{
    public async Task<List<EventModel>> GetUpcomingAsync(string? search = null, Guid? groupId = null)
        => await DbSafe.TryAsync(async () => await MapEventsAsync(await eventService.GetUpcomingAsync(search, groupId)), []);

    public async Task<List<EventModel>> GetForGroupAsync(Guid groupId)
        => await DbSafe.TryAsync(async () => await MapEventsAsync(await eventService.GetForGroupAsync(groupId)), []);

    public async Task<EventModel?> GetByIdAsync(Guid id)
        => await DbSafe.TryAsync(async () =>
        {
            var ev = await eventService.GetByIdAsync(id);
            if (ev is null) return null;
            var count = await eventService.GetRsvpCountAsync(ev.Id);
            return MapEvent(ev, count);
        }, null);

    public async Task UpsertRsvpAsync(Guid eventId, string status)
    {
        var userId = await currentUser.GetUserIdAsync();
        if (userId == Guid.Empty) throw new InvalidOperationException("No autenticado.");
        await eventService.UpsertRsvpAsync(eventId, userId, status);
    }

    public async Task RemoveRsvpAsync(Guid eventId)
    {
        var userId = await currentUser.GetUserIdAsync();
        if (userId == Guid.Empty) return;
        await eventService.RemoveRsvpAsync(eventId, userId);
    }

    private async Task<List<EventModel>> MapEventsAsync(List<Event> events)
    {
        // Sequential — EF Core does not allow concurrent queries on the same DbContext
        var result = new List<EventModel>();
        foreach (var ev in events)
        {
            var count = await eventService.GetRsvpCountAsync(ev.Id);
            result.Add(MapEvent(ev, count));
        }
        return result;
    }

    private static EventModel MapEvent(Event ev, int rsvpCount) => new(
        ev.Id, ev.GroupId, ev.Group?.Name ?? "",
        ev.Title, ev.Description, ev.Location, ev.BannerUrl,
        ev.StartAt, ev.EndAt, ev.Timezone,
        ev.Capacity, rsvpCount, ev.Status, ev.Visibility, ev.CreatedAt);
}

// ─── User API Service ─────────────────────────────────────────────────────────

public class UserApiService(CurrentUserService currentUser)
{
    public async Task<UserModel?> GetMeAsync()
        => await DbSafe.TryAsync(async () =>
        {
            var user = await currentUser.GetUserAsync();
            return user is null ? null : MapUser(user);
        }, null);

    private static UserModel MapUser(User u) => new(
        u.Id, u.EntraOid, u.Email, u.DisplayName, u.AvatarUrl,
        u.Role, u.Status, u.Role == "admin",
        u.CreatedAt, u.UpdatedAt);
}

// ─── Notification API Service ─────────────────────────────────────────────────

public class NotificationApiService(NotificationService notificationService, CurrentUserService currentUser)
{
    public async Task<List<NotificationModel>> GetMineAsync()
        => await DbSafe.TryAsync(async () =>
        {
            var userId = await currentUser.GetUserIdAsync();
            if (userId == Guid.Empty) return [];
            var notifs = await notificationService.GetForUserAsync(userId);
            return notifs.Select(n => new NotificationModel(
                n.Id, n.Kind, n.Title, n.Body, n.Href, n.Read, n.CreatedAt)).ToList();
        }, []);

    public async Task MarkAllReadAsync()
    {
        var userId = await currentUser.GetUserIdAsync();
        if (userId != Guid.Empty)
            await notificationService.MarkAllReadAsync(userId);
    }

    public async Task MarkReadAsync(Guid id)
    {
        var userId = await currentUser.GetUserIdAsync();
        if (userId != Guid.Empty)
            await notificationService.MarkReadAsync(id, userId);
    }
}

// ─── Group Registration API Service ──────────────────────────────────────────

public class GroupRegistrationApiService(
    GroupRegistrationRequestService registrationService,
    CurrentUserService currentUser)
{
    public async Task<List<GroupRegistrationRequestModel>> GetMineAsync()
        => await DbSafe.TryAsync(async () =>
        {
            var userId = await currentUser.GetUserIdAsync();
            if (userId == Guid.Empty) return [];
            return (await registrationService.GetByUserAsync(userId)).Select(MapRequest).ToList();
        }, []);

    public async Task<List<GroupRegistrationRequestModel>> GetAllPendingAsync()
        => (await registrationService.GetAllPendingAsync()).Select(MapRequest).ToList();

    public async Task<GroupRegistrationRequestModel?> CreateAsync(CreateGroupRegistrationRequest request)
    {
        var userId = await currentUser.GetUserIdAsync();
        if (userId == Guid.Empty) throw new InvalidOperationException("No autenticado.");
        var r = await registrationService.CreateAsync(
            userId, request.ProposedGroupName,
            request.ProposedDescription, request.ContactEmail);
        return MapRequest(r!);
    }

    public async Task<GroupRegistrationRequestModel?> ApproveAsync(Guid id, string? notes = null)
    {
        var userId = await currentUser.GetUserIdAsync();
        var r = await registrationService.ApproveAsync(id, userId, notes);
        return r is null ? null : MapRequest(r);
    }

    public async Task<GroupRegistrationRequestModel?> RejectAsync(Guid id, string? notes = null)
    {
        var userId = await currentUser.GetUserIdAsync();
        var r = await registrationService.RejectAsync(id, userId, notes);
        return r is null ? null : MapRequest(r);
    }

    private static GroupRegistrationRequestModel MapRequest(GroupRegistrationRequest r) => new(
        r.Id, r.RequestedByUserId,
        r.RequestedBy?.DisplayName ?? r.RequestedBy?.Email,
        r.ProposedGroupName, r.ProposedDescription,
        r.ContactEmail, r.Status, r.DecisionNotes,
        r.CreatedAt, r.ReviewedAt);
}

// ─── Dashboard API Service ────────────────────────────────────────────────────

public class DashboardApiService(DashboardService dashboardService, CurrentUserService currentUser)
{
    public async Task<DashboardStudentModel?> GetStudentAsync()
        => await DbSafe.TryAsync(async () =>
        {
            var userId = await currentUser.GetUserIdAsync();
            if (userId == Guid.Empty) return null;
            var data = await dashboardService.GetStudentDashboardAsync(userId);
            return new DashboardStudentModel(
                MapGroups(data.JoinedGroups),
                MapMemberships(data.PendingRequests),
                MapEvents(data.UpcomingEvents));
        }, null);

    public async Task<DashboardLeaderModel?> GetLeaderAsync()
        => await DbSafe.TryAsync(async () =>
        {
            var userId = await currentUser.GetUserIdAsync();
            if (userId == Guid.Empty) return null;
            var data = await dashboardService.GetLeaderDashboardAsync(userId);
            return new DashboardLeaderModel(
                MapGroups(data.ManagedGroups),
                MapMemberships(data.PendingMembershipRequests),
                MapEvents(data.UpcomingEvents));
        }, null);

    public async Task<DashboardAdminModel?> GetAdminAsync()
        => await DbSafe.TryAsync(async () =>
        {
            var data = await dashboardService.GetAdminDashboardAsync();
            return new DashboardAdminModel(
                data.TotalUsers, data.TotalGroups, data.ActiveGroups,
                data.PendingGroupRequests, data.TotalEvents,
                data.TotalMemberships, data.TotalParticipations);
        }, null);

    private static List<GroupModel> MapGroups(List<GroupResponse> groups)
        => groups.Select(g => new GroupModel(
            g.Id, g.Slug, g.Name, g.Description, g.Category,
            g.LogoUrl, g.BannerUrl, g.ContactEmail, g.ContactInfo,
            g.Status, g.MemberCount, g.CreatedAt, g.UpdatedAt)).ToList();

    private static List<MembershipModel> MapMemberships(List<MembershipResponse> memberships)
        => memberships.Select(m => new MembershipModel(
            m.MembershipId, m.UserId, m.Email, m.DisplayName,
            m.AvatarUrl, m.Status, m.RequestedAt, m.RespondedAt)).ToList();

    private static List<EventModel> MapEvents(List<EventResponse> events)
        => events.Select(e => new EventModel(
            e.Id, e.GroupId, e.GroupName, e.Title, e.Description,
            e.Location, e.BannerUrl, e.StartAt, e.EndAt, e.Timezone,
            e.Capacity, e.RsvpCount, e.Status, e.Visibility, e.CreatedAt)).ToList();
}

// ─── Admin API Service ────────────────────────────────────────────────────────

public class AdminApiService(
    UserService userService,
    DashboardService dashboardService,
    GroupService groupService,
    Microsoft.EntityFrameworkCore.IDbContextFactory<StudentGroupsHub.Data.AppDbContext> dbFactory)
{
    public async Task<PaginatedResponse<UserModel>?> GetUsersAsync(
        string? search = null, string? role = null, string? status = null,
        int page = 1, int pageSize = 20)
    {
        using var db = dbFactory.CreateDbContext();
        var q = db.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            q = q.Where(u => (u.Email.ToLower().Contains(s))
                          || (u.DisplayName != null && u.DisplayName.ToLower().Contains(s)));
        }
        if (!string.IsNullOrWhiteSpace(role))   q = q.Where(u => u.Role == role);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(u => u.Status == status);

        var total = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(q);
        var items = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
            .ToListAsync(q.OrderBy(u => u.Email)
                .Skip((page - 1) * pageSize).Take(pageSize));

        return new PaginatedResponse<UserModel>(
            items.Select(MapUser).ToList(), page, pageSize, total,
            (int)Math.Ceiling((double)total / pageSize));
    }

    public async Task<DashboardAdminModel?> GetMetricsAsync()
    {
        var data = await dashboardService.GetAdminDashboardAsync();
        return new DashboardAdminModel(
            data.TotalUsers, data.TotalGroups, data.ActiveGroups,
            data.PendingGroupRequests, data.TotalEvents,
            data.TotalMemberships, data.TotalParticipations);
    }

    public async Task<PaginatedResponse<GroupModel>?> GetAllGroupsAdminAsync(
        string? search = null, string? status = null, int page = 1, int pageSize = 20)
    {
        return await DbSafe.TryAsync(async () =>
        {
            var (items, total) = await groupService.GetAllAdminAsync(search, status, page, pageSize);
            var responses = new List<GroupModel>();
            foreach (var g in items)
            {
                var count = await groupService.GetMemberCountAsync(g.Id);
                responses.Add(new GroupModel(
                    g.Id, g.Slug, g.Name, g.Description, g.Category,
                    g.LogoUrl, g.BannerUrl, g.ContactEmail, g.ContactInfo,
                    g.Status, count, g.CreatedAt, g.UpdatedAt));
            }
            return new PaginatedResponse<GroupModel>(
                responses, page, pageSize, total,
                (int)Math.Ceiling((double)total / pageSize));
        }, null);
    }

    public async Task<bool> SetGroupStatusAsync(Guid groupId, string status)
    {
        return await DbSafe.TryAsync(
            () => groupService.SetStatusAsync(groupId, status), false);
    }

    public async Task<UserModel?> SetRoleAsync(Guid userId, string role)
    {
        using var db = dbFactory.CreateDbContext();
        var user = await db.Users.FindAsync(userId);
        if (user is null) return null;
        user.Role = role; user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapUser(user);
    }

    public async Task<UserModel?> SetStatusAsync(Guid userId, string status)
    {
        using var db = dbFactory.CreateDbContext();
        var user = await db.Users.FindAsync(userId);
        if (user is null) return null;
        user.Status = status; user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapUser(user);
    }

    private static UserModel MapUser(User u) => new(
        u.Id, u.EntraOid, u.Email, u.DisplayName, u.AvatarUrl,
        u.Role, u.Status, u.Role == "admin", u.CreatedAt, u.UpdatedAt);
}

// ─── Group Term API Service ───────────────────────────────────────────────────

public class GroupTermApiService(GroupTermService termService, CurrentUserService currentUser)
{
    public async Task<List<GroupTermModel>> GetTermsAsync(Guid groupId)
        => await DbSafe.TryAsync(async () =>
        {
            var terms = await termService.GetAllForGroupAsync(groupId);
            return terms.Select(MapTerm).ToList();
        }, []);

    /// <summary>All terms across all groups — single batch query for the timeline page.</summary>
    public async Task<List<GroupTermModel>> GetAllTermsAsync()
        => await DbSafe.TryAsync(async () =>
        {
            var terms = await termService.GetAllTermsWithMembersAsync();
            return terms.Select(MapTerm).ToList();
        }, []);

    public async Task<GroupTermModel?> CreateTermAsync(
        Guid groupId, string label, DateOnly startDate,
        DateOnly? endDate, string? notes)
    {
        var term = await termService.CreateTermAsync(groupId, label, startDate, endDate, notes);
        return MapTerm(term);
    }

    public async Task<TermMemberModel?> AddMemberAsync(
        Guid termId, string displayName, string roleLabel, int sortOrder = 0)
    {
        var m = await termService.AddMemberAsync(termId, displayName, roleLabel, sortOrder);
        return MapMember(m);
    }

    public async Task<bool> RemoveMemberAsync(Guid memberId)
        => await termService.RemoveMemberAsync(memberId);

    public async Task<bool> DeleteTermAsync(Guid termId)
        => await termService.DeleteTermAsync(termId);

    private static GroupTermModel MapTerm(GroupTerm t) => new(
        t.Id, t.GroupId, t.Label, t.StartDate, t.EndDate,
        t.Status, t.Notes, t.CreatedAt,
        t.Members.Select(MapMember).ToList());

    private static TermMemberModel MapMember(TermMember m) => new(
        m.Id, m.TermId, m.UserId, m.DisplayName,
        m.RoleLabel, m.SortOrder, m.AvatarUrl);
}
