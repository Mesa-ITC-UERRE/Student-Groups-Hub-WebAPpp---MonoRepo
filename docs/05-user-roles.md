# 05 — User Roles & Permissions

## 1. Role Overview

| Role | Spanish label | Description |
|---|---|---|
| `student` | Estudiante | Default role for all authenticated users. Browse, join, RSVP. |
| `group_leader` | Líder de Grupo | Manages one or more groups assigned by admin. |
| `admin` | Administrador | Full platform access. Approves groups, manages users. |

> Roles are stored in `users.role` in the database.
> After first Entra ID login, a user is created with role `student`.
> An admin must manually promote users to `group_leader` or `admin`.

---

## 2. Permissions Matrix

| Feature | Student | Group Leader | Admin |
|---|---|---|---|
| **Authentication** | | | |
| Login via Entra ID | ✓ | ✓ | ✓ |
| View own profile | ✓ | ✓ | ✓ |
| Edit own profile (name, avatar) | ✓ | ✓ | ✓ |
| **Groups** | | | |
| View list of active groups | ✓ | ✓ | ✓ |
| View group details | ✓ | ✓ | ✓ |
| Search / filter groups | ✓ | ✓ | ✓ |
| Submit group creation request | ✓ | ✓ | ✓ |
| Edit own group info | — | ✓ (own groups only) | ✓ (all) |
| Approve/reject group requests | — | — | ✓ |
| Activate/deactivate groups | — | — | ✓ |
| **Membership** | | | |
| Send membership request | ✓ | ✓ | ✓ |
| View own memberships | ✓ | ✓ | ✓ |
| View pending requests for a group | — | ✓ (own groups only) | ✓ (all) |
| Approve/reject membership requests | — | ✓ (own groups only) | ✓ (all) |
| Remove a member from a group | — | ✓ (own groups only) | ✓ (all) |
| **Events** | | | |
| View upcoming events | ✓ | ✓ | ✓ |
| View event details | ✓ | ✓ | ✓ |
| Search / filter events | ✓ | ✓ | ✓ |
| RSVP to events | ✓ | ✓ | ✓ |
| Create events | — | ✓ (own groups only) | ✓ (all) |
| Edit/cancel events | — | ✓ (own groups only) | ✓ (all) |
| View registered participants | — | ✓ (own groups only) | ✓ (all) |
| **Notifications** | | | |
| Receive in-app notifications | ✓ | ✓ | ✓ |
| Receive email notifications | ✓ | ✓ | ✓ |
| **Dashboard** | | | |
| Student dashboard | ✓ | ✓ | — |
| Group Leader dashboard | — | ✓ | ✓ |
| Admin dashboard | — | — | ✓ |
| **Reports & Metrics** | | | |
| View basic platform metrics | — | — | ✓ |
| File a report | ✓ | ✓ | ✓ |
| Review and resolve reports | — | — | ✓ |
| **User Management** | | | |
| View all users | — | — | ✓ |
| Change user role | — | — | ✓ |
| Activate/deactivate users | — | — | ✓ |

---

## 3. Role Assignment Flow

```
1. User logs in with @uerre.mx Microsoft account
   │
2. System creates user with role: student
   │
3. Admin reviews users in admin panel
   │
4. Admin promotes user:
   ├─ To group_leader: user can now manage groups where they have a role assignment
   └─ To admin: user has full platform access
   │
5. Admin creates a role assignment in the group:
   └─ groupId + userId + permissionRole: 'leader'
      This links the group_leader to their specific group(s)
```

---

## 4. Group Leader — Scope Restriction

A `group_leader` user can only manage groups where:
- There exists a `role_assignments` record for `(group_id, user_id)` with `permission_role = 'leader'`
- The group has `status = 'active'`

A group leader with no role assignments has no group management capabilities (equivalent to a student).

**API enforcement example:**
```
PATCH /api/groups/{id}/memberships/{mid}/approve
→ Backend checks: does the authenticated user have a role_assignment for group {id}?
→ If not: 403 Forbidden
```

---

## 5. Administrator Capabilities

Administrators bypass all group-scoped permission checks. They can:
- View and manage all users
- Approve or reject group registration requests
- Edit or deactivate any group
- Approve, reject, or remove any membership
- Edit or cancel any event
- View all reports and metrics

---

## 6. Student Responsibilities

A student with role `student` can:
- Browse all active groups publicly
- Submit a membership request to join a group
- View their own membership status
- RSVP to events
- Submit a group creation request (subject to admin approval)
- File reports for inappropriate content

---

## 7. Notification Triggers by Role

| Event | Who is notified | Type |
|---|---|---|
| Membership request sent | Group leader(s) of the group | `new_membership_request` |
| Membership request approved | Student who requested | `membership_approved` |
| Membership request rejected | Student who requested | `membership_rejected` |
| Group creation request approved | User who submitted | `group_approved` |
| Group creation request rejected | User who submitted | `group_rejected` |
| Event updated | Members of the group | `event_update` |
| Event canceled | All RSVPs + members | `event_canceled` |

---

## 8. Access Restriction Implementation

### Frontend (React)

```tsx
// Example route guard
function GroupLeaderRoute({ children }) {
  const { user } = useAuth();
  if (user.role !== 'group_leader' && user.role !== 'admin') {
    return <Navigate to="/dashboard" />;
  }
  return children;
}

// Conditional UI rendering
{user.role === 'admin' && (
  <Button onClick={approveRequest}>Aprobar</Button>
)}
```

### Backend (ASP.NET Core)

```csharp
// Policy-based authorization
[Authorize(Policy = "RequireGroupLeader")]
[HttpPatch("{id}/memberships/{membershipId}/approve")]
public async Task<IActionResult> ApproveMembership(...)
{
    // Additional check: is this leader assigned to THIS group?
    var isLeader = await _groupService.IsLeaderOfGroup(userId, groupId);
    if (!isLeader && !User.IsInRole("Admin"))
        return Forbid();
    // ...
}
```
