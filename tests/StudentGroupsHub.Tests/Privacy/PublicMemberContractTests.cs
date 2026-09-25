using System.Text.Json;
using StudentGroupsHub.Models;
using StudentGroupsHub.Services;

namespace StudentGroupsHub.Tests.Privacy;

public sealed class PublicMemberContractTests
{
    [Fact]
    public void PublicResponse_DoesNotExposeIdentifiersOrEmail()
    {
        var email = "private-address@example.test";
        var membership = new Membership
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            GroupId = Guid.NewGuid(),
            Status = "accepted",
            RequestedAt = DateTime.UtcNow.AddDays(-1),
            RespondedAt = DateTime.UtcNow,
            User = new User
            {
                Id = Guid.NewGuid(),
                EntraOid = Guid.NewGuid().ToString("N"),
                Email = email,
                DisplayName = "Visible Name",
                Role = "student",
                Status = "active",
            },
        };

        var json = JsonSerializer.Serialize(MembershipService.ToPublicResponse(membership));

        Assert.DoesNotContain(email, json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("UserId", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("MembershipId", json, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Visible Name", json, StringComparison.Ordinal);
    }
}
