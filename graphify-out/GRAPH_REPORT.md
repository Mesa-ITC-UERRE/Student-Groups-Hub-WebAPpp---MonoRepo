# Graph Report - Student-Groups-Hub-WebAPpp---MonoRepo  (2026-09-25)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 1384 nodes · 2886 edges · 89 communities (74 shown, 15 thin omitted)
- Extraction: 90% EXTRACTED · 10% INFERRED · 0% AMBIGUOUS · INFERRED: 284 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `81393ce3`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Community 0
- Community 1
- Community 2
- Community 3
- Community 4
- Community 5
- Community 6
- Community 7
- Community 8
- Community 9
- Community 10
- Community 11
- Community 12
- Community 13
- Community 14
- Community 15
- Community 16
- Community 17
- Community 18
- Community 19
- Community 20
- Community 21
- Community 22
- Community 23
- Community 24
- Community 25
- Community 26
- Community 27
- Community 28
- Community 29
- Community 30
- Community 31
- Community 32
- Community 33
- Community 34
- Community 35
- Community 36
- Community 37
- Community 38
- Community 39
- Community 40
- Community 41
- Community 43
- Community 44
- Community 45
- Community 46
- Community 47
- Community 48
- Community 49
- Community 50
- Community 51
- Community 52
- Community 53
- Community 54
- Community 55
- Community 56
- Community 57
- Community 58
- Community 59
- Community 60
- Community 61
- Community 62
- Community 63
- Community 64
- Community 65
- Community 66
- Community 67
- Community 68
- Community 69
- Community 70
- Community 71
- Community 72
- Community 73
- Community 74
- Community 75
- Community 76
- Community 77
- Community 78
- Community 79
- Community 80
- Community 81
- Community 82
- Community 83
- Community 84
- Community 85
- Community 88

## God Nodes (most connected - your core abstractions)
1. `AppDbContext` - 49 edges
2. `User` - 43 edges
3. `Group` - 39 edges
4. `Event` - 37 edges
5. `GroupService` - 31 edges
6. `GroupRegistrationRequest` - 30 edges
7. `LeadershipRequest` - 29 edges
8. `Membership` - 28 edges
9. `StudentGroupsHub.Services` - 28 edges
10. `EventModel` - 26 edges

## Surprising Connections (you probably didn't know these)
- `Commitizen and Conventional Versioning` --conceptually_related_to--> `issue-to-change`  [INFERRED]
  docs/12-versioning.md → README.md
- `Student Groups Hub` --references--> `Universidad Regiomontana (U-ERRE)`  [EXTRACTED]
  README.md → docs/01-project-overview.md
- `ARIA Collaboration Skill System` --references--> `issue-to-change`  [EXTRACTED]
  docs/13-collaboration-skills.md → README.md
- `U-ERRE Design System` --rationale_for--> `Student Groups Hub`  [EXTRACTED]
  DESIGN.md → README.md
- `UI/UX Design System` --rationale_for--> `Student Groups Hub`  [EXTRACTED]
  docs/07-ui-design.md → README.md

## Import Cycles
- None detected.

## Communities (89 total, 15 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.08
Nodes (35): AllowAnonymous, Authorize, Guid, HttpGet, HttpPatch, IActionResult, Task, GroupsController (+27 more)

### Community 1 - "Community 1"
Cohesion: 0.04
Nodes (44): CancelBannerPreview, CancelLogoCrop, CloseCreateEvent, DeletePost, LoadUserPermissionsAsync, OnAfterRenderAsync, OnBannerSelected, OnJoined (+36 more)

### Community 2 - "Community 2"
Cohesion: 0.12
Nodes (25): AllowAnonymous, Guid, HttpDelete, HttpGet, HttpPatch, HttpPost, IActionResult, Task (+17 more)

### Community 3 - "Community 3"
Cohesion: 0.08
Nodes (40): AllowAnonymous, Authorize, Guid, HttpDelete, HttpGet, HttpPost, IActionResult, Task (+32 more)

### Community 4 - "Community 4"
Cohesion: 0.13
Nodes (23): HttpGet, IActionResult, Task, DashboardController, DateTime, Guid, GroupRegistrationRequestResponse, GroupResponse (+15 more)

### Community 5 - "Community 5"
Cohesion: 0.11
Nodes (25): DateTime, Guid, List, AdminUsersResponse, ApiError, BlazorCreateEventRequest, BlazorUpdateEventRequest, CalendarDayModel (+17 more)

### Community 6 - "Community 6"
Cohesion: 0.08
Nodes (32): Guid, HttpGet, HttpPatch, HttpPost, IActionResult, IDbContextFactory, List, Task (+24 more)

### Community 7 - "Community 7"
Cohesion: 0.12
Nodes (12): CreateGroupRegistrationRequest, GroupRegistrationRequestModel, StatusCss, StatusLabel, LeadershipRequestModel, StatusCss, StatusLabel, CalendarApiService (+4 more)

### Community 8 - "Community 8"
Cohesion: 0.18
Nodes (7): DashboardAdminModel, EventsMonthDelta, PendingRequests, DashboardApiService, DbSafe, Exception, Func

### Community 9 - "Community 9"
Cohesion: 0.07
Nodes (28): Activate, ApproveMember, ConfirmSeasonReset, Deactivate, GoToPage, LoadAsync, LoadMembersAsync, OnInitializedAsync (+20 more)

### Community 10 - "Community 10"
Cohesion: 0.13
Nodes (13): AppDbContext&gt;, GroupPostAuthorModel, Initial, GroupPostModel, AuthorInitial, TimeAgo, Dictionary, Guid (+5 more)

### Community 11 - "Community 11"
Cohesion: 0.11
Nodes (14): EventPostModel, AuthorInitial, TimeAgo, EventPostApiService, Task, StorageService, AuthToken, Bucket (+6 more)

### Community 12 - "Community 12"
Cohesion: 0.10
Nodes (29): Guid, HttpGet, HttpPatch, HttpPost, IActionResult, Task, GroupRegistrationRequestsController, ReviewDecisionRequest (+21 more)

### Community 13 - "Community 13"
Cohesion: 0.17
Nodes (10): UserModel, DisplayLabel, Initial, IsAdmin, IsGroupLeader, UserApiService, AuthenticationStateProvider, ClaimsPrincipal (+2 more)

### Community 14 - "Community 14"
Cohesion: 0.09
Nodes (31): Guid, HttpGet, HttpPatch, HttpPost, IActionResult, Task, LeadershipRequestsController, Guid (+23 more)

### Community 15 - "Community 15"
Cohesion: 0.16
Nodes (14): User, AvatarUrl, CreatedAt, DisplayName, Email, EntraOid, Id, Role (+6 more)

### Community 16 - "Community 16"
Cohesion: 0.08
Nodes (23): AdminDashboardView, CancelCrop, MarkAllRead, MarkRead, OnAfterRenderAsync, OnAvatarSelected, OnInitializedAsync, OnZoomChanged (+15 more)

### Community 17 - "Community 17"
Cohesion: 0.09
Nodes (22): Activate, CancelPromoteToLeader, ConfirmPromoteToLeader, Deactivate, DemoteToStudent, GoToPage, LoadAsync, LoadLeaderGroupsAsync (+14 more)

### Community 18 - "Community 18"
Cohesion: 0.10
Nodes (20): CloseEditEvent, CloseGalleryImage, DeletePost, OnParametersSetAsync, OnPostImageSelected, OpenEditEvent, OpenGalleryImage, CurrentUserService (+12 more)

### Community 19 - "Community 19"
Cohesion: 0.10
Nodes (19): CancelExpand, CancelLeadershipExpand, ConfirmApprove, ConfirmLeadershipApprove, ConfirmLeadershipReject, ConfirmReject, ExpandApprove, ExpandLeadershipApprove (+11 more)

### Community 20 - "Community 20"
Cohesion: 0.07
Nodes (58): U-ERRE Design System, Universidad Regiomontana (U-ERRE), ASP.NET Core 8 Web API (historic architecture), Azure Static Web Apps (historic frontend hosting), React SPA (historic architecture), Event, EventParticipation, Group (+50 more)

### Community 21 - "Community 21"
Cohesion: 0.16
Nodes (10): DateOnly, GroupTermModel, DateRange, IsCurrent, StatusCss, StatusLabel, TermMemberModel, Initial (+2 more)

### Community 22 - "Community 22"
Cohesion: 0.11
Nodes (19): AppDbContext, EventParticipations, EventPosts, Events, GroupPostAuthors, GroupPosts, GroupRegistrationRequests, Groups (+11 more)

### Community 23 - "Community 23"
Cohesion: 0.33
Nodes (6): StudentGroupsHub.Data.Migrations, microsoft_entityframeworkcore_infrastructure, microsoft_entityframeworkcore_migrations, microsoft_entityframeworkcore_storage_valueconversion, npgsql_entityframeworkcore_postgresql_metadata, system

### Community 24 - "Community 24"
Cohesion: 0.12
Nodes (13): MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder, RenameEntraOidToSupabaseId, MigrationBuilder, DateOnly (+5 more)

### Community 25 - "Community 25"
Cohesion: 0.15
Nodes (15): EventModel, AttendanceSummary, DayLabel, EventDateLocal, HasCapacity, IsCanceled, IsPast, IsPublished (+7 more)

### Community 26 - "Community 26"
Cohesion: 0.36
Nodes (6): Guid, HttpGet, HttpPatch, IActionResult, Task, UsersController

### Community 27 - "Community 27"
Cohesion: 0.37
Nodes (5): StudentGroupsHub.Services, StudentGroupsHub.DTOs.Responses, StudentGroupsHub.Models, StudentGroupsHub.Data, microsoft_entityframeworkcore

### Community 28 - "Community 28"
Cohesion: 0.50
Nodes (4): DateTime, Guid, ApiError, UserResponse

### Community 29 - "Community 29"
Cohesion: 0.12
Nodes (16): HandleSubmit, OnInitializedAsync, AuthenticationStateProvider, Authorized, AuthorizeView, DataAnnotationsValidator, EditForm, GroupApiService (+8 more)

### Community 30 - "Community 30"
Cohesion: 0.39
Nodes (5): StudentGroupsHub.Extensions, StudentGroupsHub.DTOs.Requests, StudentGroupsHub.Controllers, microsoft_aspnetcore_authorization, microsoft_aspnetcore_mvc

### Community 32 - "Community 32"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 33 - "Community 33"
Cohesion: 0.13
Nodes (14): ClearFilters, GradientFor, LoadGroupsAsync, OnInitializedAsync, OnSearchChanged, AuthenticationStateProvider, Authorized, AuthorizeView (+6 more)

### Community 34 - "Community 34"
Cohesion: 0.15
Nodes (12): Microsoft.AspNetCore.Authorization, Microsoft.AspNetCore.Components.Authorization, Microsoft.AspNetCore.Components.Forms, Microsoft.AspNetCore.Components.Routing, Microsoft.AspNetCore.Components.Web, Microsoft.AspNetCore.Components.Web.RenderMode, Microsoft.AspNetCore.Components.Web.Virtualization, Microsoft.JSInterop (+4 more)

### Community 35 - "Community 35"
Cohesion: 0.15
Nodes (12): HandleSubmit, OnInitializedAsync, CurrentUserService, DataAnnotationsValidator, EditForm, GroupApiService, InputText, InputTextArea (+4 more)

### Community 36 - "Community 36"
Cohesion: 0.15
Nodes (12): FormatCompact, HomeGroupItem, OnInitializedAsync, Authorized, AuthorizeView, CalendarApiService, DashboardService, MisGruposApiService (+4 more)

### Community 37 - "Community 37"
Cohesion: 0.15
Nodes (12): CancelAddMember, CancelAddTerm, DeleteTerm, LoadAsync, OnInitializedAsync, AuthenticationStateProvider, GroupTermApiService, GroupTermModel (+4 more)

### Community 38 - "Community 38"
Cohesion: 0.15
Nodes (10): microsoft_aspnetcore_authentication_jwtbearer, microsoft_aspnetcore_authentication_openidconnect, microsoft_aspnetcore_components_authorization, microsoft_aspnetcore_httpoverrides, microsoft_aspnetcore_mvc_authorization, microsoft_identity_web, microsoft_identity_web_ui, microsoft_identitymodel_tokens (+2 more)

### Community 39 - "Community 39"
Cohesion: 0.12
Nodes (19): GroupTerm, CreatedAt, EndDate, Group, GroupId, Id, IsCurrent, Label (+11 more)

### Community 40 - "Community 40"
Cohesion: 0.15
Nodes (12): Notification, Body, CreatedAt, Href, Id, Kind, Read, ReferenceId (+4 more)

### Community 41 - "Community 41"
Cohesion: 0.15
Nodes (13): Report, CreatedAt, Details, Id, Reason, ReportedBy, ReportedByUserId, ReviewedAt (+5 more)

### Community 43 - "Community 43"
Cohesion: 0.17
Nodes (11): Dispose, OnInitializedAsync, OnUserStateChanged, Authorized, AuthorizeView, NavigationManager, NavLink, NotAuthorized (+3 more)

### Community 44 - "Community 44"
Cohesion: 0.17
Nodes (11): GroupTimelineEntry, LoadAsync, MatchesYear, OnInitializedAsync, EventApiService, EventModel, GroupTermApiService, GroupTermModel (+3 more)

### Community 45 - "Community 45"
Cohesion: 0.12
Nodes (17): ModelBuilder, Guid, EventPost, Author, AuthorUserId, Body, CreatedAt, Event (+9 more)

### Community 46 - "Community 46"
Cohesion: 0.18
Nodes (9): DateOnly, DateTime, Guid, MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder (+1 more)

### Community 47 - "Community 47"
Cohesion: 0.18
Nodes (9): DateOnly, DateTime, Guid, MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder (+1 more)

### Community 48 - "Community 48"
Cohesion: 0.17
Nodes (12): DateOnly, RoleAssignment, CreatedAt, DisplayRole, EndDate, Group, GroupId, Id (+4 more)

### Community 49 - "Community 49"
Cohesion: 0.17
Nodes (12): StudentGroupsHub, net10.0, FluentValidation.AspNetCore (11.3.1), Microsoft.AspNetCore.Authentication.JwtBearer (10.0.9), Microsoft.EntityFrameworkCore (10.0.9), Microsoft.EntityFrameworkCore.Design (10.0.9), Microsoft.EntityFrameworkCore.Tools (10.0.9), Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore (10.0.9) (+4 more)

### Community 50 - "Community 50"
Cohesion: 0.20
Nodes (8): DateTime, Guid, MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder, AddEventPosts

### Community 51 - "Community 51"
Cohesion: 0.18
Nodes (8): GlobalExceptionHandlerExtensions, StudentGroupsHub.Middleware, IApplicationBuilder, IExceptionHandlerFeature, microsoft_aspnetcore_diagnostics, system_net_http_headers, system_text, system_text_json

### Community 52 - "Community 52"
Cohesion: 0.18
Nodes (11): TermMember, AvatarUrl, CreatedAt, DisplayName, Id, RoleLabel, SortOrder, Term (+3 more)

### Community 53 - "Community 53"
Cohesion: 0.31
Nodes (7): Dictionary, Guid, IDbContextFactory, IEnumerable, List, Task, EventPostService

### Community 54 - "Community 54"
Cohesion: 0.38
Nodes (5): Guid, IDbContextFactory, List, Task, GroupPostService

### Community 55 - "Community 55"
Cohesion: 0.36
Nodes (9): attachEvents(), clampOffset(), draw(), exportCanvas(), getState(), initState(), open(), openFromInput() (+1 more)

### Community 56 - "Community 56"
Cohesion: 0.20
Nodes (9): OnInitializedAsync, AuthenticationStateProvider, GroupApiService, GroupMemberModel, GroupTermApiService, GroupTermModel, GroupTermsTab, PageTitle (+1 more)

### Community 57 - "Community 57"
Cohesion: 0.20
Nodes (9): Approve, GroupApiService, GroupMemberModel, Guid, NotificationModel, NotificationsPanel, Reject, RemoveMember (+1 more)

### Community 58 - "Community 58"
Cohesion: 0.20
Nodes (10): DateTime, EventParticipation, Attended, Event, EventId, Id, RegisteredAt, Status (+2 more)

### Community 59 - "Community 59"
Cohesion: 0.20
Nodes (10): GroupPost, Author, AuthorUserId, Body, CreatedAt, Group, GroupId, Id (+2 more)

### Community 60 - "Community 60"
Cohesion: 0.22
Nodes (8): NextMonth, OnInitializedAsync, PrevMonth, CalendarApiService, CalendarMonthModel, PageTitle, SelectDay, route:/calendario

### Community 61 - "Community 61"
Cohesion: 0.25
Nodes (6): MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder, AddProposedCategory

### Community 62 - "Community 62"
Cohesion: 0.29
Nodes (7): AuthorizationHandler, AuthorizationHandlerContext, Task, AdminRoleHandler, AdminRoleRequirement, IAuthorizationRequirement, IServiceScopeFactory

### Community 63 - "Community 63"
Cohesion: 0.32
Nodes (6): handleReconnectStateChanged(), reconnectModal, resumeButton, retry(), retryButton, retryWhenDocumentBecomesVisible()

### Community 64 - "Community 64"
Cohesion: 0.25
Nodes (7): LoadAsync, OnInitializedAsync, OnSearch, EventApiService, EventModel, PageTitle, route:/events

### Community 65 - "Community 65"
Cohesion: 0.40
Nodes (3): UpdateUserRequest, System.ComponentModel.DataAnnotations, system_componentmodel_dataannotations_schema

### Community 66 - "Community 66"
Cohesion: 0.29
Nodes (6): AuthorizeRouteView, NotAuthorized, FocusOnNavigate, Found, RedirectToLogin, Router

### Community 67 - "Community 67"
Cohesion: 0.29
Nodes (6): OnInitializedAsync, AdminApiService, AdminKpiGrid, NavigationManager, PageTitle, route:/admin

### Community 68 - "Community 68"
Cohesion: 0.29
Nodes (6): DateOnly, DateTime, Guid, ModelBuilder, AppDbContextModelSnapshot, ModelSnapshot

### Community 69 - "Community 69"
Cohesion: 0.33
Nodes (5): HeadOutlet, ImportMap, ReconnectModal, ResourcePreloader, Routes

### Community 70 - "Community 70"
Cohesion: 0.40
Nodes (4): OnInitializedAsync, MisGruposApiService, PageTitle, route:/mis-grupos

### Community 71 - "Community 71"
Cohesion: 0.40
Nodes (4): GroupRegistrationRequestModel, Guid, NotificationModel, NotificationsPanel

### Community 73 - "Community 73"
Cohesion: 0.50
Nodes (3): Authorized, AuthorizeView, NavLink

### Community 74 - "Community 74"
Cohesion: 0.50
Nodes (3): OnInitialized, PageTitle, System.Diagnostics

### Community 75 - "Community 75"
Cohesion: 0.50
Nodes (3): HandleJoin, OnParametersSet, GroupApiService

### Community 76 - "Community 76"
Cohesion: 0.50
Nodes (3): HandleCancel, HandleRsvp, EventApiService

## Knowledge Gaps
- **532 isolated node(s):** `BannerUrl`, `Category`, `ContactEmail`, `ContactInfo`, `CreatedAt` (+527 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 713 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **15 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `System.Security.Claims` connect `Community 38` to `Community 1`?**
  _High betweenness centrality (0.088) - this node is a cross-community bridge._
- **Why does `AppDbContext` connect `Community 22` to `Community 0`, `Community 2`, `Community 3`, `Community 4`, `Community 6`, `Community 12`, `Community 14`, `Community 15`, `Community 27`, `Community 39`, `Community 40`, `Community 41`, `Community 45`, `Community 48`, `Community 52`, `Community 53`, `Community 54`, `Community 58`, `Community 59`?**
  _High betweenness centrality (0.082) - this node is a cross-community bridge._
- **Why does `CurrentUserService` connect `Community 13` to `Community 0`, `Community 5`, `Community 38`, `Community 7`, `Community 8`, `Community 10`, `Community 11`, `Community 15`, `Community 21`, `Community 25`?**
  _High betweenness centrality (0.071) - this node is a cross-community bridge._
- **What connects `BannerUrl`, `Category`, `ContactEmail` to the rest of the system?**
  _532 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Community 0` be split into smaller, more focused modules?**
  _Cohesion score 0.07589285714285714 - nodes in this community are weakly interconnected._
- **Should `Community 1` be split into smaller, more focused modules?**
  _Cohesion score 0.044444444444444446 - nodes in this community are weakly interconnected._
- **Should `Community 2` be split into smaller, more focused modules?**
  _Cohesion score 0.11666666666666667 - nodes in this community are weakly interconnected._