using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Services;
using StudentGroupsHub.Tests.Infrastructure;

namespace StudentGroupsHub.Tests.Security;

[Collection(PostgreSqlCollection.Name)]
public sealed class ValidationBoundaryTests(PostgreSqlFixture database)
{
    [Fact]
    public async Task Services_RejectUnknownStateValuesWithoutPersistingThem()
    {
        await database.ResetDatabaseAsync();
        var dbFactory = database.CreateDbFactory();
        var actor = TestData.User("admin");
        var attendee = TestData.User();
        var group = TestData.Group("validation");
        var ev = TestData.Event(group.Id, actor.Id);
        await using (var db = await dbFactory.CreateDbContextAsync())
        {
            db.AddRange(actor, attendee, group, ev);
            await db.SaveChangesAsync();
        }

        var userService = new UserService(dbFactory);
        var groupService = new GroupService(dbFactory);
        var eventService = new EventService(dbFactory);

        await Assert.ThrowsAsync<UserVisibleException>(
            () => userService.SetRoleAsync(actor.Id, attendee.Id, "superadmin"));
        await Assert.ThrowsAsync<UserVisibleException>(
            () => userService.SetStatusAsync(actor.Id, attendee.Id, "deleted"));
        await Assert.ThrowsAsync<UserVisibleException>(
            () => groupService.SetStatusAsync(group.Id, "archived"));
        await Assert.ThrowsAsync<UserVisibleException>(
            () => eventService.UpsertRsvpAsync(ev.Id, attendee.Id, "approved"));

        await using var verificationDb = await dbFactory.CreateDbContextAsync();
        var persistedUser = await verificationDb.Users.AsNoTracking()
            .SingleAsync(u => u.Id == attendee.Id);
        var persistedGroup = await verificationDb.Groups.AsNoTracking()
            .SingleAsync(g => g.Id == group.Id);

        Assert.Equal("student", persistedUser.Role);
        Assert.Equal("active", persistedUser.Status);
        Assert.Equal("active", persistedGroup.Status);
        Assert.Empty(await verificationDb.EventParticipations.ToListAsync());
    }
}
