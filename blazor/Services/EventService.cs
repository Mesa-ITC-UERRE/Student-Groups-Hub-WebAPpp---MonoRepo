using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Requests;
using StudentGroupsHub.DTOs.Responses;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class EventService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<Event>> GetUpcomingAsync(string? search = null, Guid? groupId = null)
    {
        using var db = dbFactory.CreateDbContext();
        var q = db.Events.Include(e => e.Group)
            .Where(e => e.Status == "published" && e.StartAt >= DateTime.UtcNow)
            .AsQueryable();
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

    public async Task<Event?> GetByIdAsync(Guid id)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.Events.Include(e => e.Group).FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Event> CreateAsync(Guid groupId, Guid createdByUserId, CreateEventRequest req)
    {
        using var db = dbFactory.CreateDbContext();
        if (req.EndAt <= req.StartAt)
            throw new InvalidOperationException("La fecha de fin debe ser posterior a la de inicio.");
        var ev = new Event
        {
            Id = Guid.NewGuid(), GroupId = groupId, CreatedByUserId = createdByUserId,
            Title = req.Title, Description = req.Description, Location = req.Location,
            StartAt = req.StartAt.ToUniversalTime(), EndAt = req.EndAt.ToUniversalTime(),
            Capacity = req.Capacity, Status = req.Status, Visibility = req.Visibility,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow,
        };
        db.Events.Add(ev);
        await db.SaveChangesAsync();
        return ev;
    }

    public async Task<Event?> UpdateAsync(Guid eventId, UpdateEventRequest req)
    {
        using var db = dbFactory.CreateDbContext();
        var ev = await db.Events.FindAsync(eventId);
        if (ev is null) return null;
        if (req.Title      is not null) ev.Title      = req.Title;
        if (req.Description is not null) ev.Description = req.Description;
        if (req.Location   is not null) ev.Location   = req.Location;
        if (req.StartAt    is not null) ev.StartAt    = req.StartAt.Value.ToUniversalTime();
        if (req.EndAt      is not null) ev.EndAt      = req.EndAt.Value.ToUniversalTime();
        if (req.Capacity   is not null) ev.Capacity   = req.Capacity;
        if (req.Status     is not null) ev.Status     = req.Status;
        if (req.Visibility is not null) ev.Visibility = req.Visibility;
        ev.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return ev;
    }

    public async Task<bool> CancelAsync(Guid eventId)
    {
        using var db = dbFactory.CreateDbContext();
        var ev = await db.Events.FindAsync(eventId);
        if (ev is null) return false;
        ev.Status = "canceled"; ev.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<EventParticipation?> UpsertRsvpAsync(Guid eventId, Guid userId, string status)
    {
        using var db = dbFactory.CreateDbContext();
        var ev = await db.Events.FindAsync(eventId);
        if (ev is null) throw new KeyNotFoundException("Evento no encontrado.");
        if (ev.Status == "canceled")
            throw new InvalidOperationException("No puedes registrarte en un evento cancelado.");
        if (status == "going" && ev.Capacity.HasValue)
        {
            var cnt = await db.EventParticipations.CountAsync(p => p.EventId == eventId && p.Status == "going");
            if (cnt >= ev.Capacity.Value)
                throw new InvalidOperationException("No hay lugares disponibles para este evento.");
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
}
