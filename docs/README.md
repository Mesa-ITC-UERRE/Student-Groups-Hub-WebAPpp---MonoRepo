# Student Groups Hub — Project Documentation

Platform for managing student groups, events, members, and participation at **Universidad Regiomontana (U-ERRE)**.

> Made for students, by students.

---

## Document Index

| File | Description |
|---|---|
| [01-project-overview.md](./01-project-overview.md) | Goals, objectives, scope, and context |
| [02-architecture.md](./02-architecture.md) | System architecture, tech stack, deployment |
| [03-data-model.md](./03-data-model.md) | Entity-relationship model, database schema |
| [04-api-contract.md](./04-api-contract.md) | REST API endpoints, request/response shapes |
| [05-user-roles.md](./05-user-roles.md) | User roles, permissions matrix, access rules |
| [06-modules.md](./06-modules.md) | Functional modules and their responsibilities |
| [07-ui-design.md](./07-ui-design.md) | UI/UX guidelines, design system, prototype reference |
| [08-workflows.md](./08-workflows.md) | Main user workflows and interaction flows |
| [09-test-cases.md](./09-test-cases.md) | Manual test cases for acceptance criteria |
| [10-kpi-report.md](./10-kpi-report.md) | KPIs, metrics definitions, and measurement plan |
| [11-development-plan.md](./11-development-plan.md) | Phased development roadmap with per-phase task checklists |
| [12-versioning.md](./12-versioning.md) | Commit conventions, Commitizen workflow, versioning rules |

---

## Repository Structure

```
StudentGroupsBlazor/
├── docs/                     ← This documentation
└── StudentGroupsHub/         ← Blazor Web App (.NET 10)
```

---

## Quick Reference

- **University:** Universidad Regiomontana (U-ERRE)
- **Platform name:** Student Groups Hub
- **UI Language:** Spanish
- **Frontend:** Blazor Web App (.NET 10) — InteractiveServer render mode
- **Auth provider:** Microsoft Entra ID (single tenant — `@uerre.mx`)
- **Backend API:** ASP.NET Core 10 REST API (separate `student-groups-hub` repo)
- **Database:** Supabase (PostgreSQL)
- **Deployment:** Azure App Service (Blazor + backend)
