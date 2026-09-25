using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentGroupsHub.Data;
using StudentGroupsHub.Models;
using StudentGroupsHub.Services;
using StudentGroupsHub.Tests.Infrastructure;

namespace StudentGroupsHub.Tests.Security;

[Collection(PostgreSqlCollection.Name)]
public sealed class ActiveUserAuthorizationTests(PostgreSqlFixture database)
{
    [Fact]
    public async Task ActiveRequirement_AllowsActiveLocalUser()
    {
        await database.ResetDatabaseAsync();
        var dbFactory = database.CreateDbFactory();
        var activeUser = TestData.User();
        await using (var db = await dbFactory.CreateDbContextAsync())
        {
            db.Users.Add(activeUser);
            await db.SaveChangesAsync();
        }

        using var services = CreateServices(dbFactory);
        var handler = new ActiveUserHandler(services.GetRequiredService<IServiceScopeFactory>());
        var requirement = new ActiveUserRequirement();
        var context = new AuthorizationHandlerContext(
            [requirement], PrincipalFor(activeUser), resource: null);

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task ActiveRequirement_DeniesInactiveLocalUser()
    {
        await database.ResetDatabaseAsync();
        var dbFactory = database.CreateDbFactory();
        var inactiveUser = TestData.User(status: "inactive");
        await using (var db = await dbFactory.CreateDbContextAsync())
        {
            db.Users.Add(inactiveUser);
            await db.SaveChangesAsync();
        }

        using var services = CreateServices(dbFactory);
        var handler = new ActiveUserHandler(services.GetRequiredService<IServiceScopeFactory>());
        var requirement = new ActiveUserRequirement();
        var context = new AuthorizationHandlerContext(
            [requirement], PrincipalFor(inactiveUser), resource: null);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task AdminRequirement_DeniesInactiveAdmin()
    {
        await database.ResetDatabaseAsync();
        var dbFactory = database.CreateDbFactory();
        var inactiveAdmin = TestData.User("admin", "inactive");
        await using (var db = await dbFactory.CreateDbContextAsync())
        {
            db.Users.Add(inactiveAdmin);
            await db.SaveChangesAsync();
        }

        using var services = CreateServices(dbFactory);
        var handler = new AdminRoleHandler(services.GetRequiredService<IServiceScopeFactory>());
        var requirement = new AdminRoleRequirement();
        var context = new AuthorizationHandlerContext(
            [requirement], PrincipalFor(inactiveAdmin), resource: null);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task CurrentUser_ObservesStatusChangesWithoutCircuitInvalidation()
    {
        await database.ResetDatabaseAsync();
        var dbFactory = database.CreateDbFactory();
        var user = TestData.User();
        await using (var db = await dbFactory.CreateDbContextAsync())
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        var userService = new UserService(dbFactory);
        var currentUser = new CurrentUserService(
            new TestAuthenticationStateProvider(PrincipalFor(user)), userService);

        Assert.True(UserService.IsActive(await currentUser.GetUserAsync()));

        await using (var db = await dbFactory.CreateDbContextAsync())
        {
            var persisted = await db.Users.SingleAsync(u => u.Id == user.Id);
            persisted.Status = "inactive";
            await db.SaveChangesAsync();
        }

        Assert.False(UserService.IsActive(await currentUser.GetUserAsync()));
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => currentUser.RequireActiveUserAsync());
    }

    private static ServiceProvider CreateServices(IDbContextFactory<AppDbContext> dbFactory)
        => new ServiceCollection()
            .AddSingleton(dbFactory)
            .AddScoped<UserService>()
            .BuildServiceProvider();

    private static ClaimsPrincipal PrincipalFor(User user)
        => new(new ClaimsIdentity(
        [
            new Claim("oid", user.EntraOid),
            new Claim("preferred_username", user.Email),
            new Claim("name", user.DisplayName ?? "Test User"),
        ], authenticationType: "test"));

    private sealed class TestAuthenticationStateProvider(ClaimsPrincipal principal)
        : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
            => Task.FromResult(new AuthenticationState(principal));
    }
}
