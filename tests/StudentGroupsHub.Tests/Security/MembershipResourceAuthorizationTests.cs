using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Models;
using StudentGroupsHub.Services;
using StudentGroupsHub.Tests.Infrastructure;

namespace StudentGroupsHub.Tests.Security;

[Collection(PostgreSqlCollection.Name)]
public sealed class MembershipResourceAuthorizationTests(PostgreSqlFixture database)
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Review_RejectsMembershipFromAnotherGroup(bool approve)
    {
        await database.ResetDatabaseAsync();
        var dbFactory = database.CreateDbFactory();
        var applicant = TestData.User();
        var authorizedGroup = TestData.Group("authorized-group");
        var foreignGroup = TestData.Group("foreign-group");
        var membership = new Membership
        {
            Id = Guid.NewGuid(),
            UserId = applicant.Id,
            GroupId = foreignGroup.Id,
            Status = "pending",
            RequestedAt = DateTime.UtcNow,
        };

        await using (var db = await dbFactory.CreateDbContextAsync())
        {
            db.AddRange(applicant, authorizedGroup, foreignGroup, membership);
            await db.SaveChangesAsync();
        }

        var service = new MembershipService(dbFactory);
        var result = approve
            ? await service.ApproveAsync(authorizedGroup.Id, membership.Id, "review")
            : await service.RejectAsync(authorizedGroup.Id, membership.Id, "review");

        await using var verificationDb = await dbFactory.CreateDbContextAsync();
        var persisted = await verificationDb.Memberships.AsNoTracking()
            .SingleAsync(m => m.Id == membership.Id);

        Assert.Null(result);
        Assert.Equal("pending", persisted.Status);
        Assert.Null(persisted.RespondedAt);
        Assert.Null(persisted.Notes);
    }
}
