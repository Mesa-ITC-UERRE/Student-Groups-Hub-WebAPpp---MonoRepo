using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupRegistrationRequest> GroupRegistrationRequests => Set<GroupRegistrationRequest>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<RoleAssignment> RoleAssignments => Set<RoleAssignment>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventParticipation> EventParticipations => Set<EventParticipation>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Report> Reports => Set<Report>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Users ──────────────────────────────────────────────────────────
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.EntraOid).IsUnique();
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Role).HasDefaultValue("student");
            e.Property(u => u.Status).HasDefaultValue("active");
        });

        // ── Groups ─────────────────────────────────────────────────────────
        modelBuilder.Entity<Group>(e =>
        {
            e.ToTable("groups");
            e.HasKey(g => g.Id);
            e.HasIndex(g => g.Slug).IsUnique();
            e.Property(g => g.Status).HasDefaultValue("pending");
        });

        // ── GroupRegistrationRequests ──────────────────────────────────────
        modelBuilder.Entity<GroupRegistrationRequest>(e =>
        {
            e.ToTable("group_registration_requests");
            e.HasKey(r => r.Id);
            e.Property(r => r.Status).HasDefaultValue("pending");
        });

        // ── Memberships ────────────────────────────────────────────────────
        modelBuilder.Entity<Membership>(e =>
        {
            e.ToTable("memberships");
            e.HasKey(m => m.Id);
            e.HasIndex(m => new { m.UserId, m.GroupId }).IsUnique();
            e.Property(m => m.Status).HasDefaultValue("pending");
        });

        // ── RoleAssignments ────────────────────────────────────────────────
        modelBuilder.Entity<RoleAssignment>(e =>
        {
            e.ToTable("role_assignments");
            e.HasKey(r => r.Id);
        });

        // ── Events ─────────────────────────────────────────────────────────
        modelBuilder.Entity<Event>(e =>
        {
            e.ToTable("events");
            e.HasKey(ev => ev.Id);
            e.Property(ev => ev.Status).HasDefaultValue("published");
            e.Property(ev => ev.Visibility).HasDefaultValue("public");
            e.Property(ev => ev.Timezone).HasDefaultValue("America/Monterrey");
        });

        // ── EventParticipations ────────────────────────────────────────────
        modelBuilder.Entity<EventParticipation>(e =>
        {
            e.ToTable("event_participations");
            e.HasKey(ep => ep.Id);
            e.HasIndex(ep => new { ep.EventId, ep.UserId }).IsUnique();
            e.Property(ep => ep.Status).HasDefaultValue("going");
        });

        // ── Notifications ──────────────────────────────────────────────────
        modelBuilder.Entity<Notification>(e =>
        {
            e.ToTable("notifications");
            e.HasKey(n => n.Id);
            e.Property(n => n.Read).HasDefaultValue(false);
        });

        // ── Reports ────────────────────────────────────────────────────────
        modelBuilder.Entity<Report>(e =>
        {
            e.ToTable("reports");
            e.HasKey(r => r.Id);
            e.Property(r => r.Status).HasDefaultValue("open");
        });
    }
}
