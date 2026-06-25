namespace StudentGroupsHub.Models;

// ─── Pagination ───────────────────────────────────────────────────────────────

public record PaginatedResponse<T>(
    List<T> Data,
    int Page,
    int PageSize,
    int Total,
    int TotalPages
);

// ─── Users ────────────────────────────────────────────────────────────────────

public record UserModel(
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
)
{
    public bool IsAdmin       => Role == "admin";
    public bool IsGroupLeader => Role is "group_leader" or "admin";
    public string DisplayLabel => DisplayName ?? Email.Split('@')[0];
    public string Initial => (DisplayName ?? Email).Substring(0, 1).ToUpper();
}

// ─── Groups ───────────────────────────────────────────────────────────────────

public record GroupModel(
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

public record GroupMemberModel(
    Guid MembershipId,
    Guid UserId,
    string Email,
    string? DisplayName,
    string? AvatarUrl,
    string Status,
    DateTime? JoinedAt
)
{
    public string DisplayLabel => DisplayName ?? Email.Split('@')[0];
    public string Initial => (DisplayName ?? Email).Substring(0, 1).ToUpper();
}

public record JoinGroupResponse(Guid MembershipId, Guid GroupId, Guid UserId, string Status, DateTime RequestedAt);

// ─── Membership ───────────────────────────────────────────────────────────────

public record MembershipModel(
    Guid MembershipId,
    Guid UserId,
    Guid GroupId,
    string Email,
    string? DisplayName,
    string? AvatarUrl,
    string Status,
    DateTime RequestedAt,
    DateTime? RespondedAt
)
{
    public string DisplayLabel => DisplayName ?? Email.Split('@')[0];
    public string Initial => (DisplayName ?? Email).Substring(0, 1).ToUpper();
    public string StatusLabel => Status switch
    {
        "accepted" => "Miembro",
        "pending"  => "Pendiente",
        "rejected" => "Rechazada",
        "removed"  => "Eliminado",
        _          => Status
    };
    public string StatusCss => Status switch
    {
        "accepted" => "bg-success-subtle text-success",
        "pending"  => "bg-warning-subtle text-warning",
        "rejected" => "bg-danger-subtle text-danger",
        _          => "bg-secondary-subtle text-secondary"
    };
}

// ─── Group Registration Requests ──────────────────────────────────────────────

public record GroupRegistrationRequestModel(
    Guid Id,
    Guid RequestedByUserId,
    string? RequestedByDisplayName,
    string ProposedGroupName,
    string? ProposedDescription,
    string ContactEmail,
    string? ProposedCategory,
    string Status,
    string? DecisionNotes,
    DateTime CreatedAt,
    DateTime? ReviewedAt
)
{
    public string StatusLabel => Status switch
    {
        "approved" => "Aprobada",
        "rejected" => "Rechazada",
        _          => "Pendiente"
    };
    public string StatusCss => Status switch
    {
        "approved" => "bg-success-subtle text-success",
        "rejected" => "bg-danger-subtle text-danger",
        _          => "bg-warning-subtle text-warning"
    };
}

public record LeadershipRequestModel(
    Guid Id,
    Guid GroupId,
    string GroupName,
    string GroupSlug,
    Guid RequestedByUserId,
    string? RequestedByDisplayName,
    string ContactEmail,
    string? Reason,
    string Status,
    string? DecisionNotes,
    DateTime CreatedAt,
    DateTime? ReviewedAt
)
{
    public string StatusLabel => Status switch
    {
        "approved" => "Aprobada",
        "rejected" => "Rechazada",
        _          => "Pendiente"
    };
    public string StatusCss => Status switch
    {
        "approved" => "bg-success-subtle text-success",
        "rejected" => "bg-danger-subtle text-danger",
        _          => "bg-warning-subtle text-warning"
    };
}

public record CreateGroupRegistrationRequest(
    string ProposedGroupName,
    string? ProposedDescription,
    string ContactEmail,
    string? ProposedCategory = null
);

// ─── Events ───────────────────────────────────────────────────────────────────

public record EventModel(
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
)
{
    public bool IsCanceled  => Status == "canceled";
    public bool IsPublished => Status == "published";
    public bool IsPast      => EndAt < DateTime.UtcNow;
    public bool HasCapacity => !Capacity.HasValue || RsvpCount < Capacity.Value;
    public int? SpotsLeft   => Capacity.HasValue ? Capacity.Value - RsvpCount : null;
    public int OccupiedSpots => RsvpCount;
    public string StartLocal => StartAt.ToLocalTime().ToString("dd MMM yyyy, HH:mm");
    public string EventDateLocal => StartAt.ToLocalTime().ToString("dd MMM yyyy");
    public string MonthLabel => StartAt.ToString("MMM").ToUpper();
    public string DayLabel   => StartAt.Day.ToString();
    public string AttendanceSummary => Capacity.HasValue
        ? $"{OccupiedSpots} ocupados · {SpotsLeft ?? 0} disponibles"
        : $"{OccupiedSpots} registrados";
}

public record BlazorCreateEventRequest(
    string Title,
    string? Description,
    string? Location,
    DateTime StartAt,
    DateTime EndAt,
    int? Capacity,
    string Status = "published",
    string Visibility = "public"
);

// ─── Dashboards ───────────────────────────────────────────────────────────────

public record DashboardStudentModel(
    List<GroupModel> JoinedGroups,
    List<MembershipModel> PendingRequests,
    List<EventModel> UpcomingEvents,
    List<LeadershipRequestModel> LeadershipRequests
);

public record DashboardLeaderModel(
    List<GroupModel> ManagedGroups,
    List<MembershipModel> PendingMembershipRequests,
    List<EventModel> UpcomingEvents
);

public record DashboardAdminModel(
    int TotalUsers,
    int TotalGroups,
    int ActiveGroups,
    int PendingGroupRequests,
    int PendingLeadershipRequests,
    int TotalEvents,
    int TotalMemberships,
    int TotalParticipations
);

// ─── Notifications ────────────────────────────────────────────────────────────

public record NotificationModel(
    Guid Id,
    string Kind,
    string Title,
    string? Body,
    string? Href,
    bool Read,
    DateTime CreatedAt
);

// ─── Admin ────────────────────────────────────────────────────────────────────

public record AdminUsersResponse(List<UserModel> Data, int Page, int PageSize, int Total);

// ─── Group Terms (Administrations) ───────────────────────────────────────────

public record TermMemberModel(
    Guid Id,
    Guid TermId,
    Guid? UserId,
    string DisplayName,
    string RoleLabel,
    int SortOrder,
    string? AvatarUrl
)
{
    public string Initial => DisplayName.Substring(0, 1).ToUpper();
}

public record GroupTermModel(
    Guid Id,
    Guid GroupId,
    string Label,
    DateOnly StartDate,
    DateOnly? EndDate,
    string Status,
    string? Notes,
    DateTime CreatedAt,
    List<TermMemberModel> Members,
    string? GroupName = null,
    string? GroupSlug = null,
    string? GroupCategory = null
)
{
    public bool IsCurrent => Status == "active" && EndDate is null;
    public string DateRange => EndDate.HasValue
        ? $"{StartDate:MMM yyyy} — {EndDate:MMM yyyy}"
        : $"{StartDate:MMM yyyy} — Presente";
    public string StatusLabel => IsCurrent ? "Administración Actual" : "Administración Pasada";
    public string StatusCss   => IsCurrent
        ? "background:#d1fae5;color:#065f46;"
        : "background:#f3f4f6;color:#6b7280;";
}

// ─── API Errors ───────────────────────────────────────────────────────────────

public record ApiError(int Status, string Message, DateTime Timestamp);

// ─── Group Posts ──────────────────────────────────────────────────────────────

public record GroupPostModel(
    Guid Id,
    Guid GroupId,
    Guid AuthorUserId,
    string AuthorDisplayName,
    string? AuthorAvatarUrl,
    string Body,
    string? ImageUrl,
    DateTime CreatedAt
)
{
    public string AuthorInitial => AuthorDisplayName.Substring(0, 1).ToUpper();
    public string TimeAgo
    {
        get
        {
            var diff = DateTime.UtcNow - CreatedAt;
            if (diff.TotalMinutes < 1)  return "ahora";
            if (diff.TotalHours   < 1)  return $"hace {(int)diff.TotalMinutes} min";
            if (diff.TotalDays    < 1)  return $"hace {(int)diff.TotalHours} h";
            if (diff.TotalDays    < 7)  return $"hace {(int)diff.TotalDays} d";
            return CreatedAt.ToLocalTime().ToString("dd MMM yyyy");
        }
    }
}

public record EventPostModel(
    Guid Id,
    Guid EventId,
    Guid AuthorUserId,
    string AuthorDisplayName,
    string? AuthorAvatarUrl,
    string Body,
    string? ImageUrl,
    DateTime CreatedAt
)
{
    public string AuthorInitial => AuthorDisplayName.Substring(0, 1).ToUpper();
    public string TimeAgo
    {
        get
        {
            var diff = DateTime.UtcNow - CreatedAt;
            if (diff.TotalMinutes < 1) return "ahora";
            if (diff.TotalHours < 1) return $"hace {(int)diff.TotalMinutes} min";
            if (diff.TotalDays < 1) return $"hace {(int)diff.TotalHours} h";
            if (diff.TotalDays < 7) return $"hace {(int)diff.TotalDays} d";
            return CreatedAt.ToLocalTime().ToString("dd MMM yyyy");
        }
    }
}

public record GroupPostAuthorModel(Guid UserId, string DisplayName, string? AvatarUrl)
{
    public string Initial => DisplayName.Substring(0, 1).ToUpper();
}

// ─── Mis Grupos ───────────────────────────────────────────────────────────────

public record MyGroupsModel(
    List<GroupModel> MemberGroups,
    List<GroupModel> LeaderGroups
);

// ─── Calendar ─────────────────────────────────────────────────────────────────

public record CalendarDayModel(
    DateOnly Date,
    List<EventModel> MyGroupEvents,    // events from groups I belong to
    List<EventModel> AllEvents         // all platform events that are NOT in MyGroupEvents
);

public record CalendarMonthModel(
    int Year,
    int Month,
    List<CalendarDayModel> Days        // only days that have at least one event
)
{
    public string MonthLabel => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
}
