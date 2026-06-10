# Student Groups Hub — Monorepo

Platform for managing student groups, events, and participation at **Universidad Regiomontana (U-ERRE)**.

## Structure

```
student-groups-hub/
├── docs/        — Project documentation (12 markdown files)
├── frontend/    — React 19 + Vite + TypeScript SPA
└── backend/     — ASP.NET Core 10 Web API (C#)
```

## Quick Start

### Frontend
```bash
cd frontend
bun install
bun dev          # → http://localhost:5173
```

### Backend
```bash
cd backend
dotnet restore
dotnet run       # → http://localhost:8080
```

## Committing

**Always use commitizen — never `git commit -m "..."`**

```bash
# From the repo root:
cd frontend && bun run cm
```

See `docs/12-versioning.md` for the full conventions guide.
