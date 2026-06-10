# 04 — API Contract

Base URL: `https://api.studentgroupshub.uerre.mx` (production) / `http://localhost:8080` (development)

All protected endpoints require: `Authorization: Bearer <entra_id_access_token>`

All responses use `Content-Type: application/json`.

---

## Error Format

```json
{
  "status": 400,
  "message": "El nombre del grupo ya existe.",
  "timestamp": "2026-06-10T14:30:00Z",
  "errors": {
    "name": ["El nombre es requerido."]
  }
}
```

---

## Pagination (list endpoints)

Query params: `?page=1&pageSize=20`

Response envelope:
```json
{
  "data": [...],
  "page": 1,
  "pageSize": 20,
  "total": 142,
  "totalPages": 8
}
```

---

## 1. Authentication & Users

### `GET /api/users/me`
Returns the authenticated user's profile. Creates user on first call (upsert by Entra OID).

**Auth:** Required

**Response 200:**
```json
{
  "id": "uuid",
  "entraOid": "string",
  "email": "alumno@uerre.mx",
  "displayName": "María López",
  "avatarUrl": "https://...",
  "role": "student",
  "status": "active",
  "createdAt": "2026-01-15T10:00:00Z"
}
```

---

### `PATCH /api/users/me`
Update current user's display name or avatar.

**Auth:** Required

**Body:**
```json
{
  "displayName": "string (optional)",
  "avatarUrl": "string (optional)"
}
```

---

### `GET /api/users` *(admin only)*
List all users with optional filters.

**Auth:** Admin

**Query:** `?role=student&status=active&search=maria&page=1&pageSize=20`

---

### `PATCH /api/users/{id}/role` *(admin only)*
Assign or change a user's platform role.

**Auth:** Admin

**Body:**
```json
{ "role": "group_leader" }
```

---

### `PATCH /api/users/{id}/status` *(admin only)*
Activate or deactivate a user account.

**Auth:** Admin

**Body:**
```json
{ "status": "inactive" }
```

---

## 2. Groups

### `GET /api/groups`
List active groups. Supports search and filtering.

**Auth:** Public

**Query:** `?search=robotica&category=Tecnología&page=1&pageSize=20`

**Response 200:**
```json
{
  "data": [
    {
      "id": "uuid",
      "slug": "robotica-uerre",
      "name": "Robótica U-ERRE",
      "description": "...",
      "category": "Tecnología e Innovación",
      "logoUrl": "https://...",
      "bannerUrl": "https://...",
      "memberCount": 48,
      "status": "active",
      "createdAt": "2025-09-01T00:00:00Z"
    }
  ],
  "page": 1,
  "total": 23
}
```

---

### `GET /api/groups/{slug}`
Get full group details by slug.

**Auth:** Public

**Response:** Group object with `leaderInfo`, `recentEvents`, `memberCount`

---

### `GET /api/groups/{id}/members`
List accepted members of a group.

**Auth:** Public

**Response:**
```json
[
  {
    "membershipId": "uuid",
    "userId": "uuid",
    "displayName": "Juan Torres",
    "email": "j.torres@uerre.mx",
    "avatarUrl": "https://...",
    "status": "accepted",
    "joinedAt": "2026-02-10T00:00:00Z"
  }
]
```

---

### `GET /api/groups/{id}/memberships/pending` *(group leader / admin)*
List pending membership requests.

**Auth:** Group Leader of this group, or Admin

---

### `POST /api/groups/{id}/join`
Request to join a group.

**Auth:** Required (Student)

**Response 201:**
```json
{
  "membershipId": "uuid",
  "groupId": "uuid",
  "userId": "uuid",
  "status": "pending",
  "requestedAt": "2026-06-10T12:00:00Z"
}
```

**Error 409:** If duplicate active request exists.

---

### `PATCH /api/groups/{id}/memberships/{membershipId}/approve` *(group leader / admin)*
Approve a pending membership request.

**Auth:** Group Leader of this group, or Admin

**Body (optional):**
```json
{ "notes": "Bienvenido al grupo." }
```

---

### `PATCH /api/groups/{id}/memberships/{membershipId}/reject` *(group leader / admin)*
Reject a pending membership request.

**Auth:** Group Leader of this group, or Admin

**Body (optional):**
```json
{ "notes": "Por ahora no hay cupo." }
```

---

### `DELETE /api/groups/{id}/members/{userId}` *(group leader / admin)*
Remove a member from the group.

**Auth:** Group Leader of this group, or Admin

---

### `GET /api/groups/{id}/roles`
List role assignments for a group.

**Auth:** Group Leader of this group, or Admin

---

### `POST /api/groups/{id}/roles` *(admin only)*
Assign a leadership role within a group.

**Auth:** Admin

**Body:**
```json
{
  "userId": "uuid",
  "permissionRole": "leader",
  "displayRole": "Presidente",
  "startDate": "2026-01-01",
  "endDate": null
}
```

---

### `DELETE /api/groups/{id}/roles/{roleId}` *(admin only)*
Remove a role assignment.

**Auth:** Admin

---

## 3. Group Registration Requests

### `POST /api/group-registration-requests`
Submit a new group creation request.

**Auth:** Required

**Body:**
```json
{
  "proposedGroupName": "Club de Fotografía",
  "proposedDescription": "Grupo para aprender fotografía...",
  "contactEmail": "foto@uerre.mx"
}
```

**Response 201:** `GroupRegistrationRequest` object

---

### `GET /api/group-registration-requests/mine`
Get the authenticated user's own requests.

**Auth:** Required

---

### `GET /api/group-registration-requests` *(admin only)*
List all pending group requests.

**Auth:** Admin

**Query:** `?status=pending&page=1`

---

### `PATCH /api/group-registration-requests/{id}/approve` *(admin only)*
Approve a group request. Creates the group automatically.

**Auth:** Admin

**Body (optional):**
```json
{ "notes": "Aprobado. El grupo ha sido creado." }
```

---

### `PATCH /api/group-registration-requests/{id}/reject` *(admin only)*
Reject a group request.

**Auth:** Admin

**Body (optional):**
```json
{ "notes": "Ya existe un grupo similar." }
```

---

## 4. Events

### `GET /api/groups/{id}/events`
List events for a group.

**Auth:** Public

**Query:** `?status=published&from=2026-06-01&to=2026-07-01`

---

### `GET /api/events`
List all upcoming published events (cross-group).

**Auth:** Public

**Query:** `?search=hackathon&groupId=uuid&from=2026-06-01&page=1`

---

### `GET /api/events/{id}`
Get event details.

**Auth:** Public

---

### `POST /api/groups/{id}/events` *(group leader / admin)*
Create a new event.

**Auth:** Group Leader of this group, or Admin

**Body:**
```json
{
  "title": "Hackathon Spring 2026",
  "description": "48 horas de innovación...",
  "location": "Lab de Innovación, Edificio A",
  "startAt": "2026-09-15T09:00:00-06:00",
  "endAt": "2026-09-17T09:00:00-06:00",
  "capacity": 60,
  "status": "published",
  "visibility": "public"
}
```

**Response 201:** `Event` object

---

### `PUT /api/groups/{id}/events/{eventId}` *(group leader / admin)*
Update an event.

**Auth:** Group Leader of this group, or Admin

---

### `DELETE /api/groups/{id}/events/{eventId}` *(group leader / admin)*
Cancel or delete an event (sets status to `canceled`).

**Auth:** Group Leader of this group, or Admin

---

### `GET /api/events/{id}/rsvp/all` *(group leader / admin)*
List all RSVPs for an event.

**Auth:** Group Leader of this group, or Admin

---

### `POST /api/events/{id}/rsvp`
Register for an event (or update registration status).

**Auth:** Required

**Body:**
```json
{ "status": "going" }
```

---

### `DELETE /api/events/{id}/rsvp`
Cancel registration for an event.

**Auth:** Required

---

## 5. Notifications

### `GET /api/users/me/notifications`
Get the current user's notifications.

**Auth:** Required

**Query:** `?read=false&page=1&pageSize=20`

---

### `PATCH /api/notifications/{id}/read`
Mark a single notification as read.

**Auth:** Required

---

### `PATCH /api/users/me/notifications/read-all`
Mark all notifications as read.

**Auth:** Required

---

## 6. File Uploads

### `POST /api/uploads/avatar`
Upload a profile picture. Returns the Supabase Storage URL.

**Auth:** Required

**Content-Type:** `multipart/form-data`

**Body:** `file` (image/jpeg, image/png, image/webp — max 2MB)

**Response 200:**
```json
{ "url": "https://...supabase.co/storage/v1/object/public/avatars/user-uuid.jpg" }
```

---

### `POST /api/uploads/group-logo`
Upload a group logo.

**Auth:** Group Leader or Admin

---

### `POST /api/uploads/event-banner`
Upload an event banner image.

**Auth:** Group Leader or Admin

---

## 7. Reports

### `POST /api/reports`
File a report against a user, group, or event.

**Auth:** Required

**Body:**
```json
{
  "targetType": "group",
  "targetId": "uuid",
  "reason": "Contenido inapropiado",
  "details": "El grupo contiene..."
}
```

---

### `GET /api/reports` *(admin only)*
List open reports.

**Auth:** Admin

---

### `PATCH /api/reports/{id}/resolve` *(admin only)*
Resolve or dismiss a report.

**Auth:** Admin

**Body:**
```json
{ "status": "resolved" }
```

---

## 8. Dashboard & Reports

### `GET /api/dashboard/student`
Returns data for the student dashboard.

**Auth:** Required (Student)

**Response:**
```json
{
  "joinedGroups": [...],
  "pendingRequests": [...],
  "upcomingEvents": [...]
}
```

---

### `GET /api/dashboard/leader`
Returns data for the group leader dashboard.

**Auth:** Group Leader

**Response:**
```json
{
  "managedGroups": [...],
  "pendingMembershipRequests": [...],
  "upcomingEvents": [...],
  "memberCounts": {}
}
```

---

### `GET /api/dashboard/admin`
Returns platform-wide summary stats.

**Auth:** Admin

**Response:**
```json
{
  "totalUsers": 3400,
  "totalGroups": 120,
  "activeGroups": 98,
  "pendingGroupRequests": 4,
  "totalEvents": 280,
  "recentActivity": [...]
}
```

---

### `GET /api/reports/metrics` *(admin only)*
Platform KPI metrics for the reports module.

**Auth:** Admin

**Response:**
```json
{
  "userRegistrations": { "total": 3400, "lastMonth": 120 },
  "groupsCreated": { "total": 120, "lastMonth": 8 },
  "membershipRequests": { "total": 890, "approved": 720, "rejected": 170 },
  "eventsCreated": { "total": 280, "lastMonth": 24 },
  "eventParticipations": { "total": 4200, "lastMonth": 380 }
}
```
