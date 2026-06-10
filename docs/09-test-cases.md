# 09 — Test Cases

Manual test cases for validating system functionality against acceptance criteria.

**Legend:**
- **TC:** Test Case ID
- **Expected:** What should happen
- **Actual:** Filled in during testing
- **Status:** Pass / Fail / Blocked

---

## Module: Authentication (TC-AUTH)

### TC-AUTH-01 — Login with valid institutional account
- **Steps:** Open the app → click "Iniciar Sesión" → click "Continuar con Microsoft" → enter valid `@uerre.mx` credentials
- **Expected:** User is redirected to `/dashboard`; user record created in DB; role shown as `student`
- **Status:** [ ]

### TC-AUTH-02 — Login with non-institutional account
- **Steps:** Attempt login with a `@gmail.com` or `@hotmail.com` account
- **Expected:** Microsoft blocks authentication; user stays on login page; error message shown
- **Status:** [ ]

### TC-AUTH-03 — Redirect to original page after login
- **Steps:** Visit `/groups/register` while unauthenticated → get redirected to `/login` → log in
- **Expected:** After login, user is redirected to `/groups/register` (not dashboard)
- **Status:** [ ]

### TC-AUTH-04 — Logout
- **Steps:** Log in → click user avatar in navbar → click "Cerrar sesión"
- **Expected:** MSAL cache cleared; user redirected to `/`; protected routes no longer accessible
- **Status:** [ ]

### TC-AUTH-05 — Access protected route while unauthenticated
- **Steps:** Paste `/dashboard` URL directly in browser without logging in
- **Expected:** User redirected to `/login?returnTo=/dashboard`
- **Status:** [ ]

---

## Module: Groups (TC-GRP)

### TC-GRP-01 — Browse groups listing
- **Steps:** Navigate to `/groups` without logging in
- **Expected:** List of active groups visible; search bar present; category filter chips visible
- **Status:** [ ]

### TC-GRP-02 — Search groups by name
- **Steps:** On `/groups`, type "Robot" in the search bar
- **Expected:** Only groups with "Robot" in their name appear; other groups hidden
- **Status:** [ ]

### TC-GRP-03 — Filter groups by category
- **Steps:** On `/groups`, click "Tecnología e Innovación" filter chip
- **Expected:** Only groups with that category shown; counter updates
- **Status:** [ ]

### TC-GRP-04 — View group detail page
- **Steps:** Click on a group card from the listing
- **Expected:** Group detail page shows name, description, category, member count, events, join button
- **Status:** [ ]

### TC-GRP-05 — Submit group creation request (authenticated)
- **Steps:** Log in → navigate to `/groups/register` → fill in name and contact email → submit
- **Expected:** Success message shown; request appears in dashboard under pending requests
- **Status:** [ ]

### TC-GRP-06 — Submit group creation request (unauthenticated)
- **Steps:** Navigate to `/groups/register` without logging in → attempt to submit
- **Expected:** Warning banner shown; form submission redirects to login
- **Status:** [ ]

### TC-GRP-07 — Admin approves a group request
- **Steps:** Submit group request → log in as admin → go to admin panel → approve request
- **Expected:** Group becomes `active`; appears on `/groups`; requester receives notification
- **Status:** [ ]

### TC-GRP-08 — Admin rejects a group request
- **Steps:** Submit group request → log in as admin → reject with notes
- **Expected:** Request status → `rejected`; requester receives rejection notification with notes
- **Status:** [ ]

---

## Module: Membership (TC-MBR)

### TC-MBR-01 — Join a group
- **Steps:** Log in as student → navigate to a group detail page → click "Unirse al grupo"
- **Expected:** Button changes to "Solicitud pendiente"; leader receives notification
- **Status:** [ ]

### TC-MBR-02 — Prevent duplicate membership request
- **Steps:** Send a join request to a group → attempt to join the same group again
- **Expected:** Button already shows "Solicitud pendiente"; API returns 409; toast message shown
- **Status:** [ ]

### TC-MBR-03 — View membership status on dashboard
- **Steps:** Send a join request → navigate to dashboard
- **Expected:** Request appears under "Solicitudes pendientes" with group name and date
- **Status:** [ ]

### TC-MBR-04 — Leader approves membership request
- **Steps:** Student sends join request → log in as group leader → approve from dashboard or group management page
- **Expected:** Membership status → `accepted`; student notified via in-app + email; member count increases
- **Status:** [ ]

### TC-MBR-05 — Leader rejects membership request
- **Steps:** Student sends join request → leader rejects with optional notes
- **Expected:** Membership status → `rejected`; student notified with rejection message
- **Status:** [ ]

### TC-MBR-06 — Leader removes a member
- **Steps:** Log in as leader → go to group member list → click "Eliminar" on a member
- **Expected:** Membership status → `removed`; member no longer appears in the list
- **Status:** [ ]

### TC-MBR-07 — Student cannot approve memberships
- **Steps:** As a student, attempt to call `PATCH /api/groups/{id}/memberships/{mid}/approve` directly
- **Expected:** API returns 403 Forbidden
- **Status:** [ ]

---

## Module: Events (TC-EVT)

### TC-EVT-01 — View upcoming events
- **Steps:** Navigate to `/events` or group detail page events tab
- **Expected:** Events with `status: published` and `start_at` in the future are shown
- **Status:** [ ]

### TC-EVT-02 — Leader creates an event
- **Steps:** Log in as group leader → create event with title, date, location, capacity
- **Expected:** Event appears on the group page and `/events`; members receive notification
- **Status:** [ ]

### TC-EVT-03 — Create event with end date before start date
- **Steps:** Fill in event form with `end_at` before `start_at` → submit
- **Expected:** Form validation error shown; event not created
- **Status:** [ ]

### TC-EVT-04 — Leader edits an event
- **Steps:** Create event → edit the location
- **Expected:** Changes saved; members receive `event_update` notification
- **Status:** [ ]

### TC-EVT-05 — Leader cancels an event
- **Steps:** Create published event → cancel it
- **Expected:** Event status → `canceled`; no longer shown as upcoming; RSVPs receive notification
- **Status:** [ ]

### TC-EVT-06 — Student cannot create events
- **Steps:** As a student, attempt to POST to `/api/groups/{id}/events`
- **Expected:** API returns 403 Forbidden
- **Status:** [ ]

---

## Module: Participation / RSVP (TC-RSVP)

### TC-RSVP-01 — Student RSVPs to an event
- **Steps:** Log in as student → navigate to event detail → click "Asistiré"
- **Expected:** RSVP registered; available spots decrease by 1; button shows confirmed state
- **Status:** [ ]

### TC-RSVP-02 — Cancel RSVP
- **Steps:** After RSVPing → click "Cancelar asistencia"
- **Expected:** RSVP removed; available spots increase by 1
- **Status:** [ ]

### TC-RSVP-03 — RSVP to full event
- **Steps:** Fill event to capacity → attempt to RSVP as a new student
- **Expected:** API returns error; "No hay lugares disponibles" message shown
- **Status:** [ ]

### TC-RSVP-04 — RSVP to canceled event
- **Steps:** Cancel an event → student attempts to RSVP
- **Expected:** API returns error; RSVP button disabled or hidden
- **Status:** [ ]

### TC-RSVP-05 — Leader views participant list
- **Steps:** Log in as leader → navigate to event → open participant list
- **Expected:** List shows all RSVPs with student names, emails, and RSVP status
- **Status:** [ ]

---

## Module: Dashboard (TC-DASH)

### TC-DASH-01 — Student dashboard
- **Steps:** Log in as student → navigate to `/dashboard`
- **Expected:** Shows joined groups, pending requests, upcoming events from joined groups
- **Status:** [ ]

### TC-DASH-02 — Leader dashboard
- **Steps:** Log in as group leader → navigate to `/dashboard`
- **Expected:** Shows managed groups, pending membership requests, upcoming events
- **Status:** [ ]

### TC-DASH-03 — Admin dashboard
- **Steps:** Log in as admin → navigate to `/admin`
- **Expected:** Shows total users, total groups, active groups, pending approvals, recent activity
- **Status:** [ ]

### TC-DASH-04 — Student cannot access admin dashboard
- **Steps:** Log in as student → navigate to `/admin`
- **Expected:** 403 page shown or redirect to student dashboard
- **Status:** [ ]

---

## Module: Notifications (TC-NOTIF)

### TC-NOTIF-01 — Notification badge shows unread count
- **Steps:** Trigger a notification (e.g., get membership approved) → check navbar
- **Expected:** Gold badge with count appears on bell icon
- **Status:** [ ]

### TC-NOTIF-02 — Mark notification as read
- **Steps:** Click a notification in the dropdown
- **Expected:** Notification marked as read; badge count decreases
- **Status:** [ ]

### TC-NOTIF-03 — Mark all notifications as read
- **Steps:** Have 3+ unread notifications → click "Marcar todo como leído"
- **Expected:** All notifications marked read; badge disappears
- **Status:** [ ]

### TC-NOTIF-04 — Email notification on membership approval
- **Steps:** Leader approves a membership request
- **Expected:** Student receives email (via Resend) with approval message
- **Status:** [ ]

---

## Module: User Management (TC-USR)

### TC-USR-01 — Admin views user list
- **Steps:** Log in as admin → navigate to `/admin/users`
- **Expected:** Table showing all users with name, email, role, status, join date
- **Status:** [ ]

### TC-USR-02 — Admin promotes user to group leader
- **Steps:** Admin finds a user → changes role to `group_leader`
- **Expected:** User's role updated; user can now access leader features
- **Status:** [ ]

### TC-USR-03 — Admin deactivates a user
- **Steps:** Admin sets user status to `inactive`
- **Expected:** User cannot log in; their groups/events still visible but unmanageable
- **Status:** [ ]

### TC-USR-04 — User updates own profile
- **Steps:** Log in → go to `/profile` → change display name → save
- **Expected:** Name updated; navbar shows new name; success toast shown
- **Status:** [ ]

### TC-USR-05 — User uploads profile picture
- **Steps:** Go to `/profile` → click avatar → upload image (< 2MB PNG)
- **Expected:** Image uploaded to Supabase Storage; avatar URL updated; new avatar shown
- **Status:** [ ]

---

## Module: File Uploads (TC-FILE)

### TC-FILE-01 — Upload group logo
- **Steps:** Leader edits group → uploads logo (PNG, 1MB)
- **Expected:** Logo appears on group card and group detail page
- **Status:** [ ]

### TC-FILE-02 — Upload oversized file
- **Steps:** Attempt to upload a 5MB image as avatar
- **Expected:** Frontend or backend validation rejects; error message "El archivo no puede superar 2MB"
- **Status:** [ ]

### TC-FILE-03 — Upload non-image file
- **Steps:** Attempt to upload a PDF as a group logo
- **Expected:** Validation rejects; error message about file type
- **Status:** [ ]

---

## Security (TC-SEC)

### TC-SEC-01 — JWT tampered token rejected
- **Steps:** Manually modify the Bearer token in a request
- **Expected:** Backend returns 401 Unauthorized
- **Status:** [ ]

### TC-SEC-02 — Cross-user group management blocked
- **Steps:** Log in as leader of Group A → attempt to approve membership for Group B via API
- **Expected:** 403 Forbidden
- **Status:** [ ]

### TC-SEC-03 — SQL injection in search
- **Steps:** Enter `'; DROP TABLE users; --` in the group search bar
- **Expected:** Query safely parameterized via EF Core; no error or data loss
- **Status:** [ ]

### TC-SEC-04 — XSS in user input
- **Steps:** Enter `<script>alert('XSS')</script>` as a group description
- **Expected:** Text rendered as plain text, not executed
- **Status:** [ ]
