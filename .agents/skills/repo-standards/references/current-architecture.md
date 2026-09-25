# Arquitectura vigente de Student Groups Hub

## Propósito

Usar este mapa para colocar cambios en el runtime real. Describe el monolito
modular actual; no convierte la deuda existente en un patrón obligatorio.

## Runtime y flujo principal

- Aplicación única ASP.NET Core/Blazor Interactive Server en `blazor/`, .NET 10.
- Cookie/OpenID Connect para páginas Blazor y JWT Bearer para controllers REST.
- EF Core/Npgsql sobre Supabase PostgreSQL mediante
  `IDbContextFactory<AppDbContext>`.
- Supabase Storage se consume en servidor con `StorageService`.
- Despliegue a Azure Container Apps desde `.github/workflows/deploy-main.yml`.

```text
Razor Components
  → fachadas *ApiService / CurrentUserService
    → servicios de feature
      → AppDbContext / StorageService / integraciones

REST Controllers + DTOs
  → servicios de feature
    → AppDbContext / integraciones
```

## Mapa implementado

| Responsabilidad | Ubicación actual | Convención al extender |
|---|---|---|
| Páginas, layouts y UI reutilizable | `Components/Pages`, `Layout`, `Shared` | Estado visual local y delegación a fachadas |
| Orquestación para Blazor | clases `*ApiService` en `Services/ApiServices.cs` | Separar nuevos flujos en archivo por feature |
| Usuario/circuito actual | `CurrentUserService`, `UserStateService` | Contexto del actor y estado scoped pequeño |
| Reglas y consultas de feature | `GroupService`, `EventService`, etc. | Invariantes y persistencia por operación |
| API REST | `Controllers/`, `DTOs/Requests`, `DTOs/Responses` | Controllers delgados y contratos explícitos |
| Modelos para UI | `Models/AppModels.cs` | No confundir con entidades o DTO HTTP |
| Entidades EF y esquema | `Models/Entities.cs`, `Data/`, `Migrations/` | Entidades no salen de fronteras |
| Integración de archivos | `StorageService` | Red, credenciales y rutas aisladas |
| Errores REST | `Middleware/GlobalExceptionHandler.cs` | Traducción segura y consistente |
| Auth/autorización | `Program.cs`, `AdminRoleHandler`, servicios | Entra autentica; servidor comprueba alcance |

## Patrones que sí deben preservarse

- Contexto EF independiente por operación para evitar concurrencia entre fases o
  eventos de Blazor Server.
- Autorización por recurso y grupo en servidor; ocultar UI no concede permisos.
- Lotes/proyecciones para evitar N+1 y DTO/modelos separados de entidades.
- Integraciones y secretos exclusivamente en servidor.
- Estado visual explícito y componentes reutilizables cuando reducen duplicación.

## Deuda que no debe copiarse

- `ApiServices.cs` y varias páginas Razor concentran demasiadas responsabilidades.
- Mappers equivalentes están dispersos entre servicios y fachadas.
- Muchos fallos distintos usan `InvalidOperationException`; existen capturas
  genéricas, mensajes crudos y lecturas que degradan silenciosamente.
- No hay proyecto de pruebas automatizadas .NET en el repositorio.
- `docs/02-architecture.md` y partes de `docs/06-modules.md` aún describen la
  arquitectura histórica React/API separada.
- Algunos componentes inyectan servicios de feature directamente. No ampliar esa
  excepción cuando exista una fachada de aplicación adecuada.

## Regla de evolución

Mejorar el límite que toque el issue sin una reescritura transversal no solicitada.
Una abstracción nueva debe eliminar duplicación, aislar una integración, expresar
una regla o habilitar una prueba; si solo renombra una llamada EF, no aporta valor.
