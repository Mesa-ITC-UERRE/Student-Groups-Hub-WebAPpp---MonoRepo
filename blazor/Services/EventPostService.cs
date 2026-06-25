using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class EventPostService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<EventPost>> GetForEventAsync(Guid eventId, int take = 50)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.EventPosts
            .Include(p => p.Author)
            .Where(p => p.EventId == eventId)
            .OrderByDescending(p => p.CreatedAt)
            .Take(take)
            .ToListAsync();
    }

    public async Task<EventPost> CreateAsync(Guid eventId, Guid authorUserId, string body, string? imageUrl = null)
    {
        using var db = dbFactory.CreateDbContext();
        var post = new EventPost
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            AuthorUserId = authorUserId,
            Body = body.Trim(),
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        db.EventPosts.Add(post);
        await db.SaveChangesAsync();
        return await db.EventPosts.Include(p => p.Author).FirstAsync(p => p.Id == post.Id);
    }

    public async Task<Dictionary<Guid, List<string>>> GetImageUrlsForEventsAsync(IEnumerable<Guid> eventIds, int takePerEvent = 3)
    {
        using var db = dbFactory.CreateDbContext();
        var ids = eventIds.Distinct().ToList();
        if (ids.Count == 0) return [];

        var posts = await db.EventPosts
            .Where(p => ids.Contains(p.EventId) && p.ImageUrl != null)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return posts
            .GroupBy(p => p.EventId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(p => p.ImageUrl!)
                    .Take(takePerEvent)
                    .ToList());
    }

    public async Task<bool> DeleteAsync(Guid postId, Guid requestingUserId, bool isAdmin)
    {
        using var db = dbFactory.CreateDbContext();
        var post = await db.EventPosts.FindAsync(postId);
        if (post is null) return false;
        if (!isAdmin && post.AuthorUserId != requestingUserId) return false;
        db.EventPosts.Remove(post);
        await db.SaveChangesAsync();
        return true;
    }
}
