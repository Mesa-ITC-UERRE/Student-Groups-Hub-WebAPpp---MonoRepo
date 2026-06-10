namespace StudentGroupsHub.DTOs.Responses;

public record GroupResponse(
    Guid Id,
    string Slug,
    string Name,
    string? Description,
    string? Category,
    string? LogoUrl,
    string? BannerUrl,
    string? ContactEmail,
    string? ContactInfo,
    string Status,
    int MemberCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record GroupRegistrationRequestResponse(
    Guid Id,
    Guid RequestedByUserId,
    string? RequestedByDisplayName,
    string ProposedGroupName,
    string? ProposedDescription,
    string ContactEmail,
    string Status,
    string? DecisionNotes,
    DateTime CreatedAt,
    DateTime? ReviewedAt
);
