# Graph Report - Student-Groups-Hub-WebAPpp---MonoRepo  (2026-09-25)

## Corpus Check
- 130 files · ~74,132 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 10 file(s) not represented in the graph (top: .css 4, (none) 3, .toml 1)

## Summary
- 1571 nodes · 3393 edges · 103 communities (87 shown, 16 thin omitted)
- Extraction: 89% EXTRACTED · 11% INFERRED · 0% AMBIGUOUS · INFERRED: 388 edges (avg confidence: 0.81)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `c05951df`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- _Imports.razor
- GroupService
- MembershipsController
- AdminController
- AppModels.cs
- LeadershipRequest
- GroupDetail.razor
- .GetByEntraOidAsync
- User
- DashboardService
- Task
- EventModel
- AdminGroups.razor
- GroupTerm
- Event
- Perfil.razor
- AdminUsers.razor
- EventDetail.razor
- .GetActiveUserIdAsync
- AdminRequests.razor
- CurrentUserService
- AppDbContext
- StudentGroupsHub.Data.Migrations
- GlobalExceptionHandler.cs
- StudentGroupsHub.Services
- Membership
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
- .CreateDbFactory
- InitialSchema
- AddLeadershipRequestsAndSeasonReset
- GroupPost
- RoleAssignment
- Notification
- StudentGroupsHub.csproj
- .BuildTargetModel
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
- .BuildTargetModel
- .BuildTargetModel
- Guid
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
- .TryAsync
- JoinButton.razor
- RsvpButton.razor
- UserService
- MainLayout.razor
- NotificationsPanel.razor
- RedirectToLogin.razor
- NotFound.razor
- AdminDashboardView.razor
- FluentIcon.razor
- StudentGroupsHub.Tests.csproj
- U-ERRE Design System
- Universidad Regiomontana (U-ERRE)
- UI/UX Design System
- Azure Container Apps
- Commitizen and Conventional Versioning
- .User
- .Update
- GroupTermModel
- AdminController.cs
- Group
- RecordingJsRuntime
- .UpdateMe
- .Review_RejectsMembershipFromAnotherGroup
- GroupApiService
- GroupTermService
- ActiveUserHandler
- .BuildSignInUrl
- UserErrorService
- PostgreSqlCollection.cs
- .BuildTargetModel
- UserStateService
- LoginLink.razor

## God Nodes (most connected - your core abstractions)
1. `AppDbContext` - 61 edges
2. `User` - 47 edges
3. `Event` - 44 edges
4. `StudentGroupsHub.Services` - 44 edges
5. `Group` - 40 edges
6. `UserService` - 38 edges
7. `EventService` - 37 edges
8. `GroupService` - 33 edges
9. `GroupRegistrationRequest` - 30 edges
10. `Membership` - 30 edges

## Surprising Connections (you probably didn't know these)
- `TestDbContextFactory` --references--> `AppDbContext`  [EXTRACTED]
  tests/StudentGroupsHub.Tests/Infrastructure/PostgreSqlFixture.cs → blazor/Data/AppDbContext.cs
- `AdminController` --references--> `AppDbContext`  [EXTRACTED]
  blazor/Controllers/AdminController.cs → blazor/Data/AppDbContext.cs
- `AdminController` --references--> `DashboardService`  [EXTRACTED]
  blazor/Controllers/AdminController.cs → blazor/Services/DashboardService.cs
- `AdminController` --references--> `NotificationService`  [EXTRACTED]
  blazor/Controllers/AdminController.cs → blazor/Services/NotificationService.cs
- `AdminController` --references--> `UserService`  [EXTRACTED]
  blazor/Controllers/AdminController.cs → blazor/Services/UserService.cs

## Import Cycles
- None detected.

## Communities (103 total, 16 thin omitted)

### Community 0 - "_Imports.razor"
Cohesion: 0.07
Nodes (23): Microsoft.AspNetCore.Authorization, Microsoft.AspNetCore.Components.Forms, Program, StudentGroupsHub.Tests.Safety, microsoft_aspnetcore_authentication_jwtbearer, microsoft_aspnetcore_authentication_openidconnect, Microsoft.AspNetCore.Components.Authorization, Microsoft.AspNetCore.Components.Routing (+15 more)

### Community 1 - "GroupService"
Cohesion: 0.18
Nodes (11): MisGruposApiService, Dictionary, Guid, HashSet, IDbContextFactory, IEnumerable, List, Task (+3 more)

### Community 2 - "MembershipsController"
Cohesion: 0.26
Nodes (10): AllowAnonymous, Guid, HttpDelete, HttpGet, HttpPatch, HttpPost, IActionResult, Task (+2 more)

### Community 3 - "AdminController"
Cohesion: 0.16
Nodes (18): Guid, HttpGet, HttpPatch, HttpPost, IActionResult, IDbContextFactory, List, Task (+10 more)

### Community 4 - "AppModels.cs"
Cohesion: 0.11
Nodes (26): DateOnly, DateTime, Guid, List, AdminUsersResponse, ApiError, BlazorCreateEventRequest, BlazorUpdateEventRequest (+18 more)

### Community 5 - "LeadershipRequest"
Cohesion: 0.09
Nodes (31): Guid, HttpGet, HttpPatch, HttpPost, IActionResult, Task, LeadershipRequestsController, Guid (+23 more)

### Community 6 - "GroupDetail.razor"
Cohesion: 0.04
Nodes (44): CancelBannerPreview, CancelLogoCrop, CloseCreateEvent, DeletePost, LoadUserPermissionsAsync, OnAfterRenderAsync, OnBannerSelected, OnJoined (+36 more)

### Community 7 - ".GetByEntraOidAsync"
Cohesion: 0.07
Nodes (39): Guid, HttpGet, HttpPatch, HttpPost, IActionResult, Task, GroupRegistrationRequestsController, HttpGet (+31 more)

### Community 8 - "User"
Cohesion: 0.10
Nodes (43): ASP.NET Core 8 Web API (historic architecture), Azure Static Web Apps (historic frontend hosting), React SPA (historic architecture), Event, EventParticipation, Group, GroupRegistrationRequest, Membership (+35 more)

### Community 9 - "DashboardService"
Cohesion: 0.10
Nodes (27): HttpGet, IActionResult, Task, DashboardController, DateTime, Guid, GroupRegistrationRequestResponse, GroupResponse (+19 more)

### Community 10 - "Task"
Cohesion: 0.22
Nodes (10): AppDbContext&gt;, UserModel, DisplayLabel, Initial, IsAdmin, IsGroupLeader, IEnumerable, Task (+2 more)

### Community 11 - "EventModel"
Cohesion: 0.15
Nodes (14): EventModel, AttendanceSummary, DayLabel, EventDateLocal, HasCapacity, IsCanceled, IsPast, IsPublished (+6 more)

### Community 12 - "AdminGroups.razor"
Cohesion: 0.07
Nodes (29): Activate, ApproveMember, ConfirmSeasonReset, Deactivate, GoToPage, LoadAsync, LoadMembersAsync, OnInitializedAsync (+21 more)

### Community 13 - "GroupTerm"
Cohesion: 0.15
Nodes (13): GroupTerm, CreatedAt, EndDate, Group, GroupId, Id, IsCurrent, Label (+5 more)

### Community 14 - "Event"
Cohesion: 0.06
Nodes (49): AllowAnonymous, Authorize, Guid, HttpDelete, HttpGet, HttpPost, IActionResult, Task (+41 more)

### Community 15 - "Perfil.razor"
Cohesion: 0.08
Nodes (24): AdminDashboardView, CancelCrop, MarkAllRead, MarkRead, OnAfterRenderAsync, OnAvatarSelected, OnInitializedAsync, OnZoomChanged (+16 more)

### Community 16 - "AdminUsers.razor"
Cohesion: 0.08
Nodes (23): Activate, CancelPromoteToLeader, ConfirmPromoteToLeader, Deactivate, DemoteToStudent, GoToPage, LoadAsync, LoadLeaderGroupsAsync (+15 more)

### Community 17 - "EventDetail.razor"
Cohesion: 0.09
Nodes (22): CloseEditEvent, CloseGalleryImage, DeletePost, OnParametersSetAsync, OnPostImageSelected, OpenEditEvent, OpenGalleryImage, CurrentUserService (+14 more)

### Community 18 - ".GetActiveUserIdAsync"
Cohesion: 0.12
Nodes (14): GroupRegistrationRequestModel, StatusCss, StatusLabel, LeadershipRequestModel, StatusCss, StatusLabel, GroupRegistrationApiService, LeadershipRequestApiService (+6 more)

### Community 19 - "AdminRequests.razor"
Cohesion: 0.10
Nodes (19): CancelExpand, CancelLeadershipExpand, ConfirmApprove, ConfirmLeadershipApprove, ConfirmLeadershipReject, ConfirmReject, ExpandApprove, ExpandLeadershipApprove (+11 more)

### Community 20 - "CurrentUserService"
Cohesion: 0.11
Nodes (17): CalendarApiService, EventPostApiService, UserApiService, AuthenticationStateProvider, ClaimsPrincipal, Task, CurrentUserService, ILogger (+9 more)

### Community 21 - "AppDbContext"
Cohesion: 0.11
Nodes (19): DbContextOptions, AppDbContext, EventParticipations, EventPosts, Events, GroupPostAuthors, GroupPosts, GroupRegistrationRequests (+11 more)

### Community 22 - "StudentGroupsHub.Data.Migrations"
Cohesion: 0.21
Nodes (10): DateTime, Guid, MigrationBuilder, AddEventPosts, StudentGroupsHub.Data.Migrations, microsoft_entityframeworkcore_infrastructure, microsoft_entityframeworkcore_migrations, microsoft_entityframeworkcore_storage_valueconversion (+2 more)

### Community 23 - "GlobalExceptionHandler.cs"
Cohesion: 0.12
Nodes (12): OnInitialized, PageTitle, GlobalExceptionHandlerExtensions, StudentGroupsHub.Middleware, IApplicationBuilder, IExceptionHandlerFeature, ILoggerFactory, microsoft_aspnetcore_diagnostics (+4 more)

### Community 24 - "StudentGroupsHub.Services"
Cohesion: 0.16
Nodes (12): StudentGroupsHub.Services, StudentGroupsHub.DTOs.Responses, StudentGroupsHub.Models, StudentGroupsHub.Tests.Security, StudentGroupsHub.Tests.Infrastructure, StudentGroupsHub.Data, StudentGroupsHub.Tests.Privacy, microsoft_entityframeworkcore (+4 more)

### Community 25 - "Membership"
Cohesion: 0.16
Nodes (15): Membership, Group, GroupId, Id, Notes, RequestedAt, RespondedAt, Status (+7 more)

### Community 26 - "RegisterGroup.razor"
Cohesion: 0.11
Nodes (18): HandleSubmit, OnInitializedAsync, AuthenticationStateProvider, Authorized, AuthorizeView, DataAnnotationsValidator, EditForm, GroupApiService (+10 more)

### Community 27 - "User"
Cohesion: 0.12
Nodes (17): Guid, GroupPostAuthor, Group, GroupId, User, UserId, User, AvatarUrl (+9 more)

### Community 28 - "http"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 29 - "Groups.razor"
Cohesion: 0.12
Nodes (15): ClearFilters, GradientFor, LoadGroupsAsync, OnInitializedAsync, OnSearchChanged, AuthenticationStateProvider, Authorized, AuthorizeView (+7 more)

### Community 30 - "ApplyForLeadership.razor"
Cohesion: 0.12
Nodes (15): HandleSubmit, OnInitializedAsync, CurrentUserService, DataAnnotationsValidator, EditForm, GroupApiService, InputText, InputTextArea (+7 more)

### Community 31 - "Home.razor"
Cohesion: 0.14
Nodes (13): FormatCompact, HomeGroupItem, OnInitializedAsync, Authorized, AuthorizeView, CalendarApiService, DashboardService, LoginLink (+5 more)

### Community 32 - "GroupTermsTab.razor"
Cohesion: 0.13
Nodes (14): CancelAddMember, CancelAddTerm, DeleteTerm, LoadAsync, OnInitializedAsync, AuthenticationStateProvider, GroupTermApiService, GroupTermModel (+6 more)

### Community 33 - "Report"
Cohesion: 0.15
Nodes (13): Report, CreatedAt, Details, Id, Reason, ReportedBy, ReportedByUserId, ReviewedAt (+5 more)

### Community 34 - "NavMenu.razor"
Cohesion: 0.15
Nodes (12): Dispose, OnInitializedAsync, OnUserStateChanged, Authorized, AuthorizeView, LoginLink, NavigationManager, NavLink (+4 more)

### Community 35 - "Timeline.razor"
Cohesion: 0.15
Nodes (12): GroupTimelineEntry, LoadAsync, MatchesYear, OnInitializedAsync, EventApiService, EventModel, GroupTermApiService, GroupTermModel (+4 more)

### Community 36 - ".CreateDbFactory"
Cohesion: 0.16
Nodes (15): IAsyncLifetime, PostgreSqlContainer, DbContextOptions, IDbContextFactory, Task, PostgreSqlFixture, TestDbContextFactory, EventResourceAuthorizationTests (+7 more)

### Community 37 - "InitialSchema"
Cohesion: 0.18
Nodes (9): DateOnly, DateTime, Guid, MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder (+1 more)

### Community 38 - "AddLeadershipRequestsAndSeasonReset"
Cohesion: 0.18
Nodes (9): DateOnly, DateTime, Guid, MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder (+1 more)

### Community 39 - "GroupPost"
Cohesion: 0.20
Nodes (10): GroupPost, Author, AuthorUserId, Body, CreatedAt, Group, GroupId, Id (+2 more)

### Community 40 - "RoleAssignment"
Cohesion: 0.17
Nodes (12): DateOnly, RoleAssignment, CreatedAt, DisplayRole, EndDate, Group, GroupId, Id (+4 more)

### Community 41 - "Notification"
Cohesion: 0.15
Nodes (12): Notification, Body, CreatedAt, Href, Id, Kind, Read, ReferenceId (+4 more)

### Community 42 - "StudentGroupsHub.csproj"
Cohesion: 0.17
Nodes (11): net10.0, FluentValidation.AspNetCore (11.3.1), Microsoft.AspNetCore.Authentication.JwtBearer (10.0.9), Microsoft.EntityFrameworkCore (10.0.9), Microsoft.EntityFrameworkCore.Design (10.0.9), Microsoft.EntityFrameworkCore.Tools (10.0.9), Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore (10.0.9), Microsoft.Identity.Web (4.10.0) (+3 more)

### Community 43 - ".BuildTargetModel"
Cohesion: 0.40
Nodes (4): DateOnly, DateTime, Guid, ModelBuilder

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
Nodes (9): OnInitializedAsync, CurrentUserService, GroupApiService, GroupTermApiService, GroupTermModel, GroupTermsTab, PageTitle, PublicGroupMemberModel (+1 more)

### Community 48 - "LeaderDashboardView.razor"
Cohesion: 0.18
Nodes (10): Approve, GroupApiService, GroupMemberModel, Guid, NotificationModel, NotificationsPanel, UserConfirmationService, Reject (+2 more)

### Community 49 - "RenameEntraOidToSupabaseId"
Cohesion: 0.19
Nodes (7): MigrationBuilder, RenameEntraOidToSupabaseId, MigrationBuilder, RenameSupabaseIdToEntraOid, MigrationBuilder, AddProposedCategory, Migration

### Community 50 - "EventParticipation"
Cohesion: 0.20
Nodes (10): DateTime, EventParticipation, Attended, Event, EventId, Id, RegisteredAt, Status (+2 more)

### Community 51 - "EventPost"
Cohesion: 0.17
Nodes (11): ModelBuilder, EventPost, Author, AuthorUserId, Body, CreatedAt, Event, EventId (+3 more)

### Community 52 - "GroupPostService"
Cohesion: 0.38
Nodes (5): Guid, IDbContextFactory, List, Task, GroupPostService

### Community 53 - "Calendario.razor"
Cohesion: 0.22
Nodes (8): NextMonth, OnInitializedAsync, PrevMonth, CalendarApiService, CalendarMonthModel, PageTitle, SelectDay, route:/calendario

### Community 54 - ".BuildTargetModel"
Cohesion: 0.40
Nodes (4): DateOnly, DateTime, Guid, ModelBuilder

### Community 55 - ".BuildTargetModel"
Cohesion: 0.40
Nodes (4): DateOnly, DateTime, Guid, ModelBuilder

### Community 56 - "Guid"
Cohesion: 0.16
Nodes (6): GroupPostModel, AuthorInitial, TimeAgo, Guid, HashSet, GroupPostApiService

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

### Community 68 - ".TryAsync"
Cohesion: 0.15
Nodes (10): NotificationModel, PublicGroupMemberModel, Initial, Dictionary, Exception, List, DashboardApiService, DbSafe (+2 more)

### Community 69 - "JoinButton.razor"
Cohesion: 0.40
Nodes (4): HandleJoin, OnParametersSet, GroupApiService, UserErrorService

### Community 70 - "RsvpButton.razor"
Cohesion: 0.33
Nodes (5): HandleCancel, HandleRsvp, EventApiService, UserConfirmationService, UserErrorService

### Community 71 - "UserService"
Cohesion: 0.23
Nodes (9): Guid, HashSet, IDbContextFactory, Task, UserService, UserVisibleException, InvalidOperationException, Fact (+1 more)

### Community 80 - "StudentGroupsHub.Tests.csproj"
Cohesion: 0.18
Nodes (9): blazor_studentgroupshub, Microsoft.AspNetCore.Mvc.Testing (10.0.12), Microsoft.NET.Test.Sdk (18.10.1), System.Security.Cryptography.Xml (10.0.12), Testcontainers.PostgreSql (4.15.0), xunit (2.9.3), xunit.runner.visualstudio (3.1.5), Microsoft.NET.Sdk (+1 more)

### Community 86 - ".User"
Cohesion: 0.23
Nodes (11): AuthenticationState, AuthenticationStateProvider, ActiveUserRequirement, ServiceProvider, ClaimsPrincipal, Fact, IDbContextFactory, IServiceScopeFactory (+3 more)

### Community 87 - ".Update"
Cohesion: 0.24
Nodes (9): AllowAnonymous, Authorize, Guid, HttpGet, HttpPatch, IActionResult, Task, GroupsController (+1 more)

### Community 88 - "GroupTermModel"
Cohesion: 0.17
Nodes (9): GroupTermModel, DateRange, IsCurrent, StatusCss, StatusLabel, TermMemberModel, Initial, DateOnly (+1 more)

### Community 89 - "AdminController.cs"
Cohesion: 0.39
Nodes (5): StudentGroupsHub.Extensions, StudentGroupsHub.DTOs.Requests, StudentGroupsHub.Controllers, microsoft_aspnetcore_authorization, microsoft_aspnetcore_mvc

### Community 90 - "Group"
Cohesion: 0.12
Nodes (15): Group, BannerUrl, Category, ContactEmail, ContactInfo, CreatedAt, CreatedBy, CreatedByUserId (+7 more)

### Community 91 - "RecordingJsRuntime"
Cohesion: 0.14
Nodes (12): IJSRuntime, ValueTask, UserConfirmationService, CancellationToken, IJSRuntime, Fact, Task, ValueTask (+4 more)

### Community 92 - ".UpdateMe"
Cohesion: 0.21
Nodes (10): Guid, HttpGet, HttpPatch, IActionResult, Task, UsersController, DateTime, Guid (+2 more)

### Community 93 - ".Review_RejectsMembershipFromAnotherGroup"
Cohesion: 0.15
Nodes (10): Fact, PublicMemberContractTests, Fact, Guid, Task, GroupSeasonResetTests, InlineData, Task (+2 more)

### Community 94 - "GroupApiService"
Cohesion: 0.26
Nodes (6): MembershipModel, DisplayLabel, Initial, StatusCss, StatusLabel, GroupApiService

### Community 95 - "GroupTermService"
Cohesion: 0.31
Nodes (6): DateOnly, Guid, IDbContextFactory, List, Task, GroupTermService

### Community 96 - "ActiveUserHandler"
Cohesion: 0.29
Nodes (8): AuthorizationHandler, AuthorizationHandlerContext, IServiceScopeFactory, Task, ActiveUserHandler, AdminRoleHandler, AdminRoleRequirement, IAuthorizationRequirement

### Community 97 - ".BuildSignInUrl"
Cohesion: 0.31
Nodes (4): AuthenticationNavigation, Fact, AuthenticationNavigationTests, Uri

### Community 98 - "UserErrorService"
Cohesion: 0.28
Nodes (6): Exception, ILogger, UserErrorService, Fact, InvalidOperationException, ErrorContractTests

### Community 99 - "PostgreSqlCollection.cs"
Cohesion: 0.33
Nodes (3): ICollectionFixture, PostgreSqlCollection, xunit

### Community 100 - ".BuildTargetModel"
Cohesion: 0.40
Nodes (4): DateOnly, DateTime, Guid, ModelBuilder

## Knowledge Gaps
- **547 isolated node(s):** `ResourcePreloader`, `ImportMap`, `HeadOutlet`, `Routes`, `ReconnectModal` (+542 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 782 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **16 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `StudentGroupsHub.Services` connect `StudentGroupsHub.Services` to `_Imports.razor`, `.BuildSignInUrl`, `UserStateService`, `UserService`, `CurrentUserService`, `GlobalExceptionHandler.cs`, `AdminController.cs`, `RecordingJsRuntime`?**
  _High betweenness centrality (0.081) - this node is a cross-community bridge._
- **Why does `AppDbContext` connect `AppDbContext` to `GroupService`, `AdminController`, `LeadershipRequest`, `.GetByEntraOidAsync`, `DashboardService`, `GroupTerm`, `Event`, `.GetActiveUserIdAsync`, `StudentGroupsHub.Services`, `Membership`, `User`, `Report`, `.CreateDbFactory`, `GroupPost`, `RoleAssignment`, `Notification`, `TermMember`, `EventPostService`, `EventParticipation`, `EventPost`, `GroupPostService`, `UserService`, `.User`, `Group`, `GroupTermService`?**
  _High betweenness centrality (0.077) - this node is a cross-community bridge._
- **Why does `User` connect `User` to `AdminController`, `LeadershipRequest`, `.GetByEntraOidAsync`, `Task`, `Event`, `CurrentUserService`, `AppDbContext`, `Membership`, `Report`, `GroupPost`, `RoleAssignment`, `Notification`, `TermMember`, `EventParticipation`, `EventPost`, `Guid`, `UserService`, `.User`, `Group`, `.UpdateMe`?**
  _High betweenness centrality (0.072) - this node is a cross-community bridge._
- **What connects `ResourcePreloader`, `ImportMap`, `HeadOutlet` to the rest of the system?**
  _547 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `_Imports.razor` be split into smaller, more focused modules?**
  _Cohesion score 0.07407407407407407 - nodes in this community are weakly interconnected._
- **Should `AppModels.cs` be split into smaller, more focused modules?**
  _Cohesion score 0.11494252873563218 - nodes in this community are weakly interconnected._
- **Should `LeadershipRequest` be split into smaller, more focused modules?**
  _Cohesion score 0.08502415458937199 - nodes in this community are weakly interconnected._