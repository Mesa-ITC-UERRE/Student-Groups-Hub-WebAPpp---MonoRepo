using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.DTOs.Requests;
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
    CurrentUserService currentUser,
    StorageService storageService)
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

    public async Task<string?> GetMyMembershipStatusAsync(Guid groupId)
        => await DbSafe.TryAsync(async () =>
        {
            var userId = await currentUser.GetUserIdAsync();
            if (userId == Guid.Empty) return null;
            var m = await membershipService.GetByUserAndGroupAsync(userId, groupId);
            return m?.Status;
        }, null);

    public async Task<bool> IsLeaderAsync(Guid groupId)
        => await DbSafe.TryAsync(async () =>
        {
            var user = await currentUser.GetUserAsync();
            if (!UserService.IsActive(user)) return false;
            return await groupService.IsLeaderOfGroupAsync(user!.Id, groupId);
        }, false);

    public async Task<List<MembershipModel>> GetPendingMembershipsAsync(Guid groupId)
    {
        var pending = await membershipService.GetPendingAsync(groupId);
        return pending.Select(MapMembership).ToList();
    }

    public async Task<JoinGroupResponse?> JoinAsync(Guid groupId)
    {
        var userId = await currentUser.GetActiveUserIdAsync();
        var m = await membershipService.JoinAsync(userId, groupId);
        if (m is null) return null;
        return new JoinGroupResponse(m.Id, m.GroupId, m.UserId, m.Status, m.RequestedAt);
    }

    public async Task<MembershipModel?> ApproveMembershipAsync(Guid groupId, Guid membershipId, string? notes = null)
    {
        await currentUser.RequireActiveUserAsync();
        var m = await membershipService.ApproveAsync(membershipId, notes);
        return m is null ? null : MapMembership(m);
    }

    public async Task<MembershipModel?> RejectMembershipAsync(Guid groupId, Guid membershipId, string? notes = null)
    {
        await currentUser.RequireActiveUserAsync();
        var m = await membershipService.RejectAsync(membershipId, notes);
        return m is null ? null : MapMembership(m);
    }

    public async Task RemoveMemberAsync(Guid groupId, Guid userId)
    {
        var user = await currentUser.RequireActiveUserAsync();

        var isAdmin = user.Role == "admin";
        var isLeader = await groupService.IsLeaderOfGroupAsync(user.Id, groupId);
        if (!isAdmin && !isLeader)
            throw new InvalidOperationException("No tienes permiso para administrar miembros de este grupo.");

        await membershipService.RemoveAsync(groupId, userId);
    }

    /// <summary>
    /// Uploads a pre-buffered image as the logo for <paramref name="groupId"/>.
    /// Requires the caller to be a group leader or admin.
    /// Throws <see cref="InvalidOperationException"/> on validation/permission/upload failure.
    /// </summary>
    public async Task<GroupModel?> UploadLogoAsync(
        Guid groupId, byte[] fileBytes, string contentType, string ext)
    {
        if (!StorageService.AllowedMimeTypes.Contains(contentType))
            throw new InvalidOperationException(
                "Formato no permitido. Usa JPEG, PNG, WebP o GIF.");
        if (fileBytes.Length > StorageService.MaxBytes)
            throw new InvalidOperationException(
                "El archivo supera el límite de 5 MB.");

        var user = await currentUser.RequireActiveUserAsync();

        var isAdmin  = user.Role == "admin";
        var isLeader = await groupService.IsLeaderOfGroupAsync(user.Id, groupId);
        if (!isAdmin && !isLeader)
            throw new InvalidOperationException(
                "No tienes permiso para editar este grupo.");

        var existing = await groupService.GetByIdAsync(groupId);
        var oldLogoUrl = existing?.LogoUrl;

        var fileName = $"{Guid.NewGuid():N}.{ext}";
        using var stream = new MemoryStream(fileBytes);
        var newUrl = await storageService.UploadAsync(
            $"group-logos/{groupId}", fileName, stream, contentType);

        if (newUrl is null)
            throw new InvalidOperationException(
                "Error al subir la imagen. Inténtalo de nuevo.");

        var updated = await groupService.UpdateAsync(
            groupId, null, null, null, newUrl, null, null, null);
        if (updated is null) return null;

        // Clean up old logo after DB update succeeds (fire-and-forget)
        if (!string.IsNullOrEmpty(oldLogoUrl))
            await storageService.DeleteByUrlAsync(oldLogoUrl);

        var count = await groupService.GetMemberCountAsync(groupId);
        return MapGroup(updated, count);
    }

    /// <summary>
    /// Uploads a pre-buffered image as the hero banner for <paramref name="groupId"/>.
    /// Requires the caller to be a group leader or admin.
    /// Throws <see cref="InvalidOperationException"/> on validation/permission/upload failure.
    /// </summary>
    public async Task<GroupModel?> UploadBannerAsync(
        Guid groupId, byte[] fileBytes, string contentType, string ext)
    {
        if (!StorageService.AllowedMimeTypes.Contains(contentType))
            throw new InvalidOperationException(
                "Formato no permitido. Usa JPEG, PNG, WebP o GIF.");
        if (fileBytes.Length > StorageService.MaxBytes)
            throw new InvalidOperationException(
                "El archivo supera el límite de 5 MB.");

        var user = await currentUser.RequireActiveUserAsync();

        var isAdmin  = user.Role == "admin";
        var isLeader = await groupService.IsLeaderOfGroupAsync(user.Id, groupId);
        if (!isAdmin && !isLeader)
            throw new InvalidOperationException(
                "No tienes permiso para editar este grupo.");

        var existing    = await groupService.GetByIdAsync(groupId);
        var oldBannerUrl = existing?.BannerUrl;

        var fileName = $"{Guid.NewGuid():N}.{ext}";
        using var stream = new MemoryStream(fileBytes);
        var newUrl = await storageService.UploadAsync(
            $"group-banners/{groupId}", fileName, stream, contentType);

        if (newUrl is null)
            throw new InvalidOperationException(
                "Error al subir la imagen. Inténtalo de nuevo.");

        var updated = await groupService.UpdateAsync(
            groupId, null, null, null, null, newUrl, null, null);
        if (updated is null) return null;

        if (!string.IsNullOrEmpty(oldBannerUrl))
            await storageService.DeleteByUrlAsync(oldBannerUrl);

        var count = await groupService.GetMemberCountAsync(groupId);
        return MapGroup(updated, count);
    }

    private static GroupModel MapGroup(Group g, int memberCount) => new(
        g.Id, g.Slug, g.Name, g.Description, g.Category,
        g.LogoUrl, g.BannerUrl, g.ContactEmail, g.ContactInfo,
        g.Status, memberCount, g.CreatedAt, g.UpdatedAt);

    private static MembershipModel MapMembership(Membership m) => new(
        m.Id, m.UserId, m.GroupId,
        m.User?.Email ?? "",
        m.User?.DisplayName,
        m.User?.AvatarUrl,
        m.Status, m.RequestedAt, m.RespondedAt);
}

// ─── Event API Service ────────────────────────────────────────────────────────

public class EventApiService(
    EventService eventService,
    CurrentUserService currentUser,
    GroupService groupService,
    StorageService storageService)
{
    public async Task<List<EventModel>> GetUpcomingAsync(
        string? search = null,
        Guid? groupId = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null)
        => await DbSafe.TryAsync(async () => await MapEventsAsync(
            await eventService.GetUpcomingAsync(search, groupId, fromUtc, toUtc)), []);

    public async Task<List<EventModel>> GetForGroupAsync(Guid groupId)
        => await DbSafe.TryAsync(async () => await MapEventsAsync(await eventService.GetForGroupAsync(groupId)), []);

    public async Task<List<EventModel>> GetAllPublishedAsync()
        => await DbSafe.TryAsync(async () => await MapEventsAsync(await eventService.GetAllPublishedAsync()), []);

    public async Task<EventModel?> GetByIdAsync(Guid id)
        => await DbSafe.TryAsync(async () =>
        {
            var ev = await eventService.GetByIdAsync(id);
            if (ev is null) return null;
            var count = await eventService.GetRsvpCountAsync(ev.Id);
            return MapEvent(ev, count);
        }, null);

    public async Task<EventModel?> CreateEventAsync(
        Guid groupId,
        BlazorCreateEventRequest req,
        byte[]? imageBytes = null,
        string? imageContentType = null,
        string? imageExt = null)
    {
        var user = await currentUser.RequireActiveUserAsync();
        var userId = user.Id;
        var isAdmin  = user?.Role == "admin";
        var isLeader = await groupService.IsLeaderOfGroupAsync(userId, groupId);
        if (!isAdmin && !isLeader)
            throw new InvalidOperationException("No tienes permiso para crear eventos en este grupo.");

        var ev = await eventService.CreateAsync(groupId, userId, new DTOs.Requests.CreateEventRequest(
            req.Title,
            req.Description,
            req.Location,
            req.StartAt,
            req.EndAt,
            req.Capacity,
            req.Status,
            req.Visibility));

        if (imageBytes is not null && imageContentType is not null && imageExt is not null)
        {
            if (!StorageService.AllowedMimeTypes.Contains(imageContentType))
                throw new InvalidOperationException("Formato de imagen no permitido.");
            if (imageBytes.Length > StorageService.MaxBytes)
                throw new InvalidOperationException("La imagen supera el límite de 5 MB.");

            var fileName = $"{Guid.NewGuid():N}.{imageExt}";
            using var stream = new MemoryStream(imageBytes);
            var imageUrl = await storageService.UploadAsync(
                $"event-banners/{groupId}", fileName, stream, imageContentType);

            if (imageUrl is null)
                throw new InvalidOperationException("No se pudo subir la foto del evento.");

            ev = await eventService.UpdateAsync(groupId, ev.Id, new DTOs.Requests.UpdateEventRequest(
                null, null, null, imageUrl, null, null, null, null, null)) ?? ev;
        }

        return MapEvent(ev, 0);
    }

    public async Task<bool> CanManageAsync(Guid groupId)
    {
        var user = await currentUser.GetUserAsync();
        if (user is null || !UserService.IsActive(user)) return false;
        return user.Role == "admin" || await groupService.IsLeaderOfGroupAsync(user.Id, groupId);
    }

    public async Task<EventModel?> UpdateEventAsync(
        Guid groupId,
        Guid eventId,
        BlazorUpdateEventRequest req)
    {
        var user = await currentUser.RequireActiveUserAsync();
        var canManage = user.Role == "admin" || await groupService.IsLeaderOfGroupAsync(user.Id, groupId);
        if (!canManage)
            throw new InvalidOperationException("No tienes permiso para editar este evento.");

        var updated = await eventService.UpdateAsync(groupId, eventId, new DTOs.Requests.UpdateEventRequest(
            req.Title,
            req.Description ?? string.Empty,
            req.Location ?? string.Empty,
            null,
            req.StartAt,
            req.EndAt,
            req.Capacity,
            req.Status,
            req.Visibility,
            ClearCapacity: req.Capacity is null));

        if (updated is null) return null;
        var count = await eventService.GetRsvpCountAsync(updated.Id);
        return MapEvent(updated, count);
    }

    public async Task UpsertRsvpAsync(Guid eventId, string status)
    {
        var userId = await currentUser.GetActiveUserIdAsync();
        await eventService.UpsertRsvpAsync(eventId, userId, status);
    }

    public async Task RemoveRsvpAsync(Guid eventId)
    {
        var userId = await currentUser.GetActiveUserIdAsync();
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

public class UserApiService(
    CurrentUserService currentUser,
    UserService userService,
    StorageService storageService)
{
    public async Task<UserModel?> GetMeAsync()
        => await DbSafe.TryAsync(async () =>
        {
            var user = await currentUser.GetUserAsync();
            return user is null ? null : MapUser(user);
        }, null);

    /// <summary>
    /// Uploads a pre-buffered image as the current user's avatar.
    /// Throws <see cref="InvalidOperationException"/> on validation or upload failure.
    /// </summary>
    public async Task<UserModel?> UploadAvatarAsync(
        byte[] fileBytes, string contentType, string ext)
    {
        if (!StorageService.AllowedMimeTypes.Contains(contentType))
            throw new InvalidOperationException(
                "Formato no permitido. Usa JPEG, PNG, WebP o GIF.");
        if (fileBytes.Length > StorageService.MaxBytes)
            throw new InvalidOperationException(
                "El archivo supera el límite de 5 MB.");

        var existingUser = await currentUser.RequireActiveUserAsync();
        var userId = existingUser.Id;
        var oldUrl = existingUser?.AvatarUrl;

        var fileName = $"{Guid.NewGuid():N}.{ext}";
        using var stream = new MemoryStream(fileBytes);
        var newUrl = await storageService.UploadAsync(
            $"avatars/{userId}", fileName, stream, contentType);

        if (newUrl is null)
            throw new InvalidOperationException(
                "Error al subir la imagen. Inténtalo de nuevo.");

        var updated = await userService.UpdateAsync(userId, null, newUrl);
        if (updated is null) return null;

        // Clean up old avatar after DB update succeeds (fire-and-forget)
        if (!string.IsNullOrEmpty(oldUrl))
            await storageService.DeleteByUrlAsync(oldUrl);

        currentUser.Invalidate();
        return MapUser(updated);
    }

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
        var userId = await currentUser.GetActiveUserIdAsync();
        await notificationService.MarkAllReadAsync(userId);
    }

    public async Task MarkReadAsync(Guid id)
    {
        var userId = await currentUser.GetActiveUserIdAsync();
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

    public async Task<GroupRegistrationRequestModel?> CreateAsync(StudentGroupsHub.DTOs.Requests.CreateGroupRegistrationRequest request)
    {
        var userId = await currentUser.GetActiveUserIdAsync();
        var r = await registrationService.CreateAsync(
            userId, request.ProposedGroupName,
            request.ProposedDescription, request.ContactEmail,
            request.ProposedCategory);
        return MapRequest(r!);
    }

    public Task<GroupRegistrationRequestModel?> CreateAsync(StudentGroupsHub.Models.CreateGroupRegistrationRequest request)
        => CreateAsync(new StudentGroupsHub.DTOs.Requests.CreateGroupRegistrationRequest(
            request.ProposedGroupName,
            request.ProposedDescription,
            request.ContactEmail,
            request.ProposedCategory));

    public async Task<GroupRegistrationRequestModel?> ApproveAsync(Guid id, string? notes = null, string? finalCategory = null)
    {
        var userId = await currentUser.GetActiveUserIdAsync();
        var r = await registrationService.ApproveAsync(id, userId, notes, finalCategory);
        return r is null ? null : MapRequest(r);
    }

    public async Task<GroupRegistrationRequestModel?> RejectAsync(Guid id, string? notes = null)
    {
        var userId = await currentUser.GetActiveUserIdAsync();
        var r = await registrationService.RejectAsync(id, userId, notes);
        return r is null ? null : MapRequest(r);
    }

    private static GroupRegistrationRequestModel MapRequest(GroupRegistrationRequest r) => new(
        r.Id, r.RequestedByUserId,
        r.RequestedBy?.DisplayName ?? r.RequestedBy?.Email,
        r.ProposedGroupName, r.ProposedDescription,
        r.ContactEmail, r.ProposedCategory, r.Status, r.DecisionNotes,
        r.CreatedAt, r.ReviewedAt);
}

public class EventPostApiService(
    EventPostService eventPostService,
    EventService eventService,
    MembershipService membershipService,
    GroupService groupService,
    CurrentUserService currentUser,
    StorageService storageService)
{
    public async Task<List<EventPostModel>> GetPostsAsync(Guid eventId)
        => await DbSafe.TryAsync(async () =>
        {
            var posts = await eventPostService.GetForEventAsync(eventId);
            return posts.Select(MapPost).ToList();
        }, []);

    public async Task<Dictionary<Guid, List<string>>> GetImageUrlsForEventsAsync(IEnumerable<Guid> eventIds, int takePerEvent = 3)
        => await DbSafe.TryAsync(
            () => eventPostService.GetImageUrlsForEventsAsync(eventIds, takePerEvent),
            []);

    public async Task<bool> CanPostAsync(Guid eventId)
    {
        var user = await currentUser.GetUserAsync();
        if (!UserService.IsActive(user)) return false;
        var activeUser = user!;

        var ev = await eventService.GetByIdAsync(eventId);
        if (ev is null) return false;
        if (ev.EndAt >= DateTime.UtcNow && ev.Status != "canceled") return false;

        if (activeUser.Role == "admin") return true;
        if (await groupService.IsLeaderOfGroupAsync(activeUser.Id, ev.GroupId)) return true;

        var membership = await membershipService.GetByUserAndGroupAsync(activeUser.Id, ev.GroupId);
        return membership?.Status == "accepted";
    }

    public async Task<EventPostModel?> CreatePostAsync(
        Guid eventId,
        string body,
        byte[]? imageBytes = null,
        string? imageContentType = null,
        string? imageExt = null)
    {
        if (string.IsNullOrWhiteSpace(body) && imageBytes is null)
            throw new InvalidOperationException("Debes agregar texto o una imagen como evidencia.");

        var user = await currentUser.RequireActiveUserAsync();

        var ev = await eventService.GetByIdAsync(eventId);
        if (ev is null) throw new InvalidOperationException("Evento no encontrado.");
        if (ev.EndAt >= DateTime.UtcNow && ev.Status != "canceled")
            throw new InvalidOperationException("La evidencia solo puede publicarse cuando el evento haya finalizado.");

        var isAdmin = user.Role == "admin";
        var isLeader = await groupService.IsLeaderOfGroupAsync(user.Id, ev.GroupId);
        var membership = await membershipService.GetByUserAndGroupAsync(user.Id, ev.GroupId);
        var isMember = membership?.Status == "accepted";
        if (!isAdmin && !isLeader && !isMember)
            throw new InvalidOperationException("Solo miembros del grupo pueden publicar evidencia del evento.");

        string? imageUrl = null;
        if (imageBytes is not null && imageContentType is not null && imageExt is not null)
        {
            if (!StorageService.AllowedMimeTypes.Contains(imageContentType))
                throw new InvalidOperationException("Formato de imagen no permitido.");
            if (imageBytes.Length > StorageService.MaxBytes)
                throw new InvalidOperationException("La imagen supera el límite de 5 MB.");

            var fileName = $"{Guid.NewGuid():N}.{imageExt}";
            using var stream = new MemoryStream(imageBytes);
            imageUrl = await storageService.UploadAsync($"event-posts/{eventId}", fileName, stream, imageContentType);
            if (imageUrl is null)
                throw new InvalidOperationException("No se pudo subir la imagen.");
        }

        var post = await eventPostService.CreateAsync(eventId, user.Id, string.IsNullOrWhiteSpace(body) ? "Evidencia del evento" : body, imageUrl);
        return MapPost(post);
    }

    public async Task<bool> DeletePostAsync(Guid eventId, Guid postId)
    {
        var user = await currentUser.RequireActiveUserAsync();
        return await eventPostService.DeleteAsync(postId, user.Id, user.Role == "admin");
    }

    private static EventPostModel MapPost(EventPost p) => new(
        p.Id, p.EventId, p.AuthorUserId,
        p.Author?.DisplayName ?? p.Author?.Email ?? "Usuario",
        p.Author?.AvatarUrl,
        p.Body,
        p.ImageUrl,
        p.CreatedAt);
}

// ─── Leadership Request API Service ───────────────────────────────────────────

public class LeadershipRequestApiService(
    LeadershipRequestService leadershipRequestService,
    NotificationService notificationService,
    CurrentUserService currentUser)
{
    public async Task<List<LeadershipRequestModel>> GetMineAsync()
        => await DbSafe.TryAsync(async () =>
        {
            var userId = await currentUser.GetUserIdAsync();
            if (userId == Guid.Empty) return [];
            return (await leadershipRequestService.GetByUserAsync(userId)).Select(MapRequest).ToList();
        }, []);

    public async Task<List<LeadershipRequestModel>> GetAllPendingAsync()
        => await DbSafe.TryAsync(async () =>
            (await leadershipRequestService.GetAllPendingAsync()).Select(MapRequest).ToList(), []);

    public async Task<bool> CanRequestAsync(Guid groupId)
    {
        var user = await currentUser.GetUserAsync();
        if (!UserService.IsActive(user)) return false;
        var userId = user!.Id;
        return await leadershipRequestService.CanRequestAsync(groupId, userId);
    }

    public async Task<LeadershipRequestModel?> CreateAsync(StudentGroupsHub.DTOs.Requests.CreateLeadershipRequest request)
    {
        var userId = await currentUser.GetActiveUserIdAsync();

        var req = await leadershipRequestService.CreateAsync(
            request.GroupId, userId, request.ContactEmail, request.Reason);
        return MapRequest(req);
    }

    public async Task<LeadershipRequestModel?> ApproveAsync(Guid id, string? notes = null)
    {
        var reviewerId = await currentUser.GetActiveUserIdAsync();
        var req = await leadershipRequestService.ApproveAsync(id, reviewerId, notes);
        if (req is null) return null;

        await notificationService.CreateAsync(req.RequestedByUserId,
            "leadership_request_approved",
            $"Tu solicitud de liderazgo en {req.Group?.Name} fue aprobada.",
            notes,
            $"/groups/{req.Group?.Slug}",
            req.GroupId, "group");

        return MapRequest(req);
    }

    public async Task<LeadershipRequestModel?> RejectAsync(Guid id, string? notes = null)
    {
        var reviewerId = await currentUser.GetActiveUserIdAsync();
        var req = await leadershipRequestService.RejectAsync(id, reviewerId, notes);
        if (req is null) return null;

        await notificationService.CreateAsync(req.RequestedByUserId,
            "leadership_request_rejected",
            $"Tu solicitud de liderazgo en {req.Group?.Name} no fue aceptada.",
            notes,
            $"/groups/{req.Group?.Slug}",
            req.GroupId, "group");

        return MapRequest(req);
    }

    private static LeadershipRequestModel MapRequest(LeadershipRequest r) => new(
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
                MapEvents(data.UpcomingEvents),
                MapLeadershipRequests(data.LeadershipRequests ?? []));
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
                data.TotalUsers, data.ActiveStudents, data.StudentsThisMonth,
                data.TotalGroups, data.ActiveGroups,
                data.PendingGroupRequests, data.PendingLeadershipRequests,
                data.TotalEvents, data.EventsThisMonth, data.EventsPreviousMonth,
                data.ProcessedMemberships, data.MembershipApprovalRate,
                data.AverageMembershipResponseHours, data.TotalMemberships,
                data.TotalParticipations, data.AverageParticipationsPerEvent);
        }, null);

    private static List<GroupModel> MapGroups(List<GroupResponse> groups)
        => groups.Select(g => new GroupModel(
            g.Id, g.Slug, g.Name, g.Description, g.Category,
            g.LogoUrl, g.BannerUrl, g.ContactEmail, g.ContactInfo,
            g.Status, g.MemberCount, g.CreatedAt, g.UpdatedAt)).ToList();

    private static List<MembershipModel> MapMemberships(List<MembershipResponse> memberships)
        => memberships.Select(m => new MembershipModel(
            m.MembershipId, m.UserId, m.GroupId, m.Email, m.DisplayName,
            m.AvatarUrl, m.Status, m.RequestedAt, m.RespondedAt)).ToList();

    private static List<EventModel> MapEvents(List<EventResponse> events)
        => events.Select(e => new EventModel(
            e.Id, e.GroupId, e.GroupName, e.Title, e.Description,
            e.Location, e.BannerUrl, e.StartAt, e.EndAt, e.Timezone,
            e.Capacity, e.RsvpCount, e.Status, e.Visibility, e.CreatedAt)).ToList();

    private static List<LeadershipRequestModel> MapLeadershipRequests(List<LeadershipRequestResponse> requests)
        => requests.Select(r => new LeadershipRequestModel(
            r.Id, r.GroupId, r.GroupName, r.GroupSlug, r.RequestedByUserId,
            r.RequestedByDisplayName, r.ContactEmail, r.Reason, r.Status,
            r.DecisionNotes, r.CreatedAt, r.ReviewedAt)).ToList();
}

// ─── Admin API Service ────────────────────────────────────────────────────────

public class AdminApiService(
    UserService userService,
    CurrentUserService currentUser,
    DashboardService dashboardService,
    GroupService groupService,
    GroupSeasonService groupSeasonService,
    NotificationService notificationService,
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
            data.TotalUsers, data.ActiveStudents, data.StudentsThisMonth,
            data.TotalGroups, data.ActiveGroups,
            data.PendingGroupRequests, data.PendingLeadershipRequests,
            data.TotalEvents, data.EventsThisMonth, data.EventsPreviousMonth,
            data.ProcessedMemberships, data.MembershipApprovalRate,
            data.AverageMembershipResponseHours, data.TotalMemberships,
            data.TotalParticipations, data.AverageParticipationsPerEvent);
    }

    public async Task<PaginatedResponse<GroupModel>?> GetAllGroupsAdminAsync(
        string? search = null, string? status = null, int page = 1, int pageSize = 20)
    {
        return await DbSafe.TryAsync(async () =>
        {
            var (items, total) = await groupService.GetAllAdminAsync(search, status, page, pageSize);
            var counts = await groupService.GetMemberCountsAsync(items.Select(g => g.Id));
            var responses = items.Select(g => new GroupModel(
                g.Id, g.Slug, g.Name, g.Description, g.Category,
                g.LogoUrl, g.BannerUrl, g.ContactEmail, g.ContactInfo,
                g.Status, counts.GetValueOrDefault(g.Id), g.CreatedAt, g.UpdatedAt)).ToList();
            return new PaginatedResponse<GroupModel>(
                responses, page, pageSize, total,
                (int)Math.Ceiling((double)total / pageSize));
        }, null);
    }

    public async Task<List<GroupModel>> GetActiveGroupsForSeasonResetAsync()
    {
        using var db = dbFactory.CreateDbContext();
        var groups = await db.Groups
            .Where(g => g.Status == "active")
            .OrderBy(g => g.Name)
            .ToListAsync();
        var counts = await groupService.GetMemberCountsAsync(groups.Select(g => g.Id));
        return groups.Select(g => new GroupModel(
            g.Id, g.Slug, g.Name, g.Description, g.Category,
            g.LogoUrl, g.BannerUrl, g.ContactEmail, g.ContactInfo,
            g.Status, counts.GetValueOrDefault(g.Id), g.CreatedAt, g.UpdatedAt)).ToList();
    }

    public async Task<bool> SetGroupStatusAsync(Guid groupId, string status)
    {
        await currentUser.RequireActiveUserAsync();
        return await DbSafe.TryAsync(
            () => groupService.SetStatusAsync(groupId, status), false);
    }

    public async Task<UserModel?> SetRoleAsync(Guid userId, string role, Guid? groupId = null)
    {
        await currentUser.RequireActiveUserAsync();

        User? user = role switch
        {
            "group_leader" when groupId.HasValue => await userService.AssignLeaderAsync(userId, groupId.Value),
            "group_leader" => throw new InvalidOperationException("Debes seleccionar un grupo para asignar liderazgo."),
            "student" => await userService.DemoteToStudentAsync(userId),
            _ => await SetUserRoleAsync(userId, role)
        };

        return user is null ? null : MapUser(user);
    }

    public async Task<UserModel?> SetStatusAsync(Guid userId, string status)
    {
        await currentUser.RequireActiveUserAsync();
        using var db = dbFactory.CreateDbContext();
        var user = await db.Users.FindAsync(userId);
        if (user is null) return null;
        user.Status = status; user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapUser(user);
    }

    public async Task ResetSeasonAsync(IEnumerable<Guid> skipGroupIds)
    {
        await currentUser.RequireActiveUserAsync();
        using var db = dbFactory.CreateDbContext();
        var skipIds = skipGroupIds.Distinct().ToHashSet();
        var targetGroups = await db.Groups
            .Where(g => g.Status == "active" && !skipIds.Contains(g.Id))
            .Select(g => new { g.Id, g.Name, g.Slug })
            .ToListAsync();
        var targetIds = targetGroups.Select(g => g.Id).ToList();
        if (targetIds.Count == 0) return;

        await NotifyAndResetAsync(targetIds);
    }

    public async Task ResetGroupSeasonAsync(Guid groupId)
    {
        await currentUser.RequireActiveUserAsync();
        await NotifyAndResetAsync([groupId]);
    }

    private async Task<User?> SetUserRoleAsync(Guid userId, string role)
    {
        using var db = dbFactory.CreateDbContext();
        var user = await db.Users.FindAsync(userId);
        if (user is null) return null;
        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return user;
    }

    private async Task NotifyAndResetAsync(IEnumerable<Guid> groupIds)
    {
        using var db = dbFactory.CreateDbContext();
        var targetIds = groupIds.Distinct().ToList();
        if (targetIds.Count == 0) return;

        var affectedLeaders = await db.RoleAssignments
            .Include(r => r.User)
            .Include(r => r.Group)
            .Where(r => targetIds.Contains(r.GroupId) && r.PermissionRole == "leader")
            .ToListAsync();

        var affectedMemberships = await db.Memberships
            .Include(m => m.Group)
            .Where(m => targetIds.Contains(m.GroupId) && (m.Status == "accepted" || m.Status == "pending"))
            .ToListAsync();

        await groupSeasonService.ResetSeasonAsync(targetIds);

        foreach (var leader in affectedLeaders)
        {
            if (leader.UserId == Guid.Empty) continue;
            await notificationService.CreateAsync(leader.UserId,
                "group_season_reset",
                $"Tu periodo como líder de {leader.Group?.Name} ha concluido.",
                "El grupo fue reiniciado para el nuevo ciclo y requerirá nuevas solicitudes de liderazgo.",
                $"/groups/{leader.Group?.Slug}",
                leader.GroupId, "group");
        }

        foreach (var membership in affectedMemberships)
        {
            await notificationService.CreateAsync(membership.UserId,
                "group_membership_reset",
                $"{membership.Group?.Name} reinició su ciclo.",
                membership.Status == "accepted"
                    ? "Tu membresía fue cerrada y debes volver a unirte en el nuevo ciclo."
                    : "Tu solicitud fue cerrada por el reinicio anual del grupo.",
                $"/groups/{membership.Group?.Slug}",
                membership.GroupId, "group");
        }
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
        await currentUser.RequireActiveUserAsync();
        var term = await termService.CreateTermAsync(groupId, label, startDate, endDate, notes);
        return MapTerm(term);
    }

    public async Task<TermMemberModel?> AddMemberAsync(
        Guid termId, string displayName, string roleLabel, int sortOrder = 0)
    {
        await currentUser.RequireActiveUserAsync();
        var m = await termService.AddMemberAsync(termId, displayName, roleLabel, sortOrder);
        return MapMember(m);
    }

    public async Task<bool> RemoveMemberAsync(Guid memberId)
    {
        await currentUser.RequireActiveUserAsync();
        return await termService.RemoveMemberAsync(memberId);
    }

    public async Task<bool> DeleteTermAsync(Guid termId)
    {
        await currentUser.RequireActiveUserAsync();
        return await termService.DeleteTermAsync(termId);
    }

    private static GroupTermModel MapTerm(GroupTerm t) => new(
        t.Id, t.GroupId, t.Label, t.StartDate, t.EndDate,
        t.Status, t.Notes, t.CreatedAt,
        t.Members.Select(MapMember).ToList(),
        GroupName:     t.Group?.Name,
        GroupSlug:     t.Group?.Slug,
        GroupCategory: t.Group?.Category);

    private static TermMemberModel MapMember(TermMember m) => new(
        m.Id, m.TermId, m.UserId, m.DisplayName,
        m.RoleLabel, m.SortOrder, m.AvatarUrl);
}

// ─── Group Post API Service ───────────────────────────────────────────────────

public class GroupPostApiService(
    GroupPostService postService,
    GroupService groupService,
    MembershipService membershipService,
    CurrentUserService currentUser,
    StorageService storageService)
{
    public async Task<List<GroupPostModel>> GetPostsAsync(Guid groupId)
        => await DbSafe.TryAsync(async () =>
        {
            var posts = await postService.GetForGroupAsync(groupId);
            return posts.Select(MapPost).ToList();
        }, []);

    public async Task<bool> CanPostAsync(Guid groupId)
    {
        var user = await currentUser.GetUserAsync();
        if (!UserService.IsActive(user)) return false;
        var activeUser = user!;

        if (await groupService.IsLeaderOfGroupAsync(activeUser.Id, groupId)) return true;

        var membership = await membershipService.GetByUserAndGroupAsync(activeUser.Id, groupId);
        if (membership?.Status != "accepted") return false;

        if (activeUser.Role == "admin") return true;
        return await postService.IsAuthorizedToPostAsync(groupId, activeUser.Id);
    }

    public async Task<GroupPostModel?> CreatePostAsync(
        Guid groupId, string body, byte[]? imageBytes = null,
        string? imageContentType = null, string? imageExt = null)
    {
        if (string.IsNullOrWhiteSpace(body))
            throw new InvalidOperationException("La publicación no puede estar vacía.");

        var user = await currentUser.RequireActiveUserAsync();

        var isLeader = await groupService.IsLeaderOfGroupAsync(user.Id, groupId);
        var membership = await membershipService.GetByUserAndGroupAsync(user.Id, groupId);
        var isAcceptedMember = membership?.Status == "accepted";

        var canPost = isLeader
            || (isAcceptedMember && (user.Role == "admin"
                || await postService.IsAuthorizedToPostAsync(groupId, user.Id)));
        if (!canPost)
            throw new InvalidOperationException("No tienes permiso para publicar en este grupo.");

        string? imageUrl = null;
        if (imageBytes is not null && imageContentType is not null && imageExt is not null)
        {
            if (!StorageService.AllowedMimeTypes.Contains(imageContentType))
                throw new InvalidOperationException("Formato de imagen no permitido.");
            if (imageBytes.Length > StorageService.MaxBytes)
                throw new InvalidOperationException("La imagen supera el límite de 5 MB.");

            var fileName = $"{Guid.NewGuid():N}.{imageExt}";
            using var stream = new MemoryStream(imageBytes);
            imageUrl = await storageService.UploadAsync(
                $"group-posts/{groupId}", fileName, stream, imageContentType);
        }

        var post = await postService.CreateAsync(groupId, user.Id, body, imageUrl);
        return MapPost(post);
    }

    public async Task<bool> DeletePostAsync(Guid groupId, Guid postId)
    {
        var user = await currentUser.RequireActiveUserAsync();
        return await postService.DeleteAsync(postId, user.Id, user.Role == "admin");
    }

    /// <summary>Returns accepted members of the group with a flag for whether they can post.</summary>
    public async Task<List<GroupPostAuthorModel>> GetMembersWithPostFlagAsync(Guid groupId)
        => await DbSafe.TryAsync(async () =>
        {
            var members  = await membershipService.GetAcceptedAsync(groupId);
            var authors  = await postService.GetAuthorizedUsersAsync(groupId);
            var authorIds = authors.Select(a => a.UserId).ToHashSet();
            // Return ALL accepted members; IsAuthor flag drives the toggle UI
            return members
                .Where(m => m.User is not null)
                .Select(m => new GroupPostAuthorModel(
                    m.UserId,
                    m.User!.DisplayName ?? m.User.Email,
                    m.User.AvatarUrl))
                .ToList();
        }, []);

    public async Task<HashSet<Guid>> GetAuthorIdsAsync(Guid groupId)
        => await DbSafe.TryAsync(async () =>
        {
            var authors = await postService.GetAuthorizedUsersAsync(groupId);
            return authors.Select(a => a.UserId).ToHashSet();
        }, []);

    public async Task SetAuthorAsync(Guid groupId, Guid userId, bool allowed)
        => await postService.SetAuthorAsync(groupId, userId, allowed);

    private static GroupPostModel MapPost(GroupPost p) => new(
        p.Id, p.GroupId, p.AuthorUserId,
        p.Author?.DisplayName ?? p.Author?.Email ?? "Usuario",
        p.Author?.AvatarUrl,
        p.Body, p.ImageUrl, p.CreatedAt);
}

// ─── Mis Grupos API Service ───────────────────────────────────────────────────

public class MisGruposApiService(
    GroupService groupService,
    CurrentUserService currentUser)
{
    public async Task<MyGroupsModel?> GetMyGroupsAsync()
        => await DbSafe.TryAsync(async () =>
        {
            var userId = await currentUser.GetUserIdAsync();
            if (userId == Guid.Empty) return null;

            var memberTask = groupService.GetJoinedGroupsAsync(userId);
            var leaderTask = groupService.GetLedGroupsAsync(userId);
            await Task.WhenAll(memberTask, leaderTask);

            var counts = await groupService.GetMemberCountsAsync(
                memberTask.Result.Select(g => g.Id)
                    .Union(leaderTask.Result.Select(g => g.Id)));

            var memberModels = memberTask.Result.Select(g =>
                MapGroup(g, counts.GetValueOrDefault(g.Id))).ToList();
            var leaderModels = leaderTask.Result.Select(g =>
                MapGroup(g, counts.GetValueOrDefault(g.Id))).ToList();

            return new MyGroupsModel(memberModels, leaderModels);
        }, null);

    private static GroupModel MapGroup(Group g, int count) => new(
        g.Id, g.Slug, g.Name, g.Description, g.Category,
        g.LogoUrl, g.BannerUrl, g.ContactEmail, g.ContactInfo,
        g.Status, count, g.CreatedAt, g.UpdatedAt);
}

// ─── Calendar API Service ─────────────────────────────────────────────────────

public class CalendarApiService(
    EventService eventService,
    GroupService groupService,
    CurrentUserService currentUser)
{
    /// <summary>
    /// Returns three months of calendar data (prev, current, next) pre-loaded.
    /// Each CalendarMonthModel contains only days that have events.
    /// Two layers: myGroupEvents (purple) and allEvents (gold, not in my groups).
    /// </summary>
    public async Task<List<CalendarMonthModel>> GetCalendarAsync(int year, int month)
        => await DbSafe.TryAsync(async () =>
        {
            var userId      = await currentUser.GetUserIdAsync();
            var myGroupIds  = userId != Guid.Empty
                ? (await groupService.GetJoinedGroupIdsAsync(userId)).ToHashSet()
                : new HashSet<Guid>();

            // Pre-load prev / current / next month in parallel
            var months = new[] { -1, 0, 1 };
            var tasks  = months.Select(offset =>
            {
                var d = new DateTime(year, month, 1).AddMonths(offset);
                return eventService.GetForMonthAsync(d.Year, d.Month);
            }).ToList();
            await Task.WhenAll(tasks);

            var result = new List<CalendarMonthModel>();
            for (int i = 0; i < 3; i++)
            {
                var d      = new DateTime(year, month, 1).AddMonths(i - 1);
                var events = tasks[i].Result;

                // Separate into my-group events vs all-other events
                var myGroupEvents = events.Where(e => myGroupIds.Contains(e.GroupId)).ToList();
                var otherEvents   = events.Where(e => !myGroupIds.Contains(e.GroupId)).ToList();

                // Get RSVP counts (batch — one query per month)
                var rsvpTasks = events.Select(e =>
                    eventService.GetRsvpCountAsync(e.Id)).ToList();
                await Task.WhenAll(rsvpTasks);

                var rsvpCounts = events.Zip(rsvpTasks,
                    (e, t) => (e.Id, Count: t.Result))
                    .ToDictionary(x => x.Id, x => x.Count);

                EventModel MapEv(Event e) => new(
                    e.Id, e.GroupId, e.Group?.Name ?? "",
                    e.Title, e.Description, e.Location, e.BannerUrl,
                    e.StartAt, e.EndAt, e.Timezone,
                    e.Capacity, rsvpCounts.GetValueOrDefault(e.Id),
                    e.Status, e.Visibility, e.CreatedAt);

                // Group by day
                var byDay = events.GroupBy(e => DateOnly.FromDateTime(e.StartAt.ToLocalTime()))
                    .Select(g =>
                    {
                        var dayMyGroup = g.Where(e => myGroupIds.Contains(e.GroupId))
                            .Select(MapEv).ToList();
                        var dayOther   = g.Where(e => !myGroupIds.Contains(e.GroupId))
                            .Select(MapEv).ToList();
                        return new CalendarDayModel(g.Key, dayMyGroup, dayOther);
                    })
                    .OrderBy(day => day.Date)
                    .ToList();

                result.Add(new CalendarMonthModel(d.Year, d.Month, byDay));
            }
            return result;
        }, []);
}

