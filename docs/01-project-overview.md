# 01 — Project Overview

## 1. Context

**Student Groups Hub** is a web platform developed for **Universidad Regiomontana (U-ERRE)** that centralizes the management of student organizations, their events, members, and participation.

The platform solves a concrete institutional problem: student groups are currently managed through informal channels (WhatsApp, email, physical noticeboards), making it difficult for students to discover groups, for leaders to manage members, and for administrators to have visibility over platform activity.

---

## 2. General Objective

Develop a functional platform that allows:
- Students to discover, join, and participate in student groups and their events.
- Group leaders to manage their community, membership requests, and events.
- Administrators to supervise platform activity, approve groups, and maintain data integrity.

---

## 3. Specific Objectives

1. Allow students to browse and search available student groups.
2. Allow students to request and track membership in groups.
3. Allow group leaders to manage group information and members.
4. Allow group leaders to create, edit, and cancel events.
5. Allow administrators to approve groups and manage users.
6. Improve the visibility and discoverability of student groups on campus.
7. Maintain structured, queryable records of groups, members, events, and participation.
8. Provide dashboards and basic metrics for each user role.

---

## 4. Scope

### In Scope

- Web application (desktop + responsive mobile)
- User authentication via Microsoft Entra ID (single tenant: `@uerre.mx`)
- Group management with admin approval workflow
- Membership request and approval system
- Event creation, management, and registration
- Role-specific dashboards (Student / Group Leader / Administrator)
- In-app and email notifications
- Search and filtering for groups and events
- File uploads (group logos, event banners, profile pictures)
- Basic reports and KPI metrics

### Out of Scope (Phase 1)

- Mobile native application
- Real-time chat / messaging between users
- QR code attendance confirmation
- Calendar integrations (Google Calendar, Outlook)
- Group recommendation algorithm
- Advanced analytics or BI dashboards
- Public-facing group pages (no auth required for viewing is in scope; public indexing is out)

---

## 5. University Context

| Attribute | Value |
|---|---|
| Institution | Universidad Regiomontana (U-ERRE) |
| Location | Monterrey, Nuevo León, Mexico |
| Website | https://www.u-erre.mx |
| Auth domain | `@uerre.mx` |
| Entra ID model | Single tenant — only institutional accounts |

---

## 6. Platform Identity

- **Name:** Student Groups Hub
- **Tagline:** *Hecho para estudiantes, por estudiantes*
- **Language:** Spanish (UI and all user-facing content)
- **Tone:** Professional but approachable — more casual than the institutional U-ERRE website, with emphasis on community and social elements
- **Brand base:** U-ERRE official brand colors (purple palette) — see [07-ui-design.md](./07-ui-design.md)

---

## 7. Non-Functional Requirements Summary

| Category | Requirement |
|---|---|
| Usability | Minimal steps to complete common actions; clear forms with validation |
| Accessibility | WCAG AA contrast ratios; keyboard navigation; readable text |
| Security | Role-based access control; input validation; secure credential storage; Entra ID auth |
| Performance | Acceptable response times; efficient search/filter operations |
| Scalability | Modular design allowing future growth in users, groups, and features |
| Maintainability | Consistent naming conventions; clear module separation; documentation |
| Reliability | Graceful error handling; meaningful error messages; data consistency |

---

## 8. Business Rules

1. A user must be authenticated (Entra ID) to join a group.
2. A user cannot send duplicate membership requests to the same group.
3. Only group leaders can manage the groups assigned to them.
4. Only administrators can manage all groups and users.
5. A group must have at least one responsible leader at all times.
6. An event must belong to an active, approved group.
7. A canceled event must not allow new registrations.
8. An inactive or removed user must not be able to manage groups.
9. A group leader cannot approve requests for groups they do not manage.
10. New groups require administrator approval before becoming publicly visible.

---

## 9. Minimum Deliverables

1. Functional platform prototype / complete system
2. Requirements document *(this file and requirements.md)*
3. Analysis and design documentation *(this docs folder)*
4. Data model / entity-relationship diagram *(03-data-model.md)*
5. User role descriptions *(05-user-roles.md)*
6. Functional module descriptions *(06-modules.md)*
7. Evidence of system functionality *(screenshots, demo video)*
8. Test cases / validation evidence *(09-test-cases.md)*
9. Technical documentation *(02-architecture.md)*
10. KPI / results report *(10-kpi-report.md)*

---

## 10. Acceptance Criteria

The project is considered complete when:

- [ ] Users can register and log in via Microsoft Entra ID
- [ ] Users access features according to their assigned role
- [ ] Students can view and join student groups
- [ ] Group leaders can manage their groups and membership requests
- [ ] Group leaders can create and manage events
- [ ] Administrators can supervise users and groups
- [ ] The platform stores and retrieves data correctly
- [ ] Main workflows complete without critical errors
- [ ] The system includes all documentation deliverables
