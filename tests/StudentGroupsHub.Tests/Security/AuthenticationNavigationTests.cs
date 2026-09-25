using StudentGroupsHub.Services;

namespace StudentGroupsHub.Tests.Security;

public sealed class AuthenticationNavigationTests
{
    [Fact]
    public void SignInUrl_UsesLocalRedirectUriAndPreservesQuery()
    {
        var result = AuthenticationNavigation.BuildSignInUrl(
            "https://groups.example.test/",
            "https://groups.example.test/groups?category=science");

        Assert.Equal(
            "/MicrosoftIdentity/Account/SignIn?redirectUri=%2Fgroups%3Fcategory%3Dscience",
            result);
        Assert.DoesNotContain("returnUrl", result, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("https%3A", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SignInUrl_RejectsAUriOutsideTheApplicationOrigin()
    {
        var result = AuthenticationNavigation.BuildSignInUrl(
            "https://groups.example.test/app/",
            "https://attacker.example.test/collect");

        Assert.Equal(
            "/app/MicrosoftIdentity/Account/SignIn?redirectUri=%2Fapp%2F",
            result);
    }
}
