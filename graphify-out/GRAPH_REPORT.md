# Graph Report - Student-Groups-Hub-WebAPpp---MonoRepo  (2026-09-25)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 1374 nodes · 2859 edges · 86 communities (71 shown, 15 thin omitted)
- Extraction: 90% EXTRACTED · 10% INFERRED · 0% AMBIGUOUS · INFERRED: 280 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `8bcc42d7`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- StudentGroupsHub.Services
- Group
- .GetByEntraOidAsync
- UserService
- AppModels.cs
- LeadershipRequest
- GroupDetail.razor
- GroupRegistrationRequest
- User
- DashboardService
- Guid
- Task
- AdminGroups.razor
- GroupTerm
- EventService
- Perfil.razor
- AdminUsers.razor
- EventDetail.razor
- .GetActiveUserIdAsync
- AdminRequests.razor
- CurrentUserService
- AppDbContext
- StudentGroupsHub.Data.Migrations
- StorageService
- Event
- GroupApiService
- RegisterGroup.razor
- User
- http
- Groups.razor
- ApplyForLeadership.razor
- Home.razor
- GroupTermsTab.razor
- Report
- NavMenu.razor
- Timeline.razor
- Membership
- InitialSchema
- AddLeadershipRequestsAndSeasonReset
- GroupPost
- RoleAssignment
- Notification
- StudentGroupsHub
- AddEventPosts
- TermMember
- EventPostService
- avatar-crop.js
- Feur.razor
- LeaderDashboardView.razor
- RenameEntraOidToSupabaseId
- EventParticipation
- EventPost
- GroupPostService
- Calendario.razor
- RenameSupabaseIdToEntraOid
- AddProposedCategory
- GroupPostApiService
- ReconnectModal.razor.js
- Events.razor
- Routes.razor
- AdminIndex.razor
- .BuildModel
- ClaimsPrincipalExtensions
- Unreleased
- App.razor
- MisGrupos.razor
- StudentDashboardView.razor
- MobileTabBar.razor
- Error.razor
- JoinButton.razor
- RsvpButton.razor
- DashboardAdminModel
- MainLayout.razor
- NotificationsPanel.razor
- RedirectToLogin.razor
- NotFound.razor
- AdminDashboardView.razor
- FluentIcon.razor
- U-ERRE Design System
- Universidad Regiomontana (U-ERRE)
- UI/UX Design System
- Azure Container Apps
- Commitizen and Conventional Versioning

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
- `GroupsController` --references--> `UserService`  [EXTRACTED]
  blazor/Controllers/GroupsController.cs → blazor/Services/UserService.cs
- `AppDbContext` --references--> `Group`  [EXTRACTED]
  blazor/Data/AppDbContext.cs → blazor/Models/Entities.cs
- `MisGruposApiService` --references--> `CurrentUserService`  [EXTRACTED]
  blazor/Services/ApiServices.cs → blazor/Services/CurrentUserService.cs
- `EventsController` --references--> `GroupService`  [EXTRACTED]
  blazor/Controllers/EventsController.cs → blazor/Services/GroupService.cs
- `MembershipsController` --references--> `GroupService`  [EXTRACTED]
  blazor/Controllers/MembershipsController.cs → blazor/Services/GroupService.cs

## Import Cycles
- None detected.

## Communities (86 total, 15 thin omitted)

### Community 0 - "StudentGroupsHub.Services"
Cohesion: 0.07
Nodes (40): AuthorizationHandler, AuthorizationHandlerContext, Microsoft.AspNetCore.Authorization, Microsoft.AspNetCore.Components.Authorization, Microsoft.AspNetCore.Components.Forms, Task, AdminRoleHandler, AdminRoleRequirement (+32 more)

### Community 1 - "Group"
Cohesion: 0.07
Nodes (37): AllowAnonymous, Authorize, Guid, HttpGet, HttpPatch, IActionResult, Task, GroupsController (+29 more)

### Community 2 - ".GetByEntraOidAsync"
Cohesion: 0.09
Nodes (33): AllowAnonymous, Authorize, Guid, HttpDelete, HttpGet, HttpPost, IActionResult, Task (+25 more)

### Community 3 - "UserService"
Cohesion: 0.07
Nodes (36): Guid, HttpGet, HttpPatch, HttpPost, IActionResult, IDbContextFactory, List, Task (+28 more)

### Community 4 - "AppModels.cs"
Cohesion: 0.06
Nodes (53): DateOnly, DateTime, Guid, List, AdminUsersResponse, ApiError, BlazorCreateEventRequest, BlazorUpdateEventRequest (+45 more)

### Community 5 - "LeadershipRequest"
Cohesion: 0.09
Nodes (31): Guid, HttpGet, HttpPatch, HttpPost, IActionResult, Task, LeadershipRequestsController, Guid (+23 more)

### Community 6 - "GroupDetail.razor"
Cohesion: 0.04
Nodes (44): CancelBannerPreview, CancelLogoCrop, CloseCreateEvent, DeletePost, LoadUserPermissionsAsync, OnAfterRenderAsync, OnBannerSelected, OnJoined (+36 more)

### Community 7 - "GroupRegistrationRequest"
Cohesion: 0.10
Nodes (29): Guid, HttpGet, HttpPatch, HttpPost, IActionResult, Task, GroupRegistrationRequestsController, ReviewDecisionRequest (+21 more)

### Community 8 - "User"
Cohesion: 0.10
Nodes (43): ASP.NET Core 8 Web API (historic architecture), Azure Static Web Apps (historic frontend hosting), React SPA (historic architecture), Event, EventParticipation, Group, GroupRegistrationRequest, Membership (+35 more)

### Community 9 - "DashboardService"
Cohesion: 0.11
Nodes (24): HttpGet, IActionResult, Task, DashboardController, DateTime, Guid, GroupRegistrationRequestResponse, GroupResponse (+16 more)

### Community 10 - "Guid"
Cohesion: 0.11
Nodes (12): AppDbContext&gt;, UserModel, DisplayLabel, Initial, IsAdmin, IsGroupLeader, DateOnly, Guid (+4 more)

### Community 11 - "Task"
Cohesion: 0.17
Nodes (6): DateTime, Dictionary, List, Task, EventApiService, Func

### Community 12 - "AdminGroups.razor"
Cohesion: 0.07
Nodes (28): Activate, ApproveMember, ConfirmSeasonReset, Deactivate, GoToPage, LoadAsync, LoadMembersAsync, OnInitializedAsync (+20 more)

### Community 13 - "GroupTerm"
Cohesion: 0.12
Nodes (19): GroupTerm, CreatedAt, EndDate, Group, GroupId, Id, IsCurrent, Label (+11 more)

### Community 14 - "EventService"
Cohesion: 0.19
Nodes (11): DateTime, CreateEventRequest, UpdateEventRequest, DateTime, Guid, HashSet, IDbContextFactory, IEnumerable (+3 more)

### Community 15 - "Perfil.razor"
Cohesion: 0.08
Nodes (23): AdminDashboardView, CancelCrop, MarkAllRead, MarkRead, OnAfterRenderAsync, OnAvatarSelected, OnInitializedAsync, OnZoomChanged (+15 more)

### Community 16 - "AdminUsers.razor"
Cohesion: 0.09
Nodes (22): Activate, CancelPromoteToLeader, ConfirmPromoteToLeader, Deactivate, DemoteToStudent, GoToPage, LoadAsync, LoadLeaderGroupsAsync (+14 more)

### Community 17 - "EventDetail.razor"
Cohesion: 0.10
Nodes (20): CloseEditEvent, CloseGalleryImage, DeletePost, OnParametersSetAsync, OnPostImageSelected, OpenEditEvent, OpenGalleryImage, CurrentUserService (+12 more)

### Community 18 - ".GetActiveUserIdAsync"
Cohesion: 0.19
Nodes (8): GroupRegistrationRequestModel, StatusCss, StatusLabel, LeadershipRequestModel, StatusCss, StatusLabel, GroupRegistrationApiService, LeadershipRequestApiService

### Community 19 - "AdminRequests.razor"
Cohesion: 0.10
Nodes (19): CancelExpand, CancelLeadershipExpand, ConfirmApprove, ConfirmLeadershipApprove, ConfirmLeadershipReject, ConfirmReject, ExpandApprove, ExpandLeadershipApprove (+11 more)

### Community 20 - "CurrentUserService"
Cohesion: 0.15
Nodes (10): CalendarApiService, DbSafe, EventPostApiService, NotificationApiService, AuthenticationStateProvider, ClaimsPrincipal, Guid, Task (+2 more)

### Community 21 - "AppDbContext"
Cohesion: 0.11
Nodes (19): AppDbContext, EventParticipations, EventPosts, Events, GroupPostAuthors, GroupPosts, GroupRegistrationRequests, Groups (+11 more)

### Community 22 - "StudentGroupsHub.Data.Migrations"
Cohesion: 0.33
Nodes (6): StudentGroupsHub.Data.Migrations, microsoft_entityframeworkcore_infrastructure, microsoft_entityframeworkcore_migrations, microsoft_entityframeworkcore_storage_valueconversion, npgsql_entityframeworkcore_postgresql_metadata, system

### Community 23 - "StorageService"
Cohesion: 0.11
Nodes (16): GlobalExceptionHandlerExtensions, StorageService, AuthToken, Bucket, SupabaseUrl, UsingServiceKey, StudentGroupsHub.Middleware, IApplicationBuilder (+8 more)

### Community 24 - "Event"
Cohesion: 0.11
Nodes (18): Event, BannerUrl, Capacity, CreatedAt, CreatedBy, CreatedByUserId, Description, EndAt (+10 more)

### Community 25 - "GroupApiService"
Cohesion: 0.24
Nodes (6): GroupApiService, Guid, IDbContextFactory, List, Task, MembershipService

### Community 26 - "RegisterGroup.razor"
Cohesion: 0.12
Nodes (16): HandleSubmit, OnInitializedAsync, AuthenticationStateProvider, Authorized, AuthorizeView, DataAnnotationsValidator, EditForm, GroupApiService (+8 more)

### Community 27 - "User"
Cohesion: 0.12
Nodes (16): Guid, GroupPostAuthor, Group, GroupId, User, UserId, User, AvatarUrl (+8 more)

### Community 28 - "http"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 29 - "Groups.razor"
Cohesion: 0.13
Nodes (14): ClearFilters, GradientFor, LoadGroupsAsync, OnInitializedAsync, OnSearchChanged, AuthenticationStateProvider, Authorized, AuthorizeView (+6 more)

### Community 30 - "ApplyForLeadership.razor"
Cohesion: 0.15
Nodes (12): HandleSubmit, OnInitializedAsync, CurrentUserService, DataAnnotationsValidator, EditForm, GroupApiService, InputText, InputTextArea (+4 more)

### Community 31 - "Home.razor"
Cohesion: 0.15
Nodes (12): FormatCompact, HomeGroupItem, OnInitializedAsync, Authorized, AuthorizeView, CalendarApiService, DashboardService, MisGruposApiService (+4 more)

### Community 32 - "GroupTermsTab.razor"
Cohesion: 0.15
Nodes (12): CancelAddMember, CancelAddTerm, DeleteTerm, LoadAsync, OnInitializedAsync, AuthenticationStateProvider, GroupTermApiService, GroupTermModel (+4 more)

### Community 33 - "Report"
Cohesion: 0.15
Nodes (13): Report, CreatedAt, Details, Id, Reason, ReportedBy, ReportedByUserId, ReviewedAt (+5 more)

### Community 34 - "NavMenu.razor"
Cohesion: 0.17
Nodes (11): Dispose, OnInitializedAsync, OnUserStateChanged, Authorized, AuthorizeView, NavigationManager, NavLink, NotAuthorized (+3 more)

### Community 35 - "Timeline.razor"
Cohesion: 0.17
Nodes (11): GroupTimelineEntry, LoadAsync, MatchesYear, OnInitializedAsync, EventApiService, EventModel, GroupTermApiService, GroupTermModel (+3 more)

### Community 36 - "Membership"
Cohesion: 0.17
Nodes (11): ModelBuilder, Membership, Group, GroupId, Id, Notes, RequestedAt, RespondedAt (+3 more)

### Community 37 - "InitialSchema"
Cohesion: 0.18
Nodes (9): DateOnly, DateTime, Guid, MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder (+1 more)

### Community 38 - "AddLeadershipRequestsAndSeasonReset"
Cohesion: 0.18
Nodes (9): DateOnly, DateTime, Guid, MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder (+1 more)

### Community 39 - "GroupPost"
Cohesion: 0.17
Nodes (11): GroupPost, Author, AuthorUserId, Body, CreatedAt, Group, GroupId, Id (+3 more)

### Community 40 - "RoleAssignment"
Cohesion: 0.17
Nodes (12): DateOnly, RoleAssignment, CreatedAt, DisplayRole, EndDate, Group, GroupId, Id (+4 more)

### Community 41 - "Notification"
Cohesion: 0.17
Nodes (12): Notification, Body, CreatedAt, Href, Id, Kind, Read, ReferenceId (+4 more)

### Community 42 - "StudentGroupsHub"
Cohesion: 0.17
Nodes (12): StudentGroupsHub, net10.0, FluentValidation.AspNetCore (11.3.1), Microsoft.AspNetCore.Authentication.JwtBearer (10.0.9), Microsoft.EntityFrameworkCore (10.0.9), Microsoft.EntityFrameworkCore.Design (10.0.9), Microsoft.EntityFrameworkCore.Tools (10.0.9), Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore (10.0.9) (+4 more)

### Community 43 - "AddEventPosts"
Cohesion: 0.20
Nodes (8): DateTime, Guid, MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder, AddEventPosts

### Community 44 - "TermMember"
Cohesion: 0.18
Nodes (11): TermMember, AvatarUrl, CreatedAt, DisplayName, Id, RoleLabel, SortOrder, Term (+3 more)

### Community 45 - "EventPostService"
Cohesion: 0.31
Nodes (7): Dictionary, Guid, IDbContextFactory, IEnumerable, List, Task, EventPostService

### Community 46 - "avatar-crop.js"
Cohesion: 0.36
Nodes (9): attachEvents(), clampOffset(), draw(), exportCanvas(), getState(), initState(), open(), openFromInput() (+1 more)

### Community 47 - "Feur.razor"
Cohesion: 0.20
Nodes (9): OnInitializedAsync, AuthenticationStateProvider, GroupApiService, GroupMemberModel, GroupTermApiService, GroupTermModel, GroupTermsTab, PageTitle (+1 more)

### Community 48 - "LeaderDashboardView.razor"
Cohesion: 0.20
Nodes (9): Approve, GroupApiService, GroupMemberModel, Guid, NotificationModel, NotificationsPanel, Reject, RemoveMember (+1 more)

### Community 49 - "RenameEntraOidToSupabaseId"
Cohesion: 0.22
Nodes (7): MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder, RenameEntraOidToSupabaseId, Migration

### Community 50 - "EventParticipation"
Cohesion: 0.20
Nodes (10): DateTime, EventParticipation, Attended, Event, EventId, Id, RegisteredAt, Status (+2 more)

### Community 51 - "EventPost"
Cohesion: 0.20
Nodes (10): EventPost, Author, AuthorUserId, Body, CreatedAt, Event, EventId, Id (+2 more)

### Community 52 - "GroupPostService"
Cohesion: 0.40
Nodes (5): Guid, IDbContextFactory, List, Task, GroupPostService

### Community 53 - "Calendario.razor"
Cohesion: 0.22
Nodes (8): NextMonth, OnInitializedAsync, PrevMonth, CalendarApiService, CalendarMonthModel, PageTitle, SelectDay, route:/calendario

### Community 54 - "RenameSupabaseIdToEntraOid"
Cohesion: 0.25
Nodes (6): MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder, RenameSupabaseIdToEntraOid

### Community 55 - "AddProposedCategory"
Cohesion: 0.25
Nodes (6): MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder, AddProposedCategory

### Community 57 - "ReconnectModal.razor.js"
Cohesion: 0.32
Nodes (6): handleReconnectStateChanged(), reconnectModal, resumeButton, retry(), retryButton, retryWhenDocumentBecomesVisible()

### Community 58 - "Events.razor"
Cohesion: 0.25
Nodes (7): LoadAsync, OnInitializedAsync, OnSearch, EventApiService, EventModel, PageTitle, route:/events

### Community 59 - "Routes.razor"
Cohesion: 0.29
Nodes (6): AuthorizeRouteView, NotAuthorized, FocusOnNavigate, Found, RedirectToLogin, Router

### Community 60 - "AdminIndex.razor"
Cohesion: 0.29
Nodes (6): OnInitializedAsync, AdminApiService, AdminKpiGrid, NavigationManager, PageTitle, route:/admin

### Community 61 - ".BuildModel"
Cohesion: 0.29
Nodes (6): DateOnly, DateTime, Guid, ModelBuilder, AppDbContextModelSnapshot, ModelSnapshot

### Community 62 - "ClaimsPrincipalExtensions"
Cohesion: 0.38
Nodes (3): ClaimsPrincipal, ClaimsPrincipalExtensions, System.Security.Claims

### Community 63 - "Unreleased"
Cohesion: 0.33
Nodes (5): Feat, Fix, Perf, Refactor, Unreleased

### Community 64 - "App.razor"
Cohesion: 0.33
Nodes (5): HeadOutlet, ImportMap, ReconnectModal, ResourcePreloader, Routes

### Community 65 - "MisGrupos.razor"
Cohesion: 0.40
Nodes (4): OnInitializedAsync, MisGruposApiService, PageTitle, route:/mis-grupos

### Community 66 - "StudentDashboardView.razor"
Cohesion: 0.40
Nodes (4): GroupRegistrationRequestModel, Guid, NotificationModel, NotificationsPanel

### Community 67 - "MobileTabBar.razor"
Cohesion: 0.50
Nodes (3): Authorized, AuthorizeView, NavLink

### Community 68 - "Error.razor"
Cohesion: 0.50
Nodes (3): OnInitialized, PageTitle, System.Diagnostics

### Community 69 - "JoinButton.razor"
Cohesion: 0.50
Nodes (3): HandleJoin, OnParametersSet, GroupApiService

### Community 70 - "RsvpButton.razor"
Cohesion: 0.50
Nodes (3): HandleCancel, HandleRsvp, EventApiService

### Community 71 - "DashboardAdminModel"
Cohesion: 0.50
Nodes (3): DashboardAdminModel, EventsMonthDelta, PendingRequests

## Knowledge Gaps
- **535 isolated node(s):** `Microsoft.AspNetCore.Authorization`, `Microsoft.AspNetCore.Components.Authorization`, `Microsoft.AspNetCore.Components.Forms`, `AvatarUrl`, `Microsoft.AspNetCore.Components.Routing` (+530 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 712 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **15 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `AppDbContext` connect `AppDbContext` to `StudentGroupsHub.Services`, `Group`, `.GetByEntraOidAsync`, `UserService`, `LeadershipRequest`, `GroupRegistrationRequest`, `DashboardService`, `GroupTerm`, `EventService`, `Event`, `GroupApiService`, `User`, `Report`, `Membership`, `GroupPost`, `RoleAssignment`, `Notification`, `TermMember`, `EventPostService`, `EventParticipation`, `EventPost`, `GroupPostService`?**
  _High betweenness centrality (0.091) - this node is a cross-community bridge._
- **Why does `User` connect `User` to `Group`, `Report`, `UserService`, `Membership`, `LeadershipRequest`, `.GetByEntraOidAsync`, `GroupPost`, `GroupRegistrationRequest`, `Notification`, `RoleAssignment`, `Guid`, `TermMember`, `EventParticipation`, `EventPost`, `CurrentUserService`, `AppDbContext`, `Event`?**
  _High betweenness centrality (0.069) - this node is a cross-community bridge._
- **Why does `Event` connect `Event` to `Group`, `.GetByEntraOidAsync`, `Membership`, `GroupPost`, `DashboardService`, `Task`, `EventService`, `EventParticipation`, `EventPost`, `AppDbContext`, `User`?**
  _High betweenness centrality (0.046) - this node is a cross-community bridge._
- **What connects `Microsoft.AspNetCore.Authorization`, `Microsoft.AspNetCore.Components.Authorization`, `Microsoft.AspNetCore.Components.Forms` to the rest of the system?**
  _535 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `StudentGroupsHub.Services` be split into smaller, more focused modules?**
  _Cohesion score 0.06749482401656315 - nodes in this community are weakly interconnected._
- **Should `Group` be split into smaller, more focused modules?**
  _Cohesion score 0.06956521739130435 - nodes in this community are weakly interconnected._
- **Should `.GetByEntraOidAsync` be split into smaller, more focused modules?**
  _Cohesion score 0.0907103825136612 - nodes in this community are weakly interconnected._