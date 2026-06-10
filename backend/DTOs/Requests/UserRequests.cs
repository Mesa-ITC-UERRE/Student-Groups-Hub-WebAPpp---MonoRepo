using System.ComponentModel.DataAnnotations;

namespace StudentGroupsHub.DTOs.Requests;

public record UpdateUserRequest(
    [MaxLength(255)] string? DisplayName,
    string? AvatarUrl
);
