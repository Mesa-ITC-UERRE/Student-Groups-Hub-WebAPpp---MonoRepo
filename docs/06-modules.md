# 06 — System Modules

## Module Map

```
┌─────────────────────────────────────────────────────────┐
│                   Student Groups Hub                     │
├──────────┬──────────┬──────────┬──────────┬─────────────┤
│  Auth    │  Users   │  Groups  │Membership│   Events    │
│ Module   │ Module   │ Module   │ Module   │  Module     │
├──────────┴──────────┴──────────┴──────────┴─────────────┤
│           Participation │  Dashboard  │  Notifications  │
│              Module     │   Module    │     Module      │
├─────────────────────────┴─────────────┴─────────────────┤
│                    Reports Module                        │
└─────────────────────────────────────────────────────────┘
```

---

## 9.1 Authentication Module

**Purpose:** Handles user identity and session management via Microsoft Entra ID.

### Responsibilities
- Initiate Microsoft OAuth 2.0 / OIDC login redirect
- Handle the `/auth/callback` redirect and exchange auth code for tokens
- Store MSAL tokens (sessionStorage/localStorage via MSAL.js cache)
- Inject Bearer token into all authenticated API requests
- Detect token expiry and trigger silent renewal or re-login
- Handle logout (clear MSAL cache, redirect to home)
- Backend JWT validation via `Microsoft.Identity.Web`
- User upsert on first login (create user record from Entra claims)

### Key Pages / Components
- `/login` — Login page with "Continuar con Microsoft" button
- `/auth/callback` — OAuth redirect landing page
- `MsalProviderWrapper` — wraps the entire app with MSAL context
- `useAuth()` hook — exposes `user`, `isAuthenticated`, `login()`, `logout()`

### Business Rules
- Only `@uerre.mx` accounts can authenticate (enforced by Entra ID tenant restriction)
- After successful login, user is created with `role: student` if not found
- Unauthenticated users can browse groups/events but cannot join or RSVP

---

## 9.2 User Management Module

**Purpose:** Manages user profiles, roles, and account status.

### Responsibilities
- Display and edit user profile (display name, avatar)
- Upload profile pictures to Supabase Storage
- Admin: list all users with search and role/status filters
- Admin: assign or change platform roles (`student` → `group_leader` → `admin`)
- Admin: activate or deactivate user accounts

### Key Pages / Components
- `/profile` — User profile page (view + edit)
- `/admin/users` — User management table (admin only)
- `AvatarUpload` component

### Business Rules
- Users can edit their own display name and avatar
- Only admins can change roles
- A deactivated user cannot log in or perform any actions
- Admins cannot deactivate themselves

---

## 9.3 Group Management Module

**Purpose:** Creation, visualization, approval, and administration of student groups.

### Responsibilities
- Display a searchable, filterable list of active groups
- Display individual group detail pages (info, members, events)
- Accept group creation requests (form submission)
- Admin: review, approve, and reject group requests
- Admin/Leader: edit group information
- Admin: activate or deactivate groups
- Manage group metadata (name, description, category, logo, banner, contact)

### Key Pages / Components
- `/groups` — Groups listing with search/filter
- `/groups/[slug]` — Group detail page
- `/groups/register` — Group creation request form
- `/admin/groups` — Admin group management table
- `GroupCard` component — displayed in listings
- `GroupDetail` component — full group page layout

### Business Rules
- New groups start with status `pending` and require admin approval
- Only admins can approve, reject, or deactivate groups
- Only group leaders (with role assignment) or admins can edit group info
- A group must have at least one leader at all times
- Slug is auto-generated from group name and must be unique

---

## 9.4 Membership Module

**Purpose:** Manages the relationship between users and groups.

### Responsibilities
- Allow students to request to join a group
- Prevent duplicate active membership requests
- Allow group leaders to review, approve, and reject pending requests
- Allow group leaders to remove existing members
- Display membership status on group detail page
- Allow users to view all groups they belong to (on dashboard)
- Maintain full membership history

### Key Pages / Components
- `JoinButton` component — shown on group detail page
- `/dashboard` — shows joined groups and pending requests (student view)
- `/leader/groups/[id]/members` — member management page (leader view)
- Membership requests section in leader dashboard

### Status Transitions
```
pending → accepted → removed
pending → rejected
```

### Business Rules
- A user with status `pending` or `accepted` cannot submit another request to the same group
- A user with `rejected` or `removed` status may reapply
- Only the leader of a specific group (or an admin) can approve/reject requests for that group
- Notifications are sent to both parties on status change

---

## 9.5 Event Management Module

**Purpose:** Creation, publication, and management of group events.

### Responsibilities
- Allow group leaders to create, edit, and cancel events
- Display upcoming events on the public events page and group pages
- Display event details (title, description, date, location, capacity, status)
- Support event visibility (`public` vs `members_only`)
- Upload event banner images to Supabase Storage

### Key Pages / Components
- `/events` — All upcoming events (cross-group listing)
- `/events/[id]` — Event detail page
- `/leader/groups/[id]/events` — Event management (leader view)
- `/leader/groups/[id]/events/new` — Create event form
- `EventCard` component — for listings

### Business Rules
- Only leaders of a group (or admins) can create/edit/cancel events for that group
- A canceled event cannot accept new RSVPs
- Events with `draft` status are not visible to students
- A `past` status is auto-assigned by the system when `end_at` has passed

---

## 9.6 Participation Module

**Purpose:** Tracks user RSVP and attendance for events.

### Responsibilities
- Allow students to RSVP to events (going / not going / maybe)
- Enforce capacity limits when registering
- Allow leaders to view the participant list for their events
- Track attendance confirmation (attended: boolean, set post-event)
- Block RSVPs on canceled events

### Key Pages / Components
- `RsvpButton` component — shown on event detail pages
- `/leader/groups/[id]/events/[eventId]/participants` — participant list

### Business Rules
- A user can only have one RSVP per event (upsert on change)
- If event has a capacity limit and it's reached, new `going` RSVPs are blocked
- Canceled events reject new RSVPs
- Attendance is separately confirmed by the leader after the event

---

## 9.7 Dashboard Module

**Purpose:** Role-specific summary views with quick access to key actions.

### Student Dashboard
- Joined groups (with direct links)
- Pending membership requests (with status indicators)
- Upcoming events from joined groups
- Quick actions: Browse Groups, Create Group Request

### Group Leader Dashboard
- Managed groups (stats: members, pending requests)
- Pending membership requests (with approve/reject inline actions)
- Upcoming events for managed groups
- Quick actions: Create Event, Manage Members

### Admin Dashboard
- Platform stats: total users, total groups, active groups, events this month
- Pending group registration requests (with quick approve/reject)
- Open reports
- Recent activity feed (last 10 actions: new users, approved groups, events created)
- KPI summary cards

### Key Pages / Components
- `/dashboard` — Routes to correct dashboard based on user role
- `StudentDashboard`, `LeaderDashboard`, `AdminDashboard` sub-components
- `StatCard` component — for metric display
- `ActivityFeed` component — for admin recent activity

---

## 9.8 Reports Module

**Purpose:** Provides metrics, statistics, and activity summaries for administrators.

### Responsibilities
- Display platform-wide KPI metrics
- Show member count per group
- Show event count per group
- Show participation statistics (RSVPs by status)
- Allow comparison of before/after implementation (baseline vs current)
- Display open reports filed by users

### Key Pages / Components
- `/admin/reports` — Reports and metrics page
- `MetricsChart` components (bar charts, counters)
- `OpenReports` table — list of filed reports

### KPIs Tracked
| KPI | Description |
|---|---|
| Registered users | Total users by role |
| Active groups | Groups with `status = active` |
| Membership requests | Total, approved, rejected, pending |
| Events created | Total events by status |
| Event participations | Total RSVPs by status |
| Group visibility increase | Groups added since go-live |
| Time to process requests | Avg. time from request to response |

---

## Cross-Cutting Concerns

### Notifications (integrated across modules)

Every significant action triggers an in-app notification (stored in `notifications` table) and optionally an email via Resend.

Notification triggers:
- New membership request → group leader
- Membership approved/rejected → student
- Group request approved/rejected → user who submitted
- Event updated/canceled → group members

### File Upload (integrated across modules)

All image uploads are handled through:
1. Frontend sends `multipart/form-data` to `/api/uploads/{type}`
2. Backend validates (type, size: max 2MB), uploads to Supabase Storage
3. Backend returns public URL
4. Frontend stores URL in form state, submits with main record

### Search & Filtering (Groups + Events)

Implemented server-side with EF Core `ILIKE` queries on PostgreSQL:
- Groups: by `name`, `category`, `status`
- Events: by `title`, `groupId`, `startAt` date range, `status`
