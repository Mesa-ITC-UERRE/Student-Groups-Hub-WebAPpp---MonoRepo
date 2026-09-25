using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Requests;
using StudentGroupsHub.DTOs.Responses;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class EventService(IDbContextFactory<AppDbContext> dbFactory)
{
    private static readonly HashSet<string> AllowedStatuses = ["draft", "published", "canceled"];
    private static readonly HashSet<string> AllowedVisibilities = ["public", "members"];
    private static readonly HashSet<string> AllowedRsvpStatuses = ["going"];

    public async Task<List<Event>> GetUpcomingAsync(
        string? search = null,
        Guid? groupId = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null)
    {
        using var db = dbFactory.CreateDbContext();
        var from = fromUtc ?? DateTime.UtcNow;
        var q = db.Events.Include(e => e.Group)
            .Where(e => e.Status == "published"
                     && e.Visibility == "public"
                     && e.StartAt >= from)
            .AsQueryable();
        if (toUtc.HasValue) q = q.Where(e => e.StartAt <= toUtc.Value);
        if (groupId.HasValue) q = q.Where(e => e.GroupId == groupId.Value);
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(e => EF.Functions.ILike(e.Title, $"%{search}%"));
        return await q.OrderBy(e => e.StartAt).Take(50).ToListAsync();
    }

    public async Task<List<Event>> GetForGroupAsync(Guid groupId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Events
            .Where(e => e.GroupId == groupId)
            .OrderByDescending(e => e.StartAt)
            .ToListAsync();
    }

    public async Task<List<Event>> GetPublicForGroupAsync(Guid groupId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Events.Include(e => e.Group)
            .Where(e => e.GroupId == groupId
                     && e.Status == "published"
                     && e.Visibility == "public")
            .OrderByDescending(e => e.StartAt)
            .ToListAsync();
    }

    public async Task<List<Event>> GetAccessibleForGroupAsync(Guid groupId, Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        return await ApplyUserAccess(db.Events.Include(e => e.Group), db, userId)
            .Where(e => e.GroupId == groupId)
            .OrderByDescending(e => e.StartAt)
            .ToListAsync();
    }

    public async Task<Event?> GetByIdAsync(Guid id)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Events.Include(e => e.Group).FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Event?> GetPublicByIdAsync(Guid id)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Events.Include(e => e.Group).FirstOrDefaultAsync(e =>
            e.Id == id && e.Status == "published" && e.Visibility == "public");
    }

    public async Task<Event?> GetAccessibleByIdAsync(Guid id, Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        return await ApplyUserAccess(db.Events.Include(e => e.Group), db, userId)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> CanManageEventAsync(Guid eventId, Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        var groupId = await db.Events
            .Where(e => e.Id == eventId)
            .Select(e => (Guid?)e.GroupId)
            .SingleOrDefaultAsync();
        return groupId.HasValue && await CanManageGroupAsync(db, groupId.Value, userId);
    }

    public async Task<List<Event>> GetAllPublishedAsync()
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Events
            .Include(e => e.Group)
            .Where(e => e.Status == "published" && e.Visibility == "public")
            .OrderByDescending(e => e.StartAt)
            .ToListAsync();
    }

    public async Task<Event> CreateAsync(Guid groupId, Guid createdByUserId, CreateEventRequest req)
    {
        using var db = dbFactory.CreateDbContext();
        ValidateEvent(req.Title, req.Location, req.StartAt, req.EndAt, req.Capacity, req.Status, req.Visibility);
        var ev = new Event
        {
            Id = Guid.NewGuid(), GroupId = groupId, CreatedByUserId = createdByUserId,
            Title = req.Title.Trim(), Description = NormalizeOptional(req.Description), Location = NormalizeOptional(req.Location),
            StartAt = req.StartAt.ToUniversalTime(), EndAt = req.EndAt.ToUniversalTime(),
            Capacity = req.Capacity, Status = req.Status, Visibility = req.Visibility,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow,
        };
        db.Events.Add(ev);
        await db.SaveChangesAsync();
        return ev;
    }

    public async Task<Event?> UpdateAsync(Guid groupId, Guid eventId, UpdateEventRequest req)
    {
        using var db = dbFactory.CreateDbContext();
        var ev = await db.Events.FirstOrDefaultAsync(e => e.Id == eventId && e.GroupId == groupId);
        if (ev is null) return null;

        var title = req.Title?.Trim() ?? ev.Title;
        var description = req.Description is null ? ev.Description : NormalizeOptional(req.Description);
        var location = req.Location is null ? ev.Location : NormalizeOptional(req.Location);
        var startAt = req.StartAt?.ToUniversalTime() ?? ev.StartAt;
        var endAt = req.EndAt?.ToUniversalTime() ?? ev.EndAt;
        var capacity = req.ClearCapacity ? null : req.Capacity ?? ev.Capacity;
        var status = req.Status ?? ev.Status;
        var visibility = req.Visibility ?? ev.Visibility;

        ValidateEvent(title, location, startAt, endAt, capacity, status, visibility);

        ev.Title = title;
        ev.Description = description;
        ev.Location = location;
        if (req.BannerUrl  is not null) ev.BannerUrl  = req.BannerUrl;
        ev.StartAt = startAt;
        ev.EndAt = endAt;
        ev.Capacity = capacity;
        ev.Status = status;
        ev.Visibility = visibility;
        ev.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return ev;
    }

    public async Task<bool> CancelAsync(Guid groupId, Guid eventId)
    {
        using var db = dbFactory.CreateDbContext();
        var ev = await db.Events.FirstOrDefaultAsync(e => e.Id == eventId && e.GroupId == groupId);
        if (ev is null) return false;
        ev.Status = "canceled"; ev.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<EventParticipation?> UpsertRsvpAsync(Guid eventId, Guid userId, string status)
    {
        if (!AllowedRsvpStatuses.Contains(status))
            throw new UserVisibleException("El estado de asistencia no es válido.");

        using var db = dbFactory.CreateDbContext();
        var ev = await db.Events.FindAsync(eventId);
        if (ev is null) throw new KeyNotFoundException("Evento no encontrado.");
        if (!await CanAccessEventAsync(db, ev, userId))
            throw new KeyNotFoundException("Evento no encontrado.");
        if (ev.Status == "canceled")
            throw new UserVisibleException("No puedes registrarte en un evento cancelado.");
        if (status == "going" && ev.Capacity.HasValue)
        {
            var cnt = await db.EventParticipations.CountAsync(p => p.EventId == eventId && p.Status == "going");
            if (cnt >= ev.Capacity.Value)
                throw new UserVisibleException("No hay lugares disponibles para este evento.");
        }
        var existing = await db.EventParticipations
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId);
        if (existing is not null)
        {
            existing.Status = status;
        }
        else
        {
            existing = new EventParticipation
            {
                Id = Guid.NewGuid(), EventId = eventId, UserId = userId,
                Status = status, RegisteredAt = DateTime.UtcNow,
            };
            db.EventParticipations.Add(existing);
        }
        await db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> RemoveRsvpAsync(Guid eventId, Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        var p = await db.EventParticipations
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId);
        if (p is null) return false;
        db.EventParticipations.Remove(p);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<List<EventParticipation>> GetRsvpsAsync(Guid eventId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.EventParticipations.Include(p => p.User)
            .Where(p => p.EventId == eventId).ToListAsync();
    }

    public async Task<int> GetRsvpCountAsync(Guid eventId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.EventParticipations
            .CountAsync(p => p.EventId == eventId && p.Status == "going");
    }

    /// <summary>
    /// Returns all published events whose StartAt falls within the given calendar month.
    /// Optionally filtered to specific group IDs.
    /// </summary>
    public async Task<List<Event>> GetForMonthAsync(
        int year, int month, IEnumerable<Guid>? groupIds = null)
    {
        using var db = dbFactory.CreateDbContext();
        var from = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var to   = from.AddMonths(1);

        var q = db.Events
            .Include(e => e.Group)
            .Where(e => e.Status == "published"
                     && e.Visibility == "public"
                     && e.StartAt >= from
                     && e.StartAt < to)
            .AsQueryable();

        if (groupIds is not null)
        {
            var ids = groupIds.ToList();
            if (ids.Count > 0)
                q = q.Where(e => ids.Contains(e.GroupId));
        }

        return await q.OrderBy(e => e.StartAt).ToListAsync();
    }

    public async Task<EventResponse> ToResponseAsync(Event e)
    {
        var count = await GetRsvpCountAsync(e.Id);
        return ToResponse(e, count);
    }

    public static EventResponse ToResponse(Event e, int rsvpCount = 0) => new(
        e.Id, e.GroupId, e.Group?.Name ?? "",
        e.Title, e.Description, e.Location, e.BannerUrl,
        e.StartAt, e.EndAt, e.Timezone,
        e.Capacity, rsvpCount, e.Status, e.Visibility, e.CreatedAt);

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static IQueryable<Event> ApplyUserAccess(
        IQueryable<Event> query,
        AppDbContext db,
        Guid userId)
        => query.Where(e =>
            (e.Status == "published" && e.Visibility == "public")
            || db.Users.Any(u => u.Id == userId && u.Status == "active" && u.Role == "admin")
            || (db.Users.Any(u => u.Id == userId && u.Status == "active")
                && db.RoleAssignments.Any(r =>
                    r.UserId == userId
                    && r.GroupId == e.GroupId
                    && r.PermissionRole == "leader"))
            || (e.Status == "published"
                && e.Visibility == "members"
                && db.Users.Any(u => u.Id == userId && u.Status == "active")
                && db.Memberships.Any(m =>
                    m.UserId == userId
                    && m.GroupId == e.GroupId
                    && m.Status == "accepted")));

    private static async Task<bool> CanAccessEventAsync(
        AppDbContext db,
        Event ev,
        Guid userId)
    {
        if (ev.Status == "published" && ev.Visibility == "public") return true;
        if (await CanManageGroupAsync(db, ev.GroupId, userId)) return true;
        return ev.Status == "published"
            && ev.Visibility == "members"
            && await db.Memberships.AnyAsync(m =>
                m.UserId == userId
                && m.GroupId == ev.GroupId
                && m.Status == "accepted");
    }

    private static async Task<bool> CanManageGroupAsync(
        AppDbContext db,
        Guid groupId,
        Guid userId)
    {
        var activeRole = await db.Users
            .Where(u => u.Id == userId && u.Status == "active")
            .Select(u => u.Role)
            .SingleOrDefaultAsync();
        if (activeRole is null) return false;
        return activeRole == "admin" || await db.RoleAssignments.AnyAsync(r =>
            r.UserId == userId
            && r.GroupId == groupId
            && r.PermissionRole == "leader");
    }

    private static void ValidateEvent(
        string title,
        string? location,
        DateTime startAt,
        DateTime endAt,
        int? capacity,
        string status,
        string visibility)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new UserVisibleException("El título es requerido.");
        if (title.Trim().Length > 200)
            throw new UserVisibleException("El título no puede exceder 200 caracteres.");
        if (location?.Trim().Length > 300)
            throw new UserVisibleException("El lugar no puede exceder 300 caracteres.");
        if (endAt <= startAt)
            throw new UserVisibleException("La fecha de fin debe ser posterior a la de inicio.");
        if (capacity is <= 0)
            throw new UserVisibleException("La capacidad debe ser mayor que cero.");
        if (!AllowedStatuses.Contains(status))
            throw new UserVisibleException("El estado del evento no es válido.");
        if (!AllowedVisibilities.Contains(visibility))
            throw new UserVisibleException("La visibilidad del evento no es válida.");
    }
}
