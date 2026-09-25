# Student Groups Hub — U-ERRE

Platform for managing student groups, events, members, and participation at **Universidad Regiomontana (U-ERRE)**.

> Made for students, by students.

---

## Structure

```
Student-Groups-Hub/
├── blazor/    — Blazor Server (.NET 10)  ← main application
├── docs/          — Project documentation and issue plans
├── graphify-out/  — Generated Graphify knowledge graph and report
└── .agents/skills/ — ARIA-style collaboration skills + Graphify
```

The repository uses the ARIA-style skill mechanism described in
[`docs/13-collaboration-skills.md`](./docs/13-collaboration-skills.md). Start
with [`student-groups-navigator`](./.agents/skills/student-groups-navigator/SKILL.md)
and `repo-standards` before changing code.

---

## Quick Start

```bash
cd blazor

# 1. Copy the config template and fill in your credentials
cp appsettings.Development.json.template appsettings.Development.json

# 2. Run
DOTNET_ROOT=~/.dotnet ~/.dotnet/dotnet run --launch-profile https
# → https://localhost:7013
```

---

## Configuration

Copy `blazor/appsettings.Development.json.template` to `blazor/appsettings.Development.json` and fill in:

| Setting | Where to find it |
|---|---|
| `EntraId.TenantId` | Azure Portal → App registrations → Directory (tenant) ID |
| `EntraId.ClientId` | Azure Portal → App registrations → Application (client) ID |
| `EntraId.ClientSecret` | Azure Portal → App registrations → Certificates & secrets |
| `ConnectionStrings.DefaultConnection` | Supabase Dashboard → Database → Transaction Pooler (port 6543) |
| `Supabase.ServiceKey` | Supabase Dashboard → Project Settings → API → Secret keys |
| `Resend.ApiKey` | resend.com Dashboard → API Keys |

---

## Collaboration by skills

- `issue-to-change`: issue → plan → code → tests → documentation.
- `security-review`: authorization, privacy, validation and sensitive data.
- `documentation-maintainer`: keeps affected documents and contracts aligned.
- `graphify-maintainer`: updates architecture, dependencies and roadmap graph.
- `release-readiness`: release-branch gate before `main`.

See the [operational guide](./docs/13-collaboration-skills.md) and the
[skills index](./.agents/skills/README.md) and the [Graphify output](./graphify-out/GRAPH_REPORT.md).

## Committing

```bash
cz commit    # interactive conventional commit
cz bump      # bump version + update CHANGELOG.md
```

See `docs/12-versioning.md` for full conventions.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Blazor Server (.NET 10) |
| Auth | Microsoft Entra ID (single tenant `@uerre.mx`) |
| UI | Bootstrap 5 + custom U-ERRE CSS |
| Database | Supabase (PostgreSQL) via EF Core 10 |
| Email | Resend |
| Deployment | Azure App Service |
