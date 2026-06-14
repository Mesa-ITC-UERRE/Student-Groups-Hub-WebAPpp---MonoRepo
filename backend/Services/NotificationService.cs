using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Responses;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class NotificationService(AppDbContext db)
{
    public async Task<List<Notification>> GetForUserAsync(Guid userId)
        => await db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToListAsync();

    public async Task MarkReadAsync(Guid id, Guid userId)
    {
        var n = await db.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        if (n is null) return;
        n.Read = true;
        await db.SaveChangesAsync();
    }

    public async Task MarkAllReadAsync(Guid userId)
    {
        await db.Notifications
            .Where(n => n.UserId == userId && !n.Read)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.Read, true));
    }

    public async Task CreateAsync(Guid userId, string kind, string title,
        string? body = null, string? href = null,
        Guid? referenceId = null, string? referenceType = null)
    {
        db.Notifications.Add(new Notification
        {
            Id            = Guid.NewGuid(),
            UserId        = userId,
            Kind          = kind,
            Title         = title,
            Body          = body,
            Href          = href,
            Read          = false,
            ReferenceId   = referenceId,
            ReferenceType = referenceType,
            CreatedAt     = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();
    }

    public static NotificationResponse ToResponse(Notification n) => new(
        n.Id, n.Kind, n.Title, n.Body, n.Href, n.Read, n.CreatedAt);
}
