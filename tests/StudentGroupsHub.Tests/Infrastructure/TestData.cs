using StudentGroupsHub.Models;

namespace StudentGroupsHub.Tests.Infrastructure;

internal static class TestData
{
    public static User User(string role = "student", string status = "active") => new()
    {
        Id = Guid.NewGuid(),
        EntraOid = Guid.NewGuid().ToString("N"),
        Email = $"user-{Guid.NewGuid():N}@example.test",
        DisplayName = "Test User",
        Role = role,
        Status = status,
    };

    public static Group Group(string slug) => new()
    {
        Id = Guid.NewGuid(),
        Slug = slug,
        Name = slug,
        Status = "active",
    };

    public static Event Event(Guid groupId, Guid createdByUserId, string title = "Test event") => new()
    {
        Id = Guid.NewGuid(),
        GroupId = groupId,
        CreatedByUserId = createdByUserId,
        Title = title,
        StartAt = DateTime.UtcNow.AddDays(1),
        EndAt = DateTime.UtcNow.AddDays(1).AddHours(2),
        Status = "published",
        Visibility = "public",
    };
}
