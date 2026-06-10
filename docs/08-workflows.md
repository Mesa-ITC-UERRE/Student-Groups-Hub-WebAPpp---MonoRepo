# 08 — Main Workflows

Detailed step-by-step interaction flows for the primary user journeys.

---

## WF-01: Student Registers and Logs In

**Actor:** Student (new user)
**Precondition:** Has a valid `@uerre.mx` Microsoft account

1. Student visits the platform landing page (`/`)
2. Student clicks "Iniciar Sesión" or "Crear Cuenta" CTA
3. System redirects to `/login`
4. Student clicks "Continuar con Microsoft"
5. MSAL.js initiates `loginRedirect()` to Entra ID
6. Student authenticates with institutional credentials
7. Entra ID redirects back to `/auth/callback` with auth code
8. MSAL.js exchanges code for tokens
9. Frontend calls `GET /api/users/me` with Bearer token
10. Backend (first login): creates user record in DB with `role: student`
11. Backend returns user profile
12. Frontend stores user in AuthContext
13. System redirects student to `/dashboard`

**Success:** Student is authenticated, user record created, dashboard shown.
**Error handling:** If not a `@uerre.mx` account → Entra ID blocks login. If API fails → show error message on callback page.

---

## WF-02: Student Browses and Joins a Group

**Actor:** Authenticated Student
**Precondition:** Student is logged in

1. Student navigates to `/groups`
2. System loads list of active groups from `GET /api/groups`
3. Student searches by name or filters by category
4. System updates results in real-time (debounced API call)
5. Student clicks on a group card
6. System loads group detail page `/groups/[slug]`
7. Student reviews group info, members, and events
8. Student clicks "Unirse al grupo" (JoinButton)
9. Frontend calls `POST /api/groups/{id}/join`
10. Backend validates: user is authenticated, no duplicate active request
11. Backend creates membership record with `status: pending`
12. Backend sends notification to group leader(s): `new_membership_request`
13. JoinButton changes to "Solicitud pendiente" state
14. Dashboard shows the pending request under "Mis solicitudes"

**Success:** Membership request created, leader notified.
**Error (409):** Duplicate request → show message "Ya tienes una solicitud pendiente para este grupo."
**Error (not authenticated):** Show prompt to log in.

---

## WF-03: Group Leader Reviews and Approves a Membership Request

**Actor:** Group Leader
**Precondition:** Leader has a `role_assignment` for the group; student has a pending request

1. Leader receives in-app notification: "Nueva solicitud de membresía en [Grupo]"
2. Leader navigates to leader dashboard or group management page
3. System displays pending requests list for the group
4. Leader reviews student's profile (name, email)
5. Leader clicks "Aprobar" button
6. Frontend calls `PATCH /api/groups/{id}/memberships/{membershipId}/approve`
7. Backend validates: leader is authorized for this group
8. Backend updates membership `status` to `accepted`, sets `responded_at`
9. Backend creates notification for student: `membership_approved`
10. Backend sends email to student via Resend: "¡Tu solicitud fue aceptada!"
11. Membership request disappears from pending list
12. Member count increases on group detail page

**Alternative (Reject):**
- Steps 5-6 use "Rechazar" button and `reject` endpoint
- Status becomes `rejected`
- Student notified: "Tu solicitud fue rechazada"
- Optional: leader can add rejection notes

---

## WF-04: Group Leader Creates an Event

**Actor:** Group Leader
**Precondition:** Leader has an active group with `status: active`

1. Leader navigates to their group management page
2. Leader clicks "Crear Evento" button
3. System shows event creation form
4. Leader fills in:
   - Title (required)
   - Description (optional)
   - Location (optional)
   - Start date + time (required)
   - End date + time (required)
   - Capacity limit (optional)
   - Visibility: Público / Solo miembros
   - Status: Borrador / Publicado
   - Banner image (optional — upload to Supabase Storage)
5. Leader clicks "Guardar evento"
6. Frontend validates form (required fields, date logic: end > start)
7. Frontend calls `POST /api/groups/{id}/events`
8. Backend validates, creates event record
9. If `status: published`:
   - Event appears on `/groups/[slug]` and `/events`
   - Members of the group receive notification: `event_update`
10. System shows success message and redirects to event detail

**Alternative (save as draft):**
- `status: draft` — event is saved but not visible to students
- Leader can publish later by editing and changing status

---

## WF-05: Student RSVPs to an Event

**Actor:** Authenticated Student
**Precondition:** Event exists with `status: published`; capacity not yet reached

1. Student sees event on `/events` or group detail page
2. Student clicks on the event card
3. System shows event detail page
4. Student reviews event details (date, location, capacity, current RSVPs)
5. Student clicks "Asistiré" (RSVP button)
6. Frontend calls `POST /api/events/{id}/rsvp` with `{ status: "going" }`
7. Backend validates: event is not canceled; capacity not exceeded
8. Backend upserts `event_participations` record
9. RSVP button changes to "Asistiré ✓" (confirmed state)
10. Available spots count decreases

**Alternative (cancel RSVP):**
- Student clicks "Cancelar asistencia"
- Frontend calls `DELETE /api/events/{id}/rsvp`
- Spot becomes available again

---

## WF-06: Student Submits a Group Creation Request

**Actor:** Authenticated Student (or any role)
**Precondition:** User is authenticated

1. User navigates to `/groups/register`
2. System shows group creation request form
3. User fills in:
   - Proposed group name (required)
   - Description (optional)
   - Contact email (required)
4. User clicks "Enviar solicitud"
5. Frontend validates form
6. Frontend calls `POST /api/group-registration-requests`
7. Backend creates request with `status: pending`
8. System shows success confirmation card
9. User's dashboard shows the pending request under "Mis solicitudes de grupo"
10. Admin receives in-app notification of pending request

---

## WF-07: Administrator Approves a Group Request

**Actor:** Administrator
**Precondition:** A group creation request exists with `status: pending`

1. Admin logs in and accesses the Admin Dashboard (`/admin`)
2. Dashboard shows count of pending group requests in the KPI card
3. Admin navigates to Solicitudes de Grupos section
4. Admin reviews the request (proposed name, description, contact email, requester)
5. Admin clicks "Aprobar"
6. Frontend calls `PATCH /api/group-registration-requests/{id}/approve`
7. Backend:
   a. Creates new group record from request data (`status: active`)
   b. Auto-generates `slug` from name
   c. Updates request `status` to `approved`
   d. Creates `role_assignment` for the requester as `leader`
   e. Sends notification to requester: `group_approved`
   f. Sends email via Resend: "¡Tu grupo ha sido aprobado!"
8. Group is now visible on `/groups`
9. Requester's role is upgraded to `group_leader` (if not already)

**Alternative (Reject):**
- Admin can add rejection notes
- Request `status` → `rejected`
- Requester notified via in-app + email

---

## WF-08: Administrator Reviews Platform Activity

**Actor:** Administrator
**Precondition:** Admin is logged in

1. Admin navigates to Admin Dashboard (`/admin`)
2. System loads dashboard with:
   - Total users / groups / active groups counters
   - Pending requests count
   - Recent activity feed
3. Admin reviews metrics at a glance
4. Admin can drill down to:
   - `/admin/users` — user management table
   - `/admin/groups` — all groups table
   - `/admin/reports` — KPI charts + filed reports
5. If Admin sees a problem (e.g., inappropriate group):
   a. Admin navigates to the group
   b. Clicks "Desactivar grupo"
   c. Frontend calls `PATCH /api/groups/{id}/status` with `{ status: "inactive" }`
   d. Group disappears from public listings
   e. Members receive notification

---

## WF-09: User Manages Their Profile

**Actor:** Any authenticated user
**Precondition:** User is logged in

1. User clicks their avatar in the navbar
2. System shows dropdown with: Mi Perfil, Cerrar Sesión
3. User navigates to `/profile`
4. System displays current profile (avatar, name, email, joined groups)
5. User clicks "Editar perfil"
6. User can update:
   - Display name
   - Avatar (upload image → Supabase Storage)
7. User clicks "Guardar cambios"
8. Frontend calls `PATCH /api/users/me`
9. System updates profile and shows success toast

---

## WF-10: User Receives and Views Notifications

**Actor:** Any authenticated user
**Precondition:** One or more unread notifications exist

1. User sees notification bell in navbar with gold badge showing count
2. User clicks the bell icon
3. System shows notification dropdown (last 10 notifications)
4. Each notification shows: icon, title, body, time ago
5. User clicks a notification
6. Frontend calls `PATCH /api/notifications/{id}/read`
7. System navigates to the relevant page (group, event, or membership)
8. Notification marked as read; badge count decreases
9. User can click "Marcar todo como leído" to clear all
