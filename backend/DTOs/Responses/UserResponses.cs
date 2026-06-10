namespace StudentGroupsHub.DTOs.Responses;

public record UserResponse(
    Guid Id,
    string EntraOid,
    string Email,
    string? DisplayName,
    string? AvatarUrl,
    string Role,
    string Status,
    bool IsPlatformAdmin,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record ApiError(int Status, string Message, DateTime Timestamp);
