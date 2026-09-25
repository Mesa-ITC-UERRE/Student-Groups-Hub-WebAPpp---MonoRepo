namespace StudentGroupsHub.DTOs.Responses;

public record MembershipResponse(
    Guid MembershipId,
    Guid UserId,
    Guid GroupId,
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
    List<EventResponse> UpcomingEvents,
    List<LeadershipRequestResponse>? LeadershipRequests = null
);

public record DashboardLeaderResponse(
    List<GroupResponse> ManagedGroups,
    List<MembershipResponse> PendingMembershipRequests,
    List<EventResponse> UpcomingEvents
);

public record DashboardAdminResponse(
    int TotalUsers,
    int ActiveStudents,
    int StudentsThisMonth,
    int TotalGroups,
    int ActiveGroups,
    int PendingGroupRequests,
    int PendingLeadershipRequests,
    int TotalEvents,
    int EventsThisMonth,
    int EventsPreviousMonth,
    int ProcessedMemberships,
    double MembershipApprovalRate,
    double AverageMembershipResponseHours,
    int TotalMemberships,
    int TotalParticipations,
    double AverageParticipationsPerEvent
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
