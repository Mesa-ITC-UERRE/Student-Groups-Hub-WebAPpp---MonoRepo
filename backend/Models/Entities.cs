using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentGroupsHub.Models;

public class User
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    [Required, MaxLength(128)] public string EntraOid { get; set; } = string.Empty;
    [Required, MaxLength(255)] public string Email { get; set; } = string.Empty;
    [MaxLength(255)] public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    [Required, MaxLength(32)] public string Role { get; set; } = "student";
    [Required, MaxLength(32)] public string Status { get; set; } = "active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class Group
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    [Required, MaxLength(128)] public string Slug { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [MaxLength(100)] public string? Category { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    [MaxLength(255)] public string? ContactEmail { get; set; }
    public string? ContactInfo { get; set; }
    [Required, MaxLength(32)] public string Status { get; set; } = "pending";
    public Guid? CreatedByUserId { get; set; }
    [ForeignKey(nameof(CreatedByUserId))] public User? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class GroupRegistrationRequest
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RequestedByUserId { get; set; }
    [ForeignKey(nameof(RequestedByUserId))] public User? RequestedBy { get; set; }
    [Required, MaxLength(200)] public string ProposedGroupName { get; set; } = string.Empty;
    public string? ProposedDescription { get; set; }
    [Required, MaxLength(255)] public string ContactEmail { get; set; } = string.Empty;
    [Required, MaxLength(32)] public string Status { get; set; } = "pending";
    public string? DecisionNotes { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    [ForeignKey(nameof(ReviewedByUserId))] public User? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? CreatedGroupId { get; set; }
    [ForeignKey(nameof(CreatedGroupId))] public Group? CreatedGroup { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Membership
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))] public User? User { get; set; }
    public Guid GroupId { get; set; }
    [ForeignKey(nameof(GroupId))] public Group? Group { get; set; }
    [Required, MaxLength(32)] public string Status { get; set; } = "pending";
    public string? Notes { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
}

public class RoleAssignment
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid GroupId { get; set; }
    [ForeignKey(nameof(GroupId))] public Group? Group { get; set; }
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))] public User? User { get; set; }
    [Required, MaxLength(32)] public string PermissionRole { get; set; } = "leader";
    [MaxLength(100)] public string? DisplayRole { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Event
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid GroupId { get; set; }
    [ForeignKey(nameof(GroupId))] public Group? Group { get; set; }
    public Guid CreatedByUserId { get; set; }
    [ForeignKey(nameof(CreatedByUserId))] public User? CreatedBy { get; set; }
    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    [MaxLength(300)] public string? Location { get; set; }
    public string? BannerUrl { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    [MaxLength(64)] public string Timezone { get; set; } = "America/Monterrey";
    public int? Capacity { get; set; }
    [Required, MaxLength(32)] public string Status { get; set; } = "published";
    [Required, MaxLength(32)] public string Visibility { get; set; } = "public";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class EventParticipation
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EventId { get; set; }
    [ForeignKey(nameof(EventId))] public Event? Event { get; set; }
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))] public User? User { get; set; }
    [Required, MaxLength(32)] public string Status { get; set; } = "going";
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public bool? Attended { get; set; }
}

public class Notification
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))] public User? User { get; set; }
    [Required, MaxLength(64)] public string Kind { get; set; } = string.Empty;
    [Required, MaxLength(255)] public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string? Href { get; set; }
    public bool Read { get; set; } = false;
    public Guid? ReferenceId { get; set; }
    [MaxLength(64)] public string? ReferenceType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Report
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ReportedByUserId { get; set; }
    [ForeignKey(nameof(ReportedByUserId))] public User? ReportedBy { get; set; }
    [Required, MaxLength(64)] public string TargetType { get; set; } = string.Empty;
    public Guid TargetId { get; set; }
    [Required, MaxLength(255)] public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    [Required, MaxLength(32)] public string Status { get; set; } = "open";
    public Guid? ReviewedByUserId { get; set; }
    [ForeignKey(nameof(ReviewedByUserId))] public User? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
