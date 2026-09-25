using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using StudentGroupsHub.Models;
using StudentGroupsHub.Services;
using StudentGroupsHub.Tests.Infrastructure;

namespace StudentGroupsHub.Tests.Safety;

[Collection(PostgreSqlCollection.Name)]
public sealed class GroupSeasonResetTests(PostgreSqlFixture database)
{
    [Fact]
    public async Task ResetSeason_ChangesOnlyExplicitlySelectedGroups()
    {
        await database.ResetDatabaseAsync();
        var dbFactory = database.CreateDbFactory();
        var actor = TestData.User("admin");
        var leader = TestData.User("group_leader");
        var selected = TestData.Group("selected");
        var untouched = TestData.Group("untouched");
        await using (var db = await dbFactory.CreateDbContextAsync())
        {
            db.AddRange(actor, leader, selected, untouched);
            db.RoleAssignments.AddRange(
                Leader(leader.Id, selected.Id),
                Leader(leader.Id, untouched.Id));
            db.Memberships.AddRange(
                Membership(leader.Id, selected.Id),
                Membership(leader.Id, untouched.Id));
            await db.SaveChangesAsync();
        }

        var service = new GroupSeasonService(
            dbFactory, NullLogger<GroupSeasonService>.Instance);

        var emptyResult = await service.ResetSeasonAsync([], actor.Id);
        var result = await service.ResetSeasonAsync([selected.Id], actor.Id);

        Assert.Equal(0, emptyResult.Groups);
        Assert.Equal(1, result.Groups);
        Assert.Equal(1, result.LeaderAssignments);
        Assert.Equal(1, result.Memberships);
        await using var verificationDb = await dbFactory.CreateDbContextAsync();
        Assert.False(await verificationDb.RoleAssignments.AnyAsync(r =>
            r.GroupId == selected.Id));
        Assert.True(await verificationDb.RoleAssignments.AnyAsync(r =>
            r.GroupId == untouched.Id));
        Assert.Equal("removed", await verificationDb.Memberships
            .Where(m => m.GroupId == selected.Id)
            .Select(m => m.Status)
            .SingleAsync());
        Assert.Equal("accepted", await verificationDb.Memberships
            .Where(m => m.GroupId == untouched.Id)
            .Select(m => m.Status)
            .SingleAsync());
    }

    private static RoleAssignment Leader(Guid userId, Guid groupId) => new()
    {
        UserId = userId,
        GroupId = groupId,
        PermissionRole = "leader",
    };

    private static Membership Membership(Guid userId, Guid groupId) => new()
    {
        UserId = userId,
        GroupId = groupId,
        Status = "accepted",
    };
}
