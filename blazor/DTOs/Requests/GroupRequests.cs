using System.ComponentModel.DataAnnotations;

namespace StudentGroupsHub.DTOs.Requests;

public record CreateGroupRegistrationRequest(
    [Required, MaxLength(200)] string ProposedGroupName,
    string? ProposedDescription,
    [Required, EmailAddress, MaxLength(255)] string ContactEmail,
    [MaxLength(100)] string? ProposedCategory = null
);

public record ReviewDecisionRequest(
    string? DecisionNotes,
    string? FinalCategory = null
);

public record CreateLeadershipRequest(
    [Required] Guid GroupId,
    string? Reason,
    [Required, EmailAddress, MaxLength(255)] string ContactEmail
);

public record ReviewLeadershipRequest(
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
