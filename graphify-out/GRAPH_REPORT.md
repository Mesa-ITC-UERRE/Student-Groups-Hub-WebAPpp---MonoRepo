# Graph Report - Student-Groups-Hub-WebAPpp---MonoRepo  (2026-09-25)

## Corpus Check
- 163 files · ~83,110 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 10 file(s) not represented in the graph (top: .css 4, (none) 3, .toml 1)

## Summary
- 1564 nodes · 3035 edges · 134 communities (88 shown, 46 thin omitted)
- Extraction: 91% EXTRACTED · 9% INFERRED · 0% AMBIGUOUS · INFERRED: 286 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `978e6309`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Group
- GroupDetail.razor
- Membership
- Event
- DashboardService
- AppModels.cs
- AdminController
- .GetActiveUserIdAsync
- .TryAsync
- AdminGroups.razor
- Task
- StorageService
- GroupRegistrationRequest
- AdminApiService
- LeadershipRequest
- User
- Perfil.razor
- AdminUsers.razor
- EventDetail.razor
- AdminRequests.razor
- User
- .RequireActiveUserAsync
- AppDbContext
- StudentGroupsHub.Data.Migrations
- RenameEntraOidToSupabaseId
- EventModel
- UserService
- StudentGroupsHub.Services
- UserResponse
- RegisterGroup.razor
- AdminController.cs
- Azure Container Apps
- http
- Groups.razor
- _Imports.razor
- ApplyForLeadership.razor
- Home.razor
- GroupTermsTab.razor
- skills/README.md
- GroupTerm
- Notification
- Report
- CurrentUserService
- NavMenu.razor
- Timeline.razor
- EventPost
- InitialSchema
- AddLeadershipRequestsAndSeasonReset
- RoleAssignment
- StudentGroupsHub
- AddEventPosts
- StorageService.cs
- TermMember
- EventPostService
- GroupPostService
- avatar-crop.js
- Feur.razor
- LeaderDashboardView.razor
- EventParticipation
- GroupPost
- Calendario.razor
- MembershipsController
- AdminRoleHandler
- ReconnectModal.razor.js
- Events.razor
- Writing Guidelines for Postgres References
- Routes.razor
- AdminIndex.razor
- .BuildModel
- App.razor
- MisGrupos.razor
- StudentDashboardView.razor
- ClaimsPrincipalExtensions
- MobileTabBar.razor
- Error.razor
- JoinButton.razor
- RsvpButton.razor
- UserStateService
- MainLayout.razor
- NotificationsPanel.razor
- RedirectToLogin.razor
- NotFound.razor
- AdminDashboardView.razor
- FluentIcon.razor
- commit-msg
- pre-commit
- Supabase
- .GetByEntraOidAsync
- GroupTermService
- .IsActive
- Section Definitions
- RenameSupabaseIdToEntraOid
- NotificationService
- [0.1.3](https://github.com/supabase/agent-skills/compare/v0.1.2...v0.1.3) (2026-06-02)
- [1.2.0](https://github.com/supabase/agent-skills/compare/v1.1.1...v1.2.0) (2026-06-02)
- GroupSeasonService
- Supabase Postgres Best Practices
- Unreleased
- ControllerBase
- advanced-full-text-search.md
- advanced-jsonb-indexing.md
- conn-idle-timeout.md
- conn-limits.md
- conn-pooling.md
- conn-prepared-statements.md
- data-batch-inserts.md
- data-n-plus-one.md
- data-pagination.md
- data-upsert.md
- lock-advisory.md
- lock-deadlock-prevention.md
- lock-short-transactions.md
- lock-skip-locked.md
- monitor-explain-analyze.md
- monitor-pg-stat-statements.md
- monitor-vacuum-analyze.md
- query-composite-indexes.md
- query-covering-indexes.md
- query-index-types.md
- query-missing-indexes.md
- query-partial-indexes.md
- schema-constraints.md
- schema-data-types.md
- schema-foreign-key-indexes.md
- schema-lowercase-identifiers.md
- schema-partitioning.md
- schema-primary-keys.md
- security-privileges.md
- security-rls-basics.md
- security-rls-performance.md
- _template.md

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
- `Seguridad` --references--> `RoleAssignment`  [INFERRED]
  .agents/skills/repo-standards/SKILL.md → blazor/Models/Entities.cs
- `Autorización y BOLA/IDOR` --references--> `RoleAssignment`  [INFERRED]
  .agents/skills/security-review/SKILL.md → blazor/Models/Entities.cs
- `Commitizen and Conventional Versioning` --conceptually_related_to--> `issue-to-change`  [INFERRED]
  docs/12-versioning.md → README.md
- `Student Groups Hub` --references--> `Universidad Regiomontana (U-ERRE)`  [EXTRACTED]
  README.md → docs/01-project-overview.md
- `ARIA Collaboration Skill System` --references--> `issue-to-change`  [EXTRACTED]
  docs/13-collaboration-skills.md → README.md

## Import Cycles
- None detected.

## Communities (134 total, 46 thin omitted)

### Community 0 - "Group"
Cohesion: 0.07
Nodes (36): AllowAnonymous, Authorize, Guid, HttpGet, HttpPatch, IActionResult, Task, GroupsController (+28 more)

### Community 1 - "GroupDetail.razor"
Cohesion: 0.04
Nodes (44): CancelBannerPreview, CancelLogoCrop, CloseCreateEvent, DeletePost, LoadUserPermissionsAsync, OnAfterRenderAsync, OnBannerSelected, OnJoined (+36 more)

### Community 2 - "Membership"
Cohesion: 0.17
Nodes (15): Membership, Group, GroupId, Id, Notes, RequestedAt, RespondedAt, Status (+7 more)

### Community 3 - "Event"
Cohesion: 0.08
Nodes (40): AllowAnonymous, Authorize, Guid, HttpDelete, HttpGet, HttpPost, IActionResult, Task (+32 more)

### Community 4 - "DashboardService"
Cohesion: 0.12
Nodes (23): HttpGet, IActionResult, Task, DashboardController, DateTime, Guid, GroupRegistrationRequestResponse, GroupResponse (+15 more)

### Community 5 - "AppModels.cs"
Cohesion: 0.09
Nodes (28): DateTime, Guid, List, AdminUsersResponse, ApiError, BlazorCreateEventRequest, BlazorUpdateEventRequest, CalendarDayModel (+20 more)

### Community 6 - "AdminController"
Cohesion: 0.27
Nodes (11): Guid, HttpGet, HttpPatch, HttpPost, IActionResult, IDbContextFactory, List, Task (+3 more)

### Community 7 - ".GetActiveUserIdAsync"
Cohesion: 0.20
Nodes (8): GroupRegistrationRequestModel, StatusCss, StatusLabel, LeadershipRequestModel, StatusCss, StatusLabel, GroupRegistrationApiService, LeadershipRequestApiService

### Community 8 - ".TryAsync"
Cohesion: 0.18
Nodes (6): DashboardAdminModel, EventsMonthDelta, PendingRequests, DbSafe, Exception, Func

### Community 9 - "AdminGroups.razor"
Cohesion: 0.07
Nodes (28): Activate, ApproveMember, ConfirmSeasonReset, Deactivate, GoToPage, LoadAsync, LoadMembersAsync, OnInitializedAsync (+20 more)

### Community 10 - "Task"
Cohesion: 0.14
Nodes (8): GroupPostModel, AuthorInitial, TimeAgo, Guid, HashSet, Task, GroupPostApiService, Stream

### Community 11 - "StorageService"
Cohesion: 0.15
Nodes (12): EventPostModel, AuthorInitial, TimeAgo, EventPostApiService, StorageService, AuthToken, Bucket, SupabaseUrl (+4 more)

### Community 12 - "GroupRegistrationRequest"
Cohesion: 0.08
Nodes (33): Guid, HttpGet, HttpPatch, HttpPost, IActionResult, Task, GroupRegistrationRequestsController, Guid (+25 more)

### Community 13 - "AdminApiService"
Cohesion: 0.13
Nodes (10): AppDbContext&gt;, UserModel, DisplayLabel, Initial, IsAdmin, IsGroupLeader, Dictionary, IEnumerable (+2 more)

### Community 14 - "LeadershipRequest"
Cohesion: 0.13
Nodes (19): LeadershipRequest, ContactEmail, CreatedAt, DecisionNotes, Group, GroupId, Id, Reason (+11 more)

### Community 15 - "User"
Cohesion: 0.12
Nodes (17): Guid, GroupPostAuthor, Group, GroupId, User, UserId, User, AvatarUrl (+9 more)

### Community 16 - "Perfil.razor"
Cohesion: 0.08
Nodes (23): AdminDashboardView, CancelCrop, MarkAllRead, MarkRead, OnAfterRenderAsync, OnAvatarSelected, OnInitializedAsync, OnZoomChanged (+15 more)

### Community 17 - "AdminUsers.razor"
Cohesion: 0.09
Nodes (22): Activate, CancelPromoteToLeader, ConfirmPromoteToLeader, Deactivate, DemoteToStudent, GoToPage, LoadAsync, LoadLeaderGroupsAsync (+14 more)

### Community 18 - "EventDetail.razor"
Cohesion: 0.10
Nodes (20): CloseEditEvent, CloseGalleryImage, DeletePost, OnParametersSetAsync, OnPostImageSelected, OpenEditEvent, OpenGalleryImage, CurrentUserService (+12 more)

### Community 19 - "AdminRequests.razor"
Cohesion: 0.10
Nodes (19): CancelExpand, CancelLeadershipExpand, ConfirmApprove, ConfirmLeadershipApprove, ConfirmLeadershipReject, ConfirmReject, ExpandApprove, ExpandLeadershipApprove (+11 more)

### Community 20 - "User"
Cohesion: 0.07
Nodes (59): U-ERRE Design System, Universidad Regiomontana (U-ERRE), ASP.NET Core 8 Web API (historic architecture), Azure Static Web Apps (historic frontend hosting), React SPA (historic architecture), Event, EventParticipation, Group (+51 more)

### Community 21 - ".RequireActiveUserAsync"
Cohesion: 0.16
Nodes (10): DateOnly, GroupTermModel, DateRange, IsCurrent, StatusCss, StatusLabel, TermMemberModel, Initial (+2 more)

### Community 22 - "AppDbContext"
Cohesion: 0.11
Nodes (19): AppDbContext, EventParticipations, EventPosts, Events, GroupPostAuthors, GroupPosts, GroupRegistrationRequests, Groups (+11 more)

### Community 23 - "StudentGroupsHub.Data.Migrations"
Cohesion: 0.33
Nodes (6): StudentGroupsHub.Data.Migrations, microsoft_entityframeworkcore_infrastructure, microsoft_entityframeworkcore_migrations, microsoft_entityframeworkcore_storage_valueconversion, npgsql_entityframeworkcore_postgresql_metadata, system

### Community 24 - "RenameEntraOidToSupabaseId"
Cohesion: 0.12
Nodes (13): MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder, RenameEntraOidToSupabaseId, MigrationBuilder, DateOnly (+5 more)

### Community 25 - "EventModel"
Cohesion: 0.16
Nodes (15): EventModel, AttendanceSummary, DayLabel, EventDateLocal, HasCapacity, IsCanceled, IsPast, IsPublished (+7 more)

### Community 26 - "UserService"
Cohesion: 0.21
Nodes (10): Guid, HttpGet, HttpPatch, IActionResult, Task, UsersController, Guid, IDbContextFactory (+2 more)

### Community 27 - "StudentGroupsHub.Services"
Cohesion: 0.37
Nodes (5): StudentGroupsHub.Services, StudentGroupsHub.DTOs.Responses, StudentGroupsHub.Models, StudentGroupsHub.Data, microsoft_entityframeworkcore

### Community 28 - "UserResponse"
Cohesion: 0.50
Nodes (4): DateTime, Guid, ApiError, UserResponse

### Community 29 - "RegisterGroup.razor"
Cohesion: 0.12
Nodes (16): HandleSubmit, OnInitializedAsync, AuthenticationStateProvider, Authorized, AuthorizeView, DataAnnotationsValidator, EditForm, GroupApiService (+8 more)

### Community 30 - "AdminController.cs"
Cohesion: 0.25
Nodes (8): SetUserStatusRequest, StudentGroupsHub.Extensions, StudentGroupsHub.DTOs.Requests, StudentGroupsHub.Controllers, microsoft_aspnetcore_authorization, microsoft_aspnetcore_components_authorization, microsoft_aspnetcore_mvc, System.Security.Claims

### Community 32 - "http"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 33 - "Groups.razor"
Cohesion: 0.13
Nodes (14): ClearFilters, GradientFor, LoadGroupsAsync, OnInitializedAsync, OnSearchChanged, AuthenticationStateProvider, Authorized, AuthorizeView (+6 more)

### Community 34 - "_Imports.razor"
Cohesion: 0.09
Nodes (20): Microsoft.AspNetCore.Authorization, Microsoft.AspNetCore.Components.Authorization, Microsoft.AspNetCore.Components.Forms, microsoft_aspnetcore_authentication_jwtbearer, microsoft_aspnetcore_authentication_openidconnect, Microsoft.AspNetCore.Components.Routing, Microsoft.AspNetCore.Components.Web, Microsoft.AspNetCore.Components.Web.RenderMode (+12 more)

### Community 35 - "ApplyForLeadership.razor"
Cohesion: 0.12
Nodes (14): HandleSubmit, OnInitializedAsync, CurrentUserService, DataAnnotationsValidator, EditForm, GroupApiService, InputText, InputTextArea (+6 more)

### Community 36 - "Home.razor"
Cohesion: 0.15
Nodes (12): FormatCompact, HomeGroupItem, OnInitializedAsync, Authorized, AuthorizeView, CalendarApiService, DashboardService, MisGruposApiService (+4 more)

### Community 37 - "GroupTermsTab.razor"
Cohesion: 0.15
Nodes (12): CancelAddMember, CancelAddTerm, DeleteTerm, LoadAsync, OnInitializedAsync, AuthenticationStateProvider, GroupTermApiService, GroupTermModel (+4 more)

### Community 38 - "skills/README.md"
Cohesion: 0.04
Nodes (38): Documentation Maintainer, Matriz de impacto, Procedimiento, Fuente de verdad, Graphify Maintainer, Invariantes, Procedimiento, Definition of Done (+30 more)

### Community 39 - "GroupTerm"
Cohesion: 0.15
Nodes (13): GroupTerm, CreatedAt, EndDate, Group, GroupId, Id, IsCurrent, Label (+5 more)

### Community 40 - "Notification"
Cohesion: 0.17
Nodes (12): Notification, Body, CreatedAt, Href, Id, Kind, Read, ReferenceId (+4 more)

### Community 41 - "Report"
Cohesion: 0.15
Nodes (13): Report, CreatedAt, Details, Id, Reason, ReportedBy, ReportedByUserId, ReviewedAt (+5 more)

### Community 42 - "CurrentUserService"
Cohesion: 0.19
Nodes (8): CalendarApiService, DashboardApiService, NotificationApiService, AuthenticationStateProvider, ClaimsPrincipal, Guid, Task, CurrentUserService

### Community 43 - "NavMenu.razor"
Cohesion: 0.17
Nodes (11): Dispose, OnInitializedAsync, OnUserStateChanged, Authorized, AuthorizeView, NavigationManager, NavLink, NotAuthorized (+3 more)

### Community 44 - "Timeline.razor"
Cohesion: 0.17
Nodes (11): GroupTimelineEntry, LoadAsync, MatchesYear, OnInitializedAsync, EventApiService, EventModel, GroupTermApiService, GroupTermModel (+3 more)

### Community 45 - "EventPost"
Cohesion: 0.17
Nodes (11): ModelBuilder, EventPost, Author, AuthorUserId, Body, CreatedAt, Event, EventId (+3 more)

### Community 46 - "InitialSchema"
Cohesion: 0.18
Nodes (9): DateOnly, DateTime, Guid, MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder (+1 more)

### Community 47 - "AddLeadershipRequestsAndSeasonReset"
Cohesion: 0.18
Nodes (9): DateOnly, DateTime, Guid, MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder (+1 more)

### Community 48 - "RoleAssignment"
Cohesion: 0.17
Nodes (12): DateOnly, RoleAssignment, CreatedAt, DisplayRole, EndDate, Group, GroupId, Id (+4 more)

### Community 49 - "StudentGroupsHub"
Cohesion: 0.17
Nodes (12): StudentGroupsHub, net10.0, FluentValidation.AspNetCore (11.3.1), Microsoft.AspNetCore.Authentication.JwtBearer (10.0.9), Microsoft.EntityFrameworkCore (10.0.9), Microsoft.EntityFrameworkCore.Design (10.0.9), Microsoft.EntityFrameworkCore.Tools (10.0.9), Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore (10.0.9) (+4 more)

### Community 50 - "AddEventPosts"
Cohesion: 0.20
Nodes (8): DateTime, Guid, MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder, AddEventPosts

### Community 51 - "StorageService.cs"
Cohesion: 0.18
Nodes (8): GlobalExceptionHandlerExtensions, StudentGroupsHub.Middleware, IApplicationBuilder, IExceptionHandlerFeature, microsoft_aspnetcore_diagnostics, system_net_http_headers, system_text, system_text_json

### Community 52 - "TermMember"
Cohesion: 0.18
Nodes (11): TermMember, AvatarUrl, CreatedAt, DisplayName, Id, RoleLabel, SortOrder, Term (+3 more)

### Community 53 - "EventPostService"
Cohesion: 0.31
Nodes (7): Dictionary, Guid, IDbContextFactory, IEnumerable, List, Task, EventPostService

### Community 54 - "GroupPostService"
Cohesion: 0.38
Nodes (5): Guid, IDbContextFactory, List, Task, GroupPostService

### Community 55 - "avatar-crop.js"
Cohesion: 0.36
Nodes (9): attachEvents(), clampOffset(), draw(), exportCanvas(), getState(), initState(), open(), openFromInput() (+1 more)

### Community 56 - "Feur.razor"
Cohesion: 0.20
Nodes (9): OnInitializedAsync, AuthenticationStateProvider, GroupApiService, GroupMemberModel, GroupTermApiService, GroupTermModel, GroupTermsTab, PageTitle (+1 more)

### Community 57 - "LeaderDashboardView.razor"
Cohesion: 0.20
Nodes (9): Approve, GroupApiService, GroupMemberModel, Guid, NotificationModel, NotificationsPanel, Reject, RemoveMember (+1 more)

### Community 58 - "EventParticipation"
Cohesion: 0.20
Nodes (10): DateTime, EventParticipation, Attended, Event, EventId, Id, RegisteredAt, Status (+2 more)

### Community 59 - "GroupPost"
Cohesion: 0.20
Nodes (10): GroupPost, Author, AuthorUserId, Body, CreatedAt, Group, GroupId, Id (+2 more)

### Community 60 - "Calendario.razor"
Cohesion: 0.22
Nodes (8): NextMonth, OnInitializedAsync, PrevMonth, CalendarApiService, CalendarMonthModel, PageTitle, SelectDay, route:/calendario

### Community 61 - "MembershipsController"
Cohesion: 0.27
Nodes (10): AllowAnonymous, Guid, HttpDelete, HttpGet, HttpPatch, HttpPost, IActionResult, Task (+2 more)

### Community 62 - "AdminRoleHandler"
Cohesion: 0.29
Nodes (7): AuthorizationHandler, AuthorizationHandlerContext, Task, AdminRoleHandler, AdminRoleRequirement, IAuthorizationRequirement, IServiceScopeFactory

### Community 63 - "ReconnectModal.razor.js"
Cohesion: 0.32
Nodes (6): handleReconnectStateChanged(), reconnectModal, resumeButton, retry(), retryButton, retryWhenDocumentBecomesVisible()

### Community 64 - "Events.razor"
Cohesion: 0.25
Nodes (7): LoadAsync, OnInitializedAsync, OnSearch, EventApiService, EventModel, PageTitle, route:/events

### Community 65 - "Writing Guidelines for Postgres References"
Cohesion: 0.12
Nodes (15): 1. Concrete Transformation Patterns, 2. Error-First Structure, 3. Quantified Impact, 4. Self-Contained Examples, 5. Semantic Naming, Code Example Standards, Comments, Impact Level Guidelines (+7 more)

### Community 66 - "Routes.razor"
Cohesion: 0.29
Nodes (6): AuthorizeRouteView, NotAuthorized, FocusOnNavigate, Found, RedirectToLogin, Router

### Community 67 - "AdminIndex.razor"
Cohesion: 0.29
Nodes (6): OnInitializedAsync, AdminApiService, AdminKpiGrid, NavigationManager, PageTitle, route:/admin

### Community 68 - ".BuildModel"
Cohesion: 0.29
Nodes (6): DateOnly, DateTime, Guid, ModelBuilder, AppDbContextModelSnapshot, ModelSnapshot

### Community 69 - "App.razor"
Cohesion: 0.33
Nodes (5): HeadOutlet, ImportMap, ReconnectModal, ResourcePreloader, Routes

### Community 70 - "MisGrupos.razor"
Cohesion: 0.40
Nodes (4): OnInitializedAsync, MisGruposApiService, PageTitle, route:/mis-grupos

### Community 71 - "StudentDashboardView.razor"
Cohesion: 0.40
Nodes (4): GroupRegistrationRequestModel, Guid, NotificationModel, NotificationsPanel

### Community 73 - "MobileTabBar.razor"
Cohesion: 0.50
Nodes (3): Authorized, AuthorizeView, NavLink

### Community 74 - "Error.razor"
Cohesion: 0.50
Nodes (3): OnInitialized, PageTitle, System.Diagnostics

### Community 75 - "JoinButton.razor"
Cohesion: 0.50
Nodes (3): HandleJoin, OnParametersSet, GroupApiService

### Community 76 - "RsvpButton.razor"
Cohesion: 0.50
Nodes (3): HandleCancel, HandleRsvp, EventApiService

### Community 88 - "Supabase"
Cohesion: 0.13
Nodes (12): Fix suggestion, Source, What happened, Skill Feedback, Steps, Core Principles, Making and Committing Schema Changes, Reference Guides (+4 more)

### Community 90 - ".GetByEntraOidAsync"
Cohesion: 0.30
Nodes (8): Guid, HttpGet, HttpPatch, HttpPost, IActionResult, Task, LeadershipRequestsController, ReviewLeadershipRequest

### Community 91 - "GroupTermService"
Cohesion: 0.31
Nodes (6): DateOnly, Guid, IDbContextFactory, List, Task, GroupTermService

### Community 92 - ".IsActive"
Cohesion: 0.29
Nodes (6): Guid, HttpGet, HttpPatch, IActionResult, Task, NotificationsController

### Community 93 - "Section Definitions"
Cohesion: 0.20
Nodes (9): 1. Query Performance (query), 2. Connection Management (conn), 3. Security & RLS (security), 4. Schema Design (schema), 5. Concurrency & Locking (lock), 6. Data Access Patterns (data), 7. Monitoring & Diagnostics (monitor), 8. Advanced Features (advanced) (+1 more)

### Community 94 - "RenameSupabaseIdToEntraOid"
Cohesion: 0.25
Nodes (6): MigrationBuilder, DateOnly, DateTime, Guid, ModelBuilder, RenameSupabaseIdToEntraOid

### Community 95 - "NotificationService"
Cohesion: 0.39
Nodes (5): Guid, IDbContextFactory, List, Task, NotificationService

### Community 96 - "[0.1.3](https://github.com/supabase/agent-skills/compare/v0.1.2...v0.1.3) (2026-06-02)"
Cohesion: 0.25
Nodes (7): [0.1.3](https://github.com/supabase/agent-skills/compare/v0.1.2...v0.1.3) (2026-06-02), [0.1.4](https://github.com/supabase/agent-skills/compare/v0.1.3...v0.1.4) (2026-06-05), Bug Fixes, Bug Fixes, Changelog, Features, Features

### Community 97 - "[1.2.0](https://github.com/supabase/agent-skills/compare/v1.1.1...v1.2.0) (2026-06-02)"
Cohesion: 0.25
Nodes (7): [1.2.0](https://github.com/supabase/agent-skills/compare/v1.1.1...v1.2.0) (2026-06-02), [1.3.0](https://github.com/supabase/agent-skills/compare/v1.2.0...v1.3.0) (2026-06-05), Bug Fixes, Bug Fixes, Changelog, Features, Features

### Community 98 - "GroupSeasonService"
Cohesion: 0.43
Nodes (5): Guid, IDbContextFactory, IEnumerable, Task, GroupSeasonService

### Community 99 - "Supabase Postgres Best Practices"
Cohesion: 0.33
Nodes (5): How to Use, References, Rule Categories by Priority, Supabase Postgres Best Practices, When to Apply

### Community 100 - "Unreleased"
Cohesion: 0.33
Nodes (5): Feat, Fix, Perf, Refactor, Unreleased

### Community 101 - "ControllerBase"
Cohesion: 0.40
Nodes (4): HttpGet, IActionResult, HealthController, ControllerBase

## Knowledge Gaps
- **636 isolated node(s):** `ResourcePreloader`, `ImportMap`, `HeadOutlet`, `Routes`, `ReconnectModal` (+631 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 854 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **46 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `RoleAssignment` connect `RoleAssignment` to `Group`, `UserService`, `skills/README.md`, `GroupRegistrationRequest`, `EventPost`, `LeadershipRequest`, `User`, `AppDbContext`, `EventParticipation`?**
  _High betweenness centrality (0.092) - this node is a cross-community bridge._
- **Why does `AppDbContext` connect `AppDbContext` to `Group`, `Membership`, `Event`, `DashboardService`, `AdminController`, `GroupRegistrationRequest`, `LeadershipRequest`, `User`, `UserService`, `StudentGroupsHub.Services`, `GroupTerm`, `Notification`, `Report`, `EventPost`, `RoleAssignment`, `TermMember`, `EventPostService`, `GroupPostService`, `EventParticipation`, `GroupPost`, `GroupTermService`, `NotificationService`, `GroupSeasonService`?**
  _High betweenness centrality (0.088) - this node is a cross-community bridge._
- **What connects `ResourcePreloader`, `ImportMap`, `HeadOutlet` to the rest of the system?**
  _636 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Group` be split into smaller, more focused modules?**
  _Cohesion score 0.07272727272727272 - nodes in this community are weakly interconnected._
- **Should `GroupDetail.razor` be split into smaller, more focused modules?**
  _Cohesion score 0.044444444444444446 - nodes in this community are weakly interconnected._
- **Should `Event` be split into smaller, more focused modules?**
  _Cohesion score 0.07596153846153846 - nodes in this community are weakly interconnected._
- **Should `DashboardService` be split into smaller, more focused modules?**
  _Cohesion score 0.12310606060606061 - nodes in this community are weakly interconnected._