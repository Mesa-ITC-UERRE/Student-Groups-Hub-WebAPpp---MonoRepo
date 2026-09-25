using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.DTOs.Requests;
using StudentGroupsHub.Services;
using StudentGroupsHub.Tests.Infrastructure;

namespace StudentGroupsHub.Tests.Security;

[Collection(PostgreSqlCollection.Name)]
public sealed class EventResourceAuthorizationTests(PostgreSqlFixture database)
{
    [Fact]
    public async Task UpdateAndCancel_RejectEventFromAnotherGroup()
    {
        await database.ResetDatabaseAsync();
        var dbFactory = database.CreateDbFactory();
        var actor = TestData.User("group_leader");
        var authorizedGroup = TestData.Group("authorized-group");
        var foreignGroup = TestData.Group("foreign-group");
        var foreignEvent = TestData.Event(foreignGroup.Id, actor.Id, "Original title");

        await using (var db = await dbFactory.CreateDbContextAsync())
        {
            db.AddRange(actor, authorizedGroup, foreignGroup, foreignEvent);
            await db.SaveChangesAsync();
        }

        var service = new EventService(dbFactory);
        var update = await service.UpdateAsync(
            authorizedGroup.Id,
            foreignEvent.Id,
            new UpdateEventRequest(
                "Changed title", null, null, null, null, null, null, null, null));
        var cancel = await service.CancelAsync(authorizedGroup.Id, foreignEvent.Id);

        await using var verificationDb = await dbFactory.CreateDbContextAsync();
        var persisted = await verificationDb.Events.AsNoTracking()
            .SingleAsync(e => e.Id == foreignEvent.Id);

        Assert.Null(update);
        Assert.False(cancel);
        Assert.Equal("Original title", persisted.Title);
        Assert.Equal("published", persisted.Status);
    }
}
