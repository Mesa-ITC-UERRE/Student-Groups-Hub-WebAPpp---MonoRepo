using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.Models;
using StudentGroupsHub.Services;
using StudentGroupsHub.Tests.Infrastructure;

namespace StudentGroupsHub.Tests.Security;

[Collection(PostgreSqlCollection.Name)]
public sealed class UserAdministrationTests(PostgreSqlFixture database)
{
    [Theory]
    [InlineData("role")]
    [InlineData("status")]
    public async Task LastActiveAdmin_CannotBeRemoved(string operation)
    {
        await database.ResetDatabaseAsync();
        var dbFactory = database.CreateDbFactory();
        var actor = TestData.User("admin");
        var target = TestData.User("admin");
        target.Status = "inactive";
        await SeedUsersAsync(dbFactory, actor, target);
        var service = new UserService(dbFactory);

        var exception = operation == "role"
            ? await Assert.ThrowsAsync<UserVisibleException>(
                () => service.SetRoleAsync(actor.Id, actor.Id, "student"))
            : await Assert.ThrowsAsync<UserVisibleException>(
                () => service.SetStatusAsync(actor.Id, actor.Id, "inactive"));

        Assert.Contains("propi", exception.Message, StringComparison.OrdinalIgnoreCase);
        await using var db = await dbFactory.CreateDbContextAsync();
        var persisted = await db.Users.AsNoTracking().SingleAsync(u => u.Id == actor.Id);
        Assert.Equal("admin", persisted.Role);
        Assert.Equal("active", persisted.Status);
    }

    [Fact]
    public async Task Admin_CanDeactivateAnotherAdminWhenOneRemainsActive()
    {
        await database.ResetDatabaseAsync();
        var dbFactory = database.CreateDbFactory();
        var actor = TestData.User("admin");
        var target = TestData.User("admin");
        await SeedUsersAsync(dbFactory, actor, target);
        var service = new UserService(dbFactory);

        var updated = await service.SetStatusAsync(actor.Id, target.Id, "inactive");

        Assert.Equal("inactive", updated!.Status);
        await using var db = await dbFactory.CreateDbContextAsync();
        Assert.Single(await db.Users.Where(u =>
            u.Role == "admin" && u.Status == "active").ToListAsync());
    }

    [Fact]
    public async Task NonAdmin_CannotMutateUsers()
    {
        await database.ResetDatabaseAsync();
        var dbFactory = database.CreateDbFactory();
        var actor = TestData.User();
        var target = TestData.User();
        await SeedUsersAsync(dbFactory, actor, target);
        var service = new UserService(dbFactory);

        await Assert.ThrowsAsync<UserVisibleException>(
            () => service.SetRoleAsync(actor.Id, target.Id, "admin"));
        await Assert.ThrowsAsync<UserVisibleException>(
            () => service.SetStatusAsync(actor.Id, target.Id, "inactive"));
    }

    [Fact]
    public async Task LeaderPromotionAndDemotion_KeepAssignmentsAndRoleInSync()
    {
        await database.ResetDatabaseAsync();
        var dbFactory = database.CreateDbFactory();
        var actor = TestData.User("admin");
        var target = TestData.User();
        var group = TestData.Group("leadership-sync");
        await using (var db = await dbFactory.CreateDbContextAsync())
        {
            db.AddRange(actor, target, group);
            await db.SaveChangesAsync();
        }
        var service = new UserService(dbFactory);

        var promoted = await service.SetRoleAsync(
            actor.Id, target.Id, "group_leader", group.Id);

        await using (var db = await dbFactory.CreateDbContextAsync())
        {
            Assert.Equal("group_leader", promoted!.Role);
            Assert.True(await db.RoleAssignments.AnyAsync(r =>
                r.UserId == target.Id && r.GroupId == group.Id));
            Assert.True(await db.Memberships.AnyAsync(m =>
                m.UserId == target.Id && m.GroupId == group.Id && m.Status == "accepted"));
        }

        var demoted = await service.SetRoleAsync(actor.Id, target.Id, "student");

        await using (var db = await dbFactory.CreateDbContextAsync())
        {
            Assert.Equal("student", demoted!.Role);
            Assert.False(await db.RoleAssignments.AnyAsync(r => r.UserId == target.Id));
        }
    }

    private static async Task SeedUsersAsync(
        IDbContextFactory<AppDbContext> dbFactory,
        params User[] users)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        db.Users.AddRange(users);
        await db.SaveChangesAsync();
    }
}
