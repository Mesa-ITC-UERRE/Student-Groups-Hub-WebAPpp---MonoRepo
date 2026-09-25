---
name: repo-standards
description: 'Aplicar los estándares técnicos de Student Groups Hub: estructura, estilo C# y Razor, separación de capas, seguridad, documentación, configuración, pruebas y convenciones. Use when: escribir, revisar o refactorizar código; crear endpoints, servicios, componentes, migraciones, workflows o documentación. Triggers: estándares, convenciones, estructura, estilo, calidad, review, C#, Razor, Blazor, EF Core, configuración, seguridad, pruebas.'
---

# Repository Standards

## Estructura

- `blazor/`: aplicación ejecutable .NET 10.
- `Components/`: páginas, layouts y componentes Razor.
- `Controllers/`: límite HTTP; no concentrar reglas de negocio.
- `Services/`: casos de uso y coordinación de datos.
- `Models/`, `DTOs/`, `Data/`, `Middleware/`, `Extensions/`: responsabilidades
  actuales.
- `docs/`: negocio, arquitectura, contratos, workflows, pruebas, roadmap y
  colaboración.
- `.agents/skills/`: skills del proyecto y externas.
- `.github/workflows/`: automatización.

No crear una segunda SPA React, otro backend o un segundo mecanismo de skills.

## Estilo

- C# con nullable e implicit usings; PascalCase para tipos/métodos y camelCase
  para parámetros/campos privados.
- Controllers delgados: auth, binding, validación de borde y delegación.
- Usar `IDbContextFactory<AppDbContext>` en operaciones Blazor susceptibles a
  solapamiento; preferir async/await, proyecciones y consultas por lotes.
- DTOs separan contratos HTTP de entidades y minimizan PII.
- UI en español; identificadores técnicos en inglés; reutilizar `DESIGN.md`.
- Formularios con estados loading, error, vacío y éxito; foco visible y reflow.

## Seguridad

- Entra autentica; el servidor autoriza. Nunca confiar en UI, role del cliente o
  `groupId` sin comprobar.
- Líderes requieren `RoleAssignment` activa para el grupo objetivo; admin es la
  única excepción global.
- Validar límites, formato, estados, fechas, capacidad, ownership y transiciones.
- Nunca registrar tokens, secretos, cookies, connection strings ni payloads PII.
- Service keys y credenciales solo en servidor; nunca en `wwwroot` o JavaScript.
- Uploads validan tamaño, MIME, contenido, nombre, destino y ownership.
- Cambios Supabase siguen sus skills, incluyendo RLS, grants, Storage y migraciones.

## Entrega

Cada cambio no trivial declara issue, alcance, archivos, invariantes, pruebas,
riesgos y documentación. Usar `docs/plans/issue-<id>-<slug>.md` para planes.
Conventional Commits siguen `docs/12-versioning.md`.

## Verificación

- `dotnet restore blazor/StudentGroupsHub.csproj`.
- `dotnet build blazor/StudentGroupsHub.csproj --configuration Release`.
- Tests disponibles y smoke de la ruta cambiada.
- Revisión de secretos, autorización, warnings y documentación.

Deuda conocida: `.husky/pre-commit` y `.husky/commit-msg` apuntan a
`frontend/`, aunque el runtime está en `blazor/`; release-readiness debe marcarlo.
