using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class GroupPostService(IDbContextFactory<AppDbContext> dbFactory)
{
    // ── Posts ─────────────────────────────────────────────────────────────────

    public async Task<List<GroupPost>> GetForGroupAsync(Guid groupId, int take = 50)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.GroupPosts
            .Include(p => p.Author)
            .Where(p => p.GroupId == groupId)
            .OrderByDescending(p => p.CreatedAt)
            .Take(take)
            .ToListAsync();
    }

    public async Task<GroupPost> CreateAsync(
        Guid groupId, Guid authorUserId, string body, string? imageUrl = null)
    {
        using var db = dbFactory.CreateDbContext();
        var post = new GroupPost
        {
            Id           = Guid.NewGuid(),
            GroupId      = groupId,
            AuthorUserId = authorUserId,
            Body         = body.Trim(),
            ImageUrl     = imageUrl,
            CreatedAt    = DateTime.UtcNow,
            UpdatedAt    = DateTime.UtcNow,
        };
        db.GroupPosts.Add(post);
        await db.SaveChangesAsync();
        // Re-load with Author navigation
        return await db.GroupPosts.Include(p => p.Author)
            .FirstAsync(p => p.Id == post.Id);
    }

    public async Task<bool> DeleteAsync(Guid postId, Guid requestingUserId, bool isAdmin)
    {
        using var db = dbFactory.CreateDbContext();
        var post = await db.GroupPosts.FindAsync(postId);
        if (post is null) return false;
        // Only the author or an admin may delete
        if (!isAdmin && post.AuthorUserId != requestingUserId) return false;
        db.GroupPosts.Remove(post);
        await db.SaveChangesAsync();
        return true;
    }

    // ── Post authors (who can post besides leaders) ───────────────────────────

    public async Task<List<GroupPostAuthor>> GetAuthorizedUsersAsync(Guid groupId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.GroupPostAuthors
            .Include(a => a.User)
            .Where(a => a.GroupId == groupId)
            .ToListAsync();
    }

    public async Task<bool> IsAuthorizedToPostAsync(Guid groupId, Guid userId)
    {
        using var db = dbFactory.CreateDbContext();
        return await db.GroupPostAuthors
            .AnyAsync(a => a.GroupId == groupId && a.UserId == userId);
    }

    public async Task SetAuthorAsync(Guid groupId, Guid userId, bool allowed)
    {
        using var db = dbFactory.CreateDbContext();
        var existing = await db.GroupPostAuthors
            .FirstOrDefaultAsync(a => a.GroupId == groupId && a.UserId == userId);

        if (allowed && existing is null)
        {
            db.GroupPostAuthors.Add(new GroupPostAuthor { GroupId = groupId, UserId = userId });
        }
        else if (!allowed && existing is not null)
        {
            db.GroupPostAuthors.Remove(existing);
        }
        await db.SaveChangesAsync();
    }
}
