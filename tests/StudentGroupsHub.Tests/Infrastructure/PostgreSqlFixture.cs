using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using Testcontainers.PostgreSql;

namespace StudentGroupsHub.Tests.Infrastructure;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private const string Image =
        "postgres:16-alpine@sha256:721873c34ceb9f8d8fc265984940dc982404c105f19ad51be9fdc5970a6080ea";

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder(Image)
        .WithDatabase("studentgroups_tests")
        .WithUsername("postgres")
        .WithPassword("studentgroups_tests")
        // The fixture disposes the container explicitly; avoid pulling Ryuk so the
        // suite can run from the pinned PostgreSQL image without registry access.
        .WithCleanUp(false)
        .Build();

    public IDbContextFactory<AppDbContext> CreateDbFactory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;
        return new TestDbContextFactory(options);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await ResetDatabaseAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await using var db = await CreateDbFactory().CreateDbContextAsync();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync()
        => _container.DisposeAsync().AsTask();

    private sealed class TestDbContextFactory(DbContextOptions<AppDbContext> options)
        : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext() => new(options);
    }
}
