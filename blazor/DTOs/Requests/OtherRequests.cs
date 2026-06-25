using System.ComponentModel.DataAnnotations;

namespace StudentGroupsHub.DTOs.Requests;

public record ReviewMembershipRequest(string? Notes);

public record CreateEventRequest(
    [Required, MaxLength(200)] string Title,
    string? Description,
    [MaxLength(300)] string? Location,
    [Required] DateTime StartAt,
    [Required] DateTime EndAt,
    int? Capacity,
    string Status = "published",
    string Visibility = "public"
);

public record UpdateEventRequest(
    [MaxLength(200)] string? Title,
    string? Description,
    [MaxLength(300)] string? Location,
    string? BannerUrl,
    DateTime? StartAt,
    DateTime? EndAt,
    int? Capacity,
    string? Status,
    string? Visibility
);

public record UpsertRsvpRequest([Required] string Status);
