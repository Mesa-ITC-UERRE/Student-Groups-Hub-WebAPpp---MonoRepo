using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Models;
using StudentGroupsHub.Services;
using StudentGroupsHub.Tests.Infrastructure;

namespace StudentGroupsHub.Tests.Privacy;

[Collection(PostgreSqlCollection.Name)]
public sealed class EventAccessTests(PostgreSqlFixture database)
{
    [Fact]
    public async Task EventQueries_RespectVisibilityAndManagementScope()
    {
        await database.ResetDatabaseAsync();
        var dbFactory = database.CreateDbFactory();
        var creator = TestData.User();
        var outsider = TestData.User();
        var member = TestData.User();
        var leader = TestData.User("group_leader");
        var otherLeader = TestData.User("group_leader");
        var admin = TestData.User("admin");
        var group = TestData.Group("private-events");
        var otherGroup = TestData.Group("other-group");
        var publicEvent = TestData.Event(group.Id, creator.Id, "Public");
        var membersEvent = TestData.Event(group.Id, creator.Id, "Members");
        membersEvent.Visibility = "members";
        var draftEvent = TestData.Event(group.Id, creator.Id, "Draft");
        draftEvent.Status = "draft";

        await using (var db = await dbFactory.CreateDbContextAsync())
        {
            db.AddRange(creator, outsider, member, leader, otherLeader, admin, group, otherGroup);
            db.AddRange(publicEvent, membersEvent, draftEvent);
            db.Memberships.Add(new Membership
            {
                UserId = member.Id,
                GroupId = group.Id,
                Status = "accepted",
            });
            db.RoleAssignments.AddRange(
                new RoleAssignment
                {
                    UserId = leader.Id,
                    GroupId = group.Id,
                    PermissionRole = "leader",
                },
                new RoleAssignment
                {
                    UserId = otherLeader.Id,
                    GroupId = otherGroup.Id,
                    PermissionRole = "leader",
                });
            await db.SaveChangesAsync();
        }

        var service = new EventService(dbFactory);
        var publicUpcoming = await service.GetUpcomingAsync();
        var publicEvents = await service.GetPublicForGroupAsync(group.Id);
        var outsiderEvents = await service.GetAccessibleForGroupAsync(group.Id, outsider.Id);
        var memberEvents = await service.GetAccessibleForGroupAsync(group.Id, member.Id);
        var leaderEvents = await service.GetAccessibleForGroupAsync(group.Id, leader.Id);
        var adminEvents = await service.GetAccessibleForGroupAsync(group.Id, admin.Id);

        Assert.Equal([publicEvent.Id], publicUpcoming.Select(e => e.Id));
        Assert.Equal([publicEvent.Id], publicEvents.Select(e => e.Id));
        Assert.Equal([publicEvent.Id], outsiderEvents.Select(e => e.Id));
        Assert.Equal(2, memberEvents.Count);
        Assert.Contains(memberEvents, e => e.Id == publicEvent.Id);
        Assert.Contains(memberEvents, e => e.Id == membersEvent.Id);
        Assert.Equal(3, leaderEvents.Count);
        Assert.Equal(3, adminEvents.Count);
        Assert.Null(await service.GetPublicByIdAsync(membersEvent.Id));
        Assert.Null(await service.GetPublicByIdAsync(draftEvent.Id));
        Assert.True(await service.CanManageEventAsync(publicEvent.Id, leader.Id));
        Assert.True(await service.CanManageEventAsync(publicEvent.Id, admin.Id));
        Assert.False(await service.CanManageEventAsync(publicEvent.Id, otherLeader.Id));
        Assert.False(await service.CanManageEventAsync(publicEvent.Id, member.Id));
    }

    [Fact]
    public async Task Rsvp_RejectsOutsiderForMembersOnlyEvent()
    {
        await database.ResetDatabaseAsync();
        var dbFactory = database.CreateDbFactory();
        var creator = TestData.User();
        var outsider = TestData.User();
        var member = TestData.User();
        var group = TestData.Group("rsvp-private");
        var membersEvent = TestData.Event(group.Id, creator.Id, "Members");
        membersEvent.Visibility = "members";

        await using (var db = await dbFactory.CreateDbContextAsync())
        {
            db.AddRange(creator, outsider, member, group, membersEvent);
            db.Memberships.Add(new Membership
            {
                UserId = member.Id,
                GroupId = group.Id,
                Status = "accepted",
            });
            await db.SaveChangesAsync();
        }

        var service = new EventService(dbFactory);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.UpsertRsvpAsync(membersEvent.Id, outsider.Id, "going"));
        var accepted = await service.UpsertRsvpAsync(membersEvent.Id, member.Id, "going");

        Assert.NotNull(accepted);
        Assert.Equal(member.Id, accepted.UserId);
    }
}
