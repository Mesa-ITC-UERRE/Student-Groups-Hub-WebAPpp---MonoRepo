namespace StudentGroupsHub.DTOs.Responses;

public record MembershipResponse(
    Guid MembershipId,
    Guid UserId,
    string Email,
    string? DisplayName,
    string? AvatarUrl,
    string Status,
    DateTime RequestedAt,
    DateTime? RespondedAt
);

public record EventResponse(
    Guid Id,
    Guid GroupId,
    string GroupName,
    string Title,
    string? Description,
    string? Location,
    string? BannerUrl,
    DateTime StartAt,
    DateTime EndAt,
    string Timezone,
    int? Capacity,
    int RsvpCount,
    string Status,
    string Visibility,
    DateTime CreatedAt
);

public record EventRsvpResponse(
    Guid Id,
    Guid EventId,
    Guid UserId,
    string? UserDisplayName,
    string Status,
    DateTime CreatedAt
);

public record DashboardStudentResponse(
    List<GroupResponse> JoinedGroups,
    List<MembershipResponse> PendingRequests,
    List<EventResponse> UpcomingEvents
);

public record DashboardLeaderResponse(
    List<GroupResponse> ManagedGroups,
    List<MembershipResponse> PendingMembershipRequests,
    List<EventResponse> UpcomingEvents
);

public record DashboardAdminResponse(
    int TotalUsers,
    int TotalGroups,
    int ActiveGroups,
    int PendingGroupRequests,
    int TotalEvents,
    int TotalMemberships,
    int TotalParticipations
);

public record NotificationResponse(
    Guid Id,
    string Kind,
    string Title,
    string? Body,
    string? Href,
    bool Read,
    DateTime CreatedAt
);
