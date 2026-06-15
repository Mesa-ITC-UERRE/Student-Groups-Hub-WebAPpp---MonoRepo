# Student Groups Hub — Monorepo

Platform for managing student groups, events, and participation at **Universidad Regiomontana (U-ERRE)**.

## Structure

```
student-groups-hub/
├── docs/        — Project documentation
├── frontend/    — React 19 + Vite SPA (visual reference prototype)
├── backend/     — ASP.NET Core 10 REST API (C#)
└── blazor/      — Blazor Server (.NET 10) — main frontend application
```

## Quick Start

### Blazor (main app)
```bash
cd blazor
cp appsettings.Development.json.template appsettings.Development.json
# Fill in credentials in appsettings.Development.json
DOTNET_ROOT=~/.dotnet ~/.dotnet/dotnet run --launch-profile https
# → https://localhost:7013
```

### Backend REST API (optional — already merged into blazor/)
```bash
cd backend
cp appsettings.Development.json.template appsettings.Development.json
# Fill in credentials
DOTNET_ROOT=~/.dotnet ~/.dotnet/dotnet run
# → http://localhost:8080
```

## Configuration

Copy `appsettings.Development.json.template` to `appsettings.Development.json` in both `blazor/` and `backend/` and fill in:

| Setting | Where to find it |
|---|---|
| `EntraId.TenantId` | Azure Portal → App registrations → Directory (tenant) ID |
| `EntraId.ClientId` | Azure Portal → App registrations → Application (client) ID |
| `EntraId.ClientSecret` | Azure Portal → App registrations → Certificates & secrets |
| `ConnectionStrings.DefaultConnection` | Supabase Dashboard → Database → Transaction Pooler (port 6543) |
| `Supabase.ServiceKey` | Supabase Dashboard → Project Settings → API → Secret keys |
| `Resend.ApiKey` | resend.com Dashboard → API Keys |

## Committing

```bash
cz commit    # interactive conventional commit prompt
cz bump      # bump version + update CHANGELOG.md
```
