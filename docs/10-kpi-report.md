# 10 — KPI & Results Report

## 1. Purpose

This document defines the Key Performance Indicators (KPIs) for the Student Groups Hub platform, establishes measurement methods, and provides a template for recording baseline vs. post-implementation results for academic project evaluation.

---

## 2. KPI Definitions

### KPI-01 — Registered Students

| Attribute | Value |
|---|---|
| **Description** | Total number of students registered on the platform |
| **Metric** | Count of `users` records with `role = 'student'` and `status = 'active'` |
| **API endpoint** | `GET /api/reports/metrics` → `userRegistrations.total` |
| **Target** | 100+ in first semester |
| **Measurement frequency** | Monthly |

---

### KPI-02 — Active Student Groups

| Attribute | Value |
|---|---|
| **Description** | Number of student groups with `status = 'active'` |
| **Metric** | Count of `groups` with `status = 'active'` |
| **API endpoint** | `GET /api/reports/metrics` → `groupsCreated` |
| **Target** | 20+ active groups at launch |
| **Measurement frequency** | Monthly |

---

### KPI-03 — Membership Requests Processed

| Attribute | Value |
|---|---|
| **Description** | Total membership requests and their outcomes |
| **Metric** | Count of `memberships` by status: pending / accepted / rejected / removed |
| **Sub-metrics** | Approval rate = accepted / (accepted + rejected) × 100 |
| **API endpoint** | `GET /api/reports/metrics` → `membershipRequests` |
| **Target** | > 70% approval rate |
| **Measurement frequency** | Monthly |

---

### KPI-04 — Events Created

| Attribute | Value |
|---|---|
| **Description** | Total events organized by student groups |
| **Metric** | Count of `events` by status: draft / published / canceled / past |
| **API endpoint** | `GET /api/reports/metrics` → `eventsCreated` |
| **Target** | 10+ events per month across all groups |
| **Measurement frequency** | Monthly |

---

### KPI-05 — Event Participations (RSVPs)

| Attribute | Value |
|---|---|
| **Description** | Total student registrations for events |
| **Metric** | Count of `event_participations` with `status = 'going'` |
| **API endpoint** | `GET /api/reports/metrics` → `eventParticipations` |
| **Target** | Average 15+ participants per event |
| **Measurement frequency** | Per event + monthly |

---

### KPI-06 — Group Visibility Increase

| Attribute | Value |
|---|---|
| **Description** | Increase in the number of discoverable groups vs. pre-platform baseline |
| **Metric** | Count of active groups at measurement date vs. baseline (before platform) |
| **Baseline** | Number of groups with known informal presence (WhatsApp/email) before launch |
| **Target** | 30%+ increase in discoverable groups |
| **Measurement frequency** | End of semester |

---

### KPI-07 — Time to Process Membership Requests

| Attribute | Value |
|---|---|
| **Description** | Average time between a membership request being submitted and a leader responding |
| **Metric** | AVG(`responded_at` - `requested_at`) for memberships with `status IN ('accepted', 'rejected')` |
| **Target** | < 48 hours average response time |
| **Measurement frequency** | Monthly |

---

### KPI-08 — Platform Satisfaction Rate

| Attribute | Value |
|---|---|
| **Description** | Percentage of users who rate the platform as satisfactory |
| **Metric** | Survey result (external — not tracked in platform DB) |
| **Target** | > 80% satisfaction |
| **Measurement frequency** | End of semester |

---

## 3. Results Report Template

Fill in the following table at each measurement interval.

### Monthly Report — [Month/Year]

| KPI | Metric | Baseline | This Month | Cumulative | Target | Status |
|---|---|---|---|---|---|---|
| KPI-01 Registered students | Count | 0 | | | 100 | |
| KPI-02 Active groups | Count | 0 | | | 20 | |
| KPI-03 Membership requests | Total | 0 | | | — | |
| KPI-03 Approval rate | % | — | | | > 70% | |
| KPI-04 Events created | Count | 0 | | | 10/month | |
| KPI-05 Event RSVPs | Count | 0 | | | 15/event avg | |
| KPI-06 Group visibility | % increase | — | | | +30% | |
| KPI-07 Avg response time | Hours | — | | | < 48h | |
| KPI-08 Satisfaction | % | — | — | — | > 80% | |

---

## 4. Before/After Comparison

For academic evaluation, a before/after analysis should document:

### Before Platform (Baseline)
- How were groups discovered? (word of mouth, physical noticeboards, social media)
- How were membership requests managed? (WhatsApp DMs, email)
- How were events announced? (Instagram posts, WhatsApp broadcasts)
- Estimated time to process a membership request: ___
- Number of discoverable groups: ___
- Estimated total students participating in groups: ___

### After Platform (End of First Semester)
Fill using data from `GET /api/reports/metrics`:
- Number of discoverable groups: ___
- Total registered students: ___
- Total membership requests processed: ___
- Average time to process membership request: ___
- Total events created: ___
- Total event participations recorded: ___

---

## 5. Admin Dashboard KPI Cards

The Admin Dashboard at `/admin` will display real-time KPI cards:

```
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│  Estudiantes    │  │  Grupos Activos  │  │  Eventos / Mes  │
│     3,400       │  │      120         │  │       24        │
│  +120 este mes  │  │  +8 este mes     │  │  +6 vs anterior │
└─────────────────┘  └─────────────────┘  └─────────────────┘

┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│  Solicitudes    │  │  Tasa Aprobación │  │  Participaciones │
│   Procesadas    │  │   Membresías     │  │    en Eventos    │
│      890        │  │      81%         │  │      4,200      │
└─────────────────┘  └─────────────────┘  └─────────────────┘
```

---

## 6. Data Export

For project evaluation, the following data exports should be available:

- **Users export:** `GET /api/admin/export/users` → CSV with id, email, role, status, created_at
- **Groups export:** `GET /api/admin/export/groups` → CSV with id, name, category, status, member_count, created_at
- **Events export:** `GET /api/admin/export/events` → CSV with id, title, group, start_at, capacity, rsvp_count
- **Memberships export:** `GET /api/admin/export/memberships` → CSV with membership stats per group

These endpoints are admin-only and generate CSV files for offline analysis (Excel-compatible).

---

## 7. Academic Report Summary

For the final project deliverable, include:

### Quantitative Evidence
- Screenshots of Admin Dashboard KPI cards
- Exported CSV data tables
- Charts generated from the Reports Module

### Qualitative Evidence
- Student satisfaction survey results
- Group leader testimonials on time savings
- Before/after comparison narrative

### System Functionality Evidence
- Screenshots of each main workflow (completed test cases from TC-* document)
- Video walkthrough of all 3 user roles
- Demonstration of key business rules enforcement (duplicate request blocked, canceled event RSVP blocked, etc.)
