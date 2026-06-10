# 02 — System Architecture

## 1. Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        CLIENT (Browser)                          │
│                                                                  │
│   React SPA (Vite + TypeScript)                                  │
│   Tailwind CSS + shadcn/ui                                       │
│   MSAL.js (Microsoft Entra ID auth)                              │
│   TanStack Query (server state)                                  │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                    Bearer JWT (Entra ID token)
                             │
┌────────────────────────────▼─────────────────────────────────────┐
│                     BACKEND API                                   │
│                                                                   │
│   ASP.NET Core 8 Web API (C#)                                     │
│   Entity Framework Core 8 (Npgsql)                                │
│   Microsoft.Identity.Web (JWT validation)                         │
│   Resend SDK (email notifications)                                │
│   Supabase Storage SDK (file uploads)                             │
└──────────┬────────────────────────────────────────┬──────────────┘
           │                                        │
           ▼                                        ▼
┌──────────────────────┐              ┌─────────────────────────┐
│  Supabase PostgreSQL  │              │   Supabase Storage      │
│  (primary database)   │              │   (images/files)        │
└──────────────────────┘              └─────────────────────────┘

                             ┌────────────────────────┐
                             │  Microsoft Entra ID     │
                             │  (single tenant auth)   │
                             │  tenant: uerre.mx       │
                             └────────────────────────┘

                             ┌────────────────────────┐
                             │  Resend                 │
                             │  (transactional email)  │
                             └────────────────────────┘
```

---

## 2. Tech Stack

### Frontend

| Concern | Technology | Notes |
|---|---|---|
| Framework | **React 19** + **Vite** | SPA, TypeScript |
| Styling | **Tailwind CSS v4** | PostCSS-driven config |
| UI Components | **shadcn/ui** (new-york style) | Built on Radix primitives |
| Icons | **Lucide React** | Outlined, 1.5px stroke |
| Auth client | **@azure/msal-browser v5** + **@azure/msal-react v5** | Entra ID PKCE flow |
| Server state | **TanStack Query (React Query)** | Caching, invalidation, loading states |
| Client state | **React Context** | Auth context, user context |
| Forms | **React Hook Form** | With Zod validation |
| HTTP client | **Fetch API** (custom wrapper) | Bearer token injection |
| Routing | **React Router v7** | SPA routing |
| Linting | **Biome** | Replaces ESLint + Prettier |

### Backend

| Concern | Technology | Notes |
|---|---|---|
| Framework | **ASP.NET Core 8** | Web API project |
| Language | **C# 12** | |
| ORM | **Entity Framework Core 8** | Code-first, migrations |
| DB driver | **Npgsql.EntityFrameworkCore.PostgreSQL** | For Supabase PostgreSQL |
| Auth | **Microsoft.Identity.Web** | Validates Entra ID JWTs |
| File storage | **Supabase .NET SDK** | Or direct REST calls |
| Email | **Resend .NET SDK** | Transactional email |
| Validation | **FluentValidation** | Request DTO validation |
| Logging | **Serilog** | Structured logging |
| API docs | **Scalar** (or Swagger/OpenAPI) | Auto-generated API docs |

### Infrastructure

| Service | Purpose | Notes |
|---|---|---|
| **Supabase** | PostgreSQL + File Storage | Free tier sufficient for MVP |
| **Microsoft Entra ID** | Authentication | Single tenant (`@uerre.mx`) |
| **Resend** | Email notifications | Free: 3,000 emails/month |
| **Azure Static Web Apps** | Frontend hosting | CI/CD from GitHub |
| **Azure App Service** | Backend hosting | B1 tier for MVP |

---

## 3. Authentication Flow

```
1. User clicks "Continuar con Microsoft"
   │
2. MSAL.js initiates loginRedirect()
   │
3. Browser redirects to: login.microsoftonline.com/{tenantId}
   │
4. User authenticates with @uerre.mx credentials
   │
5. Entra ID redirects back to /auth/callback with auth code
   │
6. MSAL.js exchanges code for tokens (PKCE)
   │  access_token (for API calls)
   │  id_token (user identity)
   │
7. Frontend calls GET /api/users/me with Bearer access_token
   │
8. Backend (Microsoft.Identity.Web) validates JWT:
   │  - Checks signature, issuer, audience, expiry
   │  - Extracts oid (Entra Object ID) from token claims
   │
9. Backend creates user in DB if first login (upsert by entra_oid)
   │  Default role: STUDENT
   │
10. Backend returns user profile with role
    │
11. Frontend stores role in React Context
    │  Routes/UI adapt to role
```

---

## 4. Role-Based Access Control

Access control is enforced at two layers:

**Frontend layer:** React Router guards hide routes and UI elements based on the role stored in the user context.

**Backend layer:** ASP.NET Core authorization policies enforce permissions on every API endpoint, regardless of frontend state.

```csharp
// Example backend policy definitions
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireStudent", p => p.RequireAuthenticatedUser());
    options.AddPolicy("RequireGroupLeader", p => p.RequireRole("GroupLeader", "Admin"));
    options.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
});
```

---

## 5. Project Directory Structure

### Frontend (`student-groups-hub-fronted/`)

```
src/
├── app/                    # (current Next.js prototype — migrate to React Router)
├── components/
│   ├── ui/                 # shadcn/ui primitives (button, card, input, etc.)
│   ├── layout/             # Navbar, Footer, PageLayout
│   └── shared/             # JoinButton, NotificationBell, AvatarUpload, etc.
├── pages/                  # Route-level page components
│   ├── home/               # Landing page
│   ├── login/              # Login + auth callback
│   ├── dashboard/          # Role-specific dashboard
│   ├── groups/             # Groups list + detail + register
│   ├── events/             # Events list + detail
│   └── admin/              # Admin panel pages
├── hooks/                  # Custom hooks (useAuth, useGroups, useEvents, etc.)
├── services/               # Typed API client (api.ts)
├── context/                # React Context providers (AuthContext, etc.)
├── types/                  # TypeScript interfaces (matching API DTOs)
└── lib/                    # Utilities (cn, msal config, constants)
```

### Backend (to be created: `student-groups-hub-backend/`)

```
MesaITC.sln
├── MesaITC.API/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── UsersController.cs
│   │   ├── GroupsController.cs
│   │   ├── MembershipsController.cs
│   │   ├── EventsController.cs
│   │   ├── ParticipationsController.cs
│   │   ├── NotificationsController.cs
│   │   └── ReportsController.cs
│   ├── Models/              # EF Core entity classes
│   ├── DTOs/                # Request + Response data transfer objects
│   ├── Services/            # Business logic layer
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── Migrations/
│   ├── Middleware/          # Error handling, logging
│   └── Program.cs
└── MesaITC.Tests/           # (optional) xUnit tests
```

---

## 6. API Design Principles

- **RESTful** resource-oriented endpoints
- **JSON** request and response bodies
- **Bearer token** authentication on protected endpoints
- **Consistent error format:**
  ```json
  {
    "status": 400,
    "message": "El nombre del grupo ya existe.",
    "timestamp": "2026-06-10T14:30:00Z"
  }
  ```
- **Pagination** for list endpoints: `?page=1&pageSize=20`
- **Soft deletes** for groups and users (status = inactive)
- **Audit fields** (`createdAt`, `updatedAt`) on all entities

---

## 7. Deployment Pipeline

```
GitHub Repository
      │
      ├─ Push to main ──► GitHub Actions
      │                        │
      │                        ├─ Build React SPA
      │                        │   └─► Azure Static Web Apps (frontend)
      │                        │
      │                        └─ Build .NET API
      │                            └─► Azure App Service (backend)
      │
      └─ Database
          └─► Supabase (managed PostgreSQL + Storage)
                  │
                  └─ EF Core Migrations run on startup
```

---

## 8. Environment Configuration

### Frontend `.env` variables

```env
VITE_ENTRA_CLIENT_ID=<entra-app-client-id>
VITE_ENTRA_TENANT_ID=<entra-tenant-id>
VITE_API_URL=https://api.studentgroupshub.uerre.mx
```

### Backend `appsettings.json` (environment overrides in Azure)

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "<tenant-id>",
    "ClientId": "<api-app-client-id>",
    "Audience": "api://<api-app-client-id>"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=...;Database=...;Username=...;Password=..."
  },
  "Supabase": {
    "Url": "https://<project>.supabase.co",
    "ServiceKey": "<service-role-key>",
    "StorageBucket": "student-groups-assets"
  },
  "Resend": {
    "ApiKey": "<resend-api-key>",
    "FromEmail": "noreply@studentgroupshub.uerre.mx"
  }
}
```
