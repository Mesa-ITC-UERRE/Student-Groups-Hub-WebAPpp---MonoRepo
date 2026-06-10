# 03 — Data Model

## 1. Entity-Relationship Overview

```
┌──────────┐       ┌─────────────────────────┐       ┌──────────┐
│   User   │──────►│   GroupRegistration      │       │  Group   │
│          │       │       Request            │       │          │
│  entra   │       └─────────────────────────┘       │ status:  │
│  _oid    │                                          │ pending  │
│  role    │◄──────────────────────────────────────── │ active   │
│          │           Membership                     │ inactive │
└──────────┘     (pending/accepted/rejected/removed)  └──────────┘
     │                                                     │
     │                                          ┌──────────▼──────────┐
     │                                          │    RoleAssignment    │
     │                                          │  (leader/moderator)  │
     │                                          └─────────────────────┘
     │
     │           ┌──────────────────────────────────────────────────┐
     │           │                    Event                          │
     └──────────►│   (belongs to Group, created by User)            │
                 │   status: draft/published/canceled/past           │
                 └───────────────────┬──────────────────────────────┘
                                     │
                          ┌──────────▼──────────┐
                          │  EventParticipation  │
                          │  (User ↔ Event)      │
                          │  status: going/      │
                          │  not_going/maybe     │
                          └─────────────────────┘
```

---

## 2. Database Tables

### 2.1 `users`

Stores all platform users. Created on first login via Entra ID.

| Column | Type | Constraints | Description |
|---|---|---|---|
| `id` | UUID | PK, default gen_random_uuid() | Internal user ID |
| `entra_oid` | VARCHAR(128) | UNIQUE, NOT NULL | Entra ID Object ID (from JWT `oid` claim) |
| `email` | VARCHAR(255) | UNIQUE, NOT NULL | User email (`@uerre.mx`) |
| `display_name` | VARCHAR(255) | NULL | Full name from Entra ID |
| `avatar_url` | TEXT | NULL | Supabase Storage URL |
| `role` | VARCHAR(32) | NOT NULL, default 'student' | `student` / `group_leader` / `admin` |
| `status` | VARCHAR(32) | NOT NULL, default 'active' | `active` / `inactive` / `suspended` |
| `created_at` | TIMESTAMPTZ | NOT NULL, default now() | |
| `updated_at` | TIMESTAMPTZ | NOT NULL, default now() | |

**Indexes:** `users_entra_oid_idx` (unique), `users_email_idx`

---

### 2.2 `groups`

Student organizations. Must be approved by admin before becoming active.

| Column | Type | Constraints | Description |
|---|---|---|---|
| `id` | UUID | PK | Internal group ID |
| `slug` | VARCHAR(128) | UNIQUE, NOT NULL | URL-friendly identifier (e.g. `robotica-uerre`) |
| `name` | VARCHAR(200) | NOT NULL | Display name |
| `description` | TEXT | NULL | Full description |
| `category` | VARCHAR(100) | NULL | Area of interest (e.g. `Tecnología`, `Arte`, `Deporte`) |
| `logo_url` | TEXT | NULL | Supabase Storage URL |
| `banner_url` | TEXT | NULL | Supabase Storage URL |
| `contact_email` | VARCHAR(255) | NULL | Public contact email |
| `contact_info` | TEXT | NULL | Additional contact info |
| `status` | VARCHAR(32) | NOT NULL, default 'pending' | `pending` / `active` / `inactive` / `rejected` |
| `created_by_user_id` | UUID | FK → users.id | User who submitted the group |
| `created_at` | TIMESTAMPTZ | NOT NULL, default now() | |
| `updated_at` | TIMESTAMPTZ | NOT NULL, default now() | |

**Indexes:** `groups_slug_idx` (unique), `groups_status_idx`, `groups_category_idx`

---

### 2.3 `group_registration_requests`

Tracks requests to create a new group, pending admin approval.

| Column | Type | Constraints | Description |
|---|---|---|---|
| `id` | UUID | PK | |
| `requested_by_user_id` | UUID | FK → users.id, NOT NULL | Requester |
| `proposed_group_name` | VARCHAR(200) | NOT NULL | |
| `proposed_description` | TEXT | NULL | |
| `contact_email` | VARCHAR(255) | NOT NULL | |
| `status` | VARCHAR(32) | NOT NULL, default 'pending' | `pending` / `approved` / `rejected` |
| `decision_notes` | TEXT | NULL | Admin notes on approval/rejection |
| `reviewed_by_user_id` | UUID | FK → users.id | Admin who reviewed |
| `reviewed_at` | TIMESTAMPTZ | NULL | |
| `created_group_id` | UUID | FK → groups.id | Set when approved (group created) |
| `created_at` | TIMESTAMPTZ | NOT NULL, default now() | |

---

### 2.4 `memberships`

Tracks a user's membership status within a group.

| Column | Type | Constraints | Description |
|---|---|---|---|
| `id` | UUID | PK | |
| `user_id` | UUID | FK → users.id, NOT NULL | |
| `group_id` | UUID | FK → groups.id, NOT NULL | |
| `status` | VARCHAR(32) | NOT NULL, default 'pending' | `pending` / `accepted` / `rejected` / `removed` |
| `notes` | TEXT | NULL | Leader notes on rejection/removal |
| `requested_at` | TIMESTAMPTZ | NOT NULL, default now() | |
| `responded_at` | TIMESTAMPTZ | NULL | When leader acted |

**Constraints:**
- `UNIQUE (user_id, group_id)` — prevents duplicate active requests
- Business rule: if current status is `rejected` or `removed`, allow a new request (enforced at API level, not DB level, to preserve history)

**Indexes:** `memberships_user_id_idx`, `memberships_group_id_idx`, `memberships_status_idx`

---

### 2.5 `role_assignments`

Assigns a leadership or moderation role within a specific group.

| Column | Type | Constraints | Description |
|---|---|---|---|
| `id` | UUID | PK | |
| `group_id` | UUID | FK → groups.id, NOT NULL | |
| `user_id` | UUID | FK → users.id, NOT NULL | |
| `permission_role` | VARCHAR(32) | NOT NULL | `leader` / `moderator` |
| `display_role` | VARCHAR(100) | NULL | Custom label (e.g. "Presidente") |
| `start_date` | DATE | NULL | |
| `end_date` | DATE | NULL | |
| `created_at` | TIMESTAMPTZ | NOT NULL, default now() | |

**Indexes:** `role_assignments_group_user_idx`

---

### 2.6 `events`

Activities organized by a student group.

| Column | Type | Constraints | Description |
|---|---|---|---|
| `id` | UUID | PK | |
| `group_id` | UUID | FK → groups.id, NOT NULL | Organizing group |
| `created_by_user_id` | UUID | FK → users.id, NOT NULL | Leader who created it |
| `title` | VARCHAR(200) | NOT NULL | |
| `description` | TEXT | NULL | |
| `location` | VARCHAR(300) | NULL | Physical or virtual location |
| `banner_url` | TEXT | NULL | Supabase Storage URL |
| `start_at` | TIMESTAMPTZ | NOT NULL | Start date + time |
| `end_at` | TIMESTAMPTZ | NOT NULL | End date + time |
| `timezone` | VARCHAR(64) | NOT NULL, default 'America/Monterrey' | |
| `capacity` | INTEGER | NULL | Max participants (NULL = unlimited) |
| `status` | VARCHAR(32) | NOT NULL, default 'published' | `draft` / `published` / `canceled` / `past` |
| `visibility` | VARCHAR(32) | NOT NULL, default 'public' | `public` / `members_only` |
| `created_at` | TIMESTAMPTZ | NOT NULL, default now() | |
| `updated_at` | TIMESTAMPTZ | NOT NULL, default now() | |

**Indexes:** `events_group_id_idx`, `events_start_at_idx`, `events_status_idx`

---

### 2.7 `event_participations`

Tracks a user's RSVP / registration for an event.

| Column | Type | Constraints | Description |
|---|---|---|---|
| `id` | UUID | PK | |
| `event_id` | UUID | FK → events.id, NOT NULL | |
| `user_id` | UUID | FK → users.id, NOT NULL | |
| `status` | VARCHAR(32) | NOT NULL, default 'going' | `going` / `not_going` / `maybe` |
| `registered_at` | TIMESTAMPTZ | NOT NULL, default now() | |
| `attended` | BOOLEAN | NULL | NULL = not confirmed yet |

**Constraints:** `UNIQUE (event_id, user_id)`

---

### 2.8 `notifications`

In-app notification store.

| Column | Type | Constraints | Description |
|---|---|---|---|
| `id` | UUID | PK | |
| `user_id` | UUID | FK → users.id, NOT NULL | Recipient |
| `kind` | VARCHAR(64) | NOT NULL | `membership_approved` / `membership_rejected` / `new_membership_request` / `event_update` / `event_canceled` / `group_approved` / `group_rejected` |
| `title` | VARCHAR(255) | NOT NULL | Short notification title |
| `body` | TEXT | NULL | Extended message |
| `href` | TEXT | NULL | Link to navigate to on click |
| `read` | BOOLEAN | NOT NULL, default false | |
| `reference_id` | UUID | NULL | ID of the related entity |
| `reference_type` | VARCHAR(64) | NULL | `group` / `event` / `membership` |
| `created_at` | TIMESTAMPTZ | NOT NULL, default now() | |

**Indexes:** `notifications_user_id_idx`, `notifications_read_idx`

---

### 2.9 `reports`

User-submitted reports for inappropriate content or conduct.

| Column | Type | Constraints | Description |
|---|---|---|---|
| `id` | UUID | PK | |
| `reported_by_user_id` | UUID | FK → users.id, NOT NULL | |
| `target_type` | VARCHAR(64) | NOT NULL | `group` / `event` / `user` |
| `target_id` | UUID | NOT NULL | ID of reported entity |
| `reason` | VARCHAR(255) | NOT NULL | |
| `details` | TEXT | NULL | |
| `status` | VARCHAR(32) | NOT NULL, default 'open' | `open` / `resolved` / `dismissed` |
| `reviewed_by_user_id` | UUID | FK → users.id | |
| `reviewed_at` | TIMESTAMPTZ | NULL | |
| `created_at` | TIMESTAMPTZ | NOT NULL, default now() | |

---

## 3. Entity Categories

| Category | Table |
|---|---|
| **Group request** | `group_registration_requests` |
| **Category tags** | Stored as `VARCHAR` in `groups.category` (free-text + enum validation at API level) |
| **File storage** | Supabase Storage (URLs stored in `_url` columns) |
| **Soft delete** | `users.status`, `groups.status` — never hard-delete |

---

## 4. Predefined Category Values

```
Tecnología e Innovación
Arte y Cultura
Deporte y Bienestar
Ciencias y Académico
Emprendimiento
Voluntariado y Servicio Social
Música
Debate y Oratoria
Medio Ambiente
Internacional y Idiomas
Otro
```

---

## 5. User Roles (stored in `users.role`)

| Value | Description |
|---|---|
| `student` | Default role. Browse groups, join, RSVP to events. |
| `group_leader` | Assigned by admin. Manages groups they are a leader of. |
| `admin` | Full platform access. Approves groups, manages users. |

> Note: A user can be a `group_leader` at the platform level but only manages groups where they have a `role_assignment` with `permission_role = 'leader'`.

---

## 6. Status Transition Diagrams

### Group status
```
[pending] ──approve──► [active] ──deactivate──► [inactive]
          ──reject───► [rejected]
```

### Membership status
```
[pending] ──accept──► [accepted] ──remove──► [removed]
          ──reject──► [rejected]
```

### Event status
```
[draft] ──publish──► [published] ──cancel──► [canceled]
                                ──time passes──► [past]
```

### Group Registration Request status
```
[pending] ──approve──► [approved]  (group is created)
          ──reject───► [rejected]
```
