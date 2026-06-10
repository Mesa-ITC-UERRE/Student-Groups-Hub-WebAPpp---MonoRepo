using System.ComponentModel.DataAnnotations;

namespace StudentGroupsHub.DTOs.Requests;

public record CreateGroupRegistrationRequest(
    [Required, MaxLength(200)] string ProposedGroupName,
    string? ProposedDescription,
    [Required, EmailAddress, MaxLength(255)] string ContactEmail
);

public record ReviewDecisionRequest(
    string? DecisionNotes
);

public record UpdateGroupRequest(
    [MaxLength(200)] string? Name,
    string? Description,
    [MaxLength(100)] string? Category,
    string? LogoUrl,
    string? BannerUrl,
    [MaxLength(255)] string? ContactEmail,
    string? ContactInfo
);
