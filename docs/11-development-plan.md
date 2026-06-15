# 11 — Development Plan (Updated: Blazor Stack)

## Stack Summary

| Concern | Technology |
|---|---|
| Frontend | **Blazor Server (.NET 10)** — InteractiveServer render mode |
| Auth | **Microsoft Entra ID** via `Microsoft.Identity.Web` |
| UI | **Bootstrap 5** + custom U-ERRE CSS variables |
| HTTP | **HttpClient** typed services → REST API |
| Backend | **ASP.NET Core 10** Web API (C#) — separate project |
| ORM | **Entity Framework Core 10** + Npgsql |
| Database | **Supabase (PostgreSQL)** |
| Email | **Resend** |
| Deployment | **Azure App Service** (both Blazor + backend) |
| Versioning | **Semantic Versioning** via Commitizen + conventional commits |

---

## Current Status (as of Phase 2 completion)

### Backend (`student-groups-hub/backend/`) ✅ Phase 0–2 done
- [x] ASP.NET Core 10 Web API running on `localhost:8080`
- [x] EF Core 10 + Npgsql, 9 tables in Supabase DB
- [x] Global error handler, health check endpoint
- [x] `GET/POST/PATCH /api/groups` + `/api/group-registration-requests`
- [x] `GET/PATCH /api/users/me`
- [ ] JWT validation uses Supabase JWKS — **needs revert to Entra OIDC**
- [ ] CORS not updated for Blazor port 7013

### Blazor (`StudentGroupsBlazor/`) ✅ Phase 0–2 done
- [x] Entra ID auth working (`Microsoft.Identity.Web`)
- [x] Home page — matches React prototype (hero, floating cards, events, stats, CTA)
- [x] Groups listing (search + category filters)
- [x] Group detail (members list)
- [x] Register group form
- [x] Dashboard (basic — notifications + requests)
- [x] JoinButton component (not yet wired to live API)
- [ ] JWT scope `access_as_user` not yet exposed in Entra

### Database ✅ Live in Supabase
- [x] All 9 tables created, migrations applied
- [x] 8 groups seeded (Robótica, Ensamble Musical, etc.)

---

## Phase Overview

| Phase | Name | Status | Goal |
|---|---|---|---|
| 0 | Foundation | ✅ Done | Repos, schema, auth scaffold |
| 1 | Auth + Users | ✅ Done | Login, user upsert, profile |
| 2 | Groups | ✅ Done | Group listing, detail, registration |
| 3 | Membership | 🔄 Next | Join requests, approve/reject |
| 4 | Events | ⏳ Pending | Event CRUD, listing, RSVP |
| 5 | Dashboards | ⏳ Pending | Role-specific dashboards |
| 6 | Notifications | ⏳ Pending | In-app bell + Resend email |
| 7 | Admin Panel | ⏳ Pending | User mgmt, group approval, KPIs |
| 8 | Polish | ⏳ Pending | Loading states, accessibility, errors |
| 9 | Stabilization | ⏳ Pending | QA, Azure deploy, v1.0.0 |

---

## Immediate Fixes (before Phase 3)

### Backend
- [ ] Revert JWT validation from Supabase JWKS → Entra OIDC
- [ ] Update CORS: add `https://localhost:7013` to allowed origins
- [ ] Update `ClaimsPrincipalExtensions`: use `oid` claim (Entra) instead of `sub`
- [ ] Add EF Core migration: rename `SupabaseId` → `EntraOid` on `users` table

### Entra portal
- [ ] Expose API scope: `api://44996e21.../access_as_user`
- [ ] Add Web platform redirect: `https://localhost:7013/signin-oidc`

---

## Phase 3 — Membership

### Backend tasks
- [ ] `POST /api/groups/{id}/join` — create pending membership
- [ ] `GET /api/groups/{id}/members` — accepted members
- [ ] `GET /api/groups/{id}/memberships/pending` — pending (leader/admin)
- [ ] `PATCH /api/groups/{id}/memberships/{mid}/approve`
- [ ] `PATCH /api/groups/{id}/memberships/{mid}/reject`
- [ ] `DELETE /api/groups/{id}/members/{userId}` — remove member
- [ ] `MembershipService` + `MembershipsController`
- [ ] Response DTOs: `MembershipResponse`, `MembershipListResponse`

### Blazor tasks
- [ ] `MembershipApiService` in `ApiServices.cs`
- [ ] `MembershipModel` in `AppModels.cs`
- [ ] Wire `JoinButton` to live `POST /api/groups/{id}/join`
- [ ] Group detail: show membership status (pending/member badge)
- [ ] Leader: `/leader/groups/{id}/members` — member management page
- [ ] Inline approve/reject on pending requests
- [ ] Dashboard: "Mis grupos" section showing accepted memberships

---

## Phase 4 — Events

### Backend tasks
- [ ] `GET /api/events` — all upcoming published events
- [ ] `GET /api/groups/{id}/events`
- [ ] `GET /api/events/{id}`
- [ ] `POST /api/groups/{id}/events` (leader/admin)
- [ ] `PUT /api/groups/{id}/events/{eid}` (leader/admin)
- [ ] `DELETE /api/groups/{id}/events/{eid}` — cancel
- [ ] `POST /api/events/{id}/rsvp` — upsert RSVP
- [ ] `DELETE /api/events/{id}/rsvp`
- [ ] `GET /api/events/{id}/rsvp/all` (leader/admin)
- [ ] `EventService` + `EventsController` + `EventRsvpController`

### Blazor tasks
- [ ] `EventModel`, `EventRsvpModel` in `AppModels.cs`
- [ ] `EventApiService` in `ApiServices.cs`
- [ ] `/events` — events listing page (cross-group, with filters)
- [ ] `/events/{id}` — event detail page
- [ ] `RsvpButton` component (going/not going/maybe states)
- [ ] Group detail: events tab
- [ ] Leader: event creation form
- [ ] Replace hardcoded homepage events with live API data

---

## Phase 5 — Dashboards

### Backend tasks
- [ ] `GET /api/dashboard/student` — joined groups + requests + upcoming events
- [ ] `GET /api/dashboard/leader` — managed groups + pending requests + events
- [ ] `GET /api/dashboard/admin` — platform stats + pending approvals + activity

### Blazor tasks
- [ ] `StudentDashboard.razor` component — groups, requests, upcoming events
- [ ] `LeaderDashboard.razor` component — inline approve/reject requests
- [ ] `AdminDashboard.razor` component — stat cards + activity feed
- [ ] Route `/dashboard` to correct component based on user role
- [ ] `StatCard` shared component

---

## Phase 6 — Notifications

### Backend tasks
- [ ] `NotificationService` — trigger on: membership approved/rejected, group approved/rejected, event updated/canceled
- [ ] `GET /api/users/me/notifications`
- [ ] `PATCH /api/notifications/{id}/read`
- [ ] `PATCH /api/users/me/notifications/read-all`
- [ ] Resend SDK integration — email templates (Spanish)

### Blazor tasks
- [ ] `NotificationBell` component in `NavMenu`
- [ ] Notification dropdown (last 10, click to navigate)
- [ ] Gold badge with unread count
- [ ] "Marcar todo como leído" button
- [ ] Toast/snackbar for action confirmations

---

## Phase 7 — Admin Panel

### Backend tasks
- [ ] `GET /api/users` (admin) — paginated + search + role filter
- [ ] `PATCH /api/users/{id}/role` (admin)
- [ ] `PATCH /api/users/{id}/status` (admin)
- [ ] `GET /api/reports` (open reports list)
- [ ] `POST /api/reports` (file a report)
- [ ] `GET /api/reports/metrics` (KPI data)
- [ ] `POST/DELETE /api/groups/{id}/roles` (assign leader)

### Blazor tasks
- [ ] `/admin` — admin dashboard (KPI cards)
- [ ] `/admin/users` — user management table
- [ ] `/admin/groups` — all groups table (approve/deactivate)
- [ ] `/admin/requests` — pending group requests
- [ ] `/admin/reports` — KPI charts + filed reports
- [ ] Admin sidebar layout component

---

## Phase 8 — Polish

### Backend tasks
- [ ] `POST /api/uploads/avatar` — Supabase Storage
- [ ] `POST /api/uploads/group-logo`
- [ ] `POST /api/uploads/event-banner`
- [ ] Input sanitization audit
- [ ] Consistent error response audit

### Blazor tasks
- [ ] Loading skeleton components for all list pages
- [ ] 404 and error page refinements
- [ ] Form validation error display in Spanish
- [ ] Accessibility pass (aria labels, keyboard nav)
- [ ] Responsive layout audit (mobile)
- [ ] Avatar upload on profile page
- [ ] Group logo + banner upload

---

## Phase 9 — Stabilization

- [ ] Run full TC-* test case suite (from `09-test-cases.md`)
- [ ] Deploy backend to Azure App Service
- [ ] Deploy Blazor to Azure App Service
- [ ] Configure production Entra redirect URIs
- [ ] Fill KPI report (`10-kpi-report.md`)
- [ ] Record demo video
- [ ] `git tag v1.0.0` on both repos

---

## Definition of Done (per phase)

1. All backend endpoints respond correctly
2. All Blazor pages are functional and match design
3. Relevant test cases pass
4. Build compiles clean (0 errors, 0 warnings)
5. Changes committed with conventional commit messages
6. Both repos pushed to GitHub
