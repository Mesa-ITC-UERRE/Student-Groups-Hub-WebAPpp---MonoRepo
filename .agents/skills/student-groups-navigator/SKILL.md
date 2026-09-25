---
name: student-groups-navigator
description: 'Navegar el dominio, arquitectura implementada, documentación y grafo de Student Groups Hub antes de diseñar o modificar una feature. Use when: localizar UI, casos de uso, modelos, DTOs, persistencia, estado, integraciones, callers o invariantes; resolver contradicciones entre código y docs; revisar grupos, membresías, liderazgo, eventos, RSVP, publicaciones, temporadas, notificaciones, admin, Entra, Supabase Storage/PostgreSQL o despliegue. Triggers: student groups, grupo, membresía, líder, evento, RSVP, dashboard, admin, arquitectura, dominio, flujo, caller, Supabase, Entra, Storage.'
---

# Student Groups Navigator

Ubicar la tarea dentro del producto real y devolver estado, archivos, callers,
invariantes, checks y documentos afectados antes de proponer una solución.

## Jerarquía de evidencia

1. Código compilable, migraciones y configuración ejecutable: implementación.
2. Tests y observación reproducible: comportamiento comprobado.
3. Documentos de negocio: intención y reglas; contrastarlos con el runtime.
4. Roadmap y contrato previsto: plan, no evidencia de implementación.
5. Graphify: índice de navegación; confirmar inferencias en código.

Leer el mapa detallado de
[`repo-standards/references/current-architecture.md`](../repo-standards/references/current-architecture.md)
para cambios estructurales. La arquitectura React/API separada en documentos es
histórica; el runtime es Blazor Interactive Server .NET 10.

## Mapa rápido

| Concepto | Implementación principal | Contrato/contexto |
|---|---|---|
| Arranque, DI, auth y middleware | `blazor/Program.cs` | workflow y appsettings vigentes |
| UI y estado local | `blazor/Components/` | `DESIGN.md`, módulos y casos |
| Orquestación Blazor y mappers UI | `Services/ApiServices.cs`, `CurrentUserService` | `Models/AppModels.cs` |
| Estado compartido de circuito | `Services/UserStateService.cs` | consumidores en Components |
| Reglas, consultas y transiciones | servicios de feature | overview, roles y workflows |
| API HTTP | `Controllers/` | `DTOs/`, contrato API |
| Entidades y persistencia | `Models/Entities.cs`, `Data/`, migraciones | modelo de datos |
| Archivos e integraciones | `StorageService`, configuración, workflow | seguridad y operación |
| Arquitectura y callers | código + `graphify-out/` | confirmar nodos inferidos |

## Invariantes del producto

- Operaciones protegidas requieren identidad Entra institucional y usuario activo.
- `student`, `group_leader` y `admin` tienen alcances distintos; un líder solo
  administra grupos con `RoleAssignment` activa.
- La UI y el estado del circuito nunca son frontera de autorización.
- No crear solicitudes activas duplicadas ni transiciones inválidas.
- Eventos cancelados, finalizados o llenos respetan reglas de RSVP y evidencia.
- Notificaciones, participantes, reportes, correos y enlaces minimizan PII.
- Credenciales de Entra, PostgreSQL, Supabase Storage y Resend permanecen en
  servidor. Supabase Auth no pertenece al runtime actual.

## Procedimiento

1. Clasificar feature y tipo de cambio: UI, aplicación, negocio, HTTP, datos,
   estado, integración, seguridad, despliegue o documentación.
2. Consultar Graphify para candidatos y confirmar archivos/callers en código.
3. Seguir el flujo entrada → orquestación → regla → persistencia/integración →
   salida, incluyendo rutas Blazor y REST cuando ambas existan.
4. Localizar actor, recurso, rol, estados, DTO/modelo, mapper, migración y manejo
   de error involucrados.
5. Separar `implementado`, `observado`, `documentado`, `planificado` y
   `contradictorio`; no completar huecos por suposición.
6. Entregar un mapa breve con archivos, invariantes, riesgos, pruebas, documentos
   y necesidad de actualizar el grafo.

No cambiar arquitectura ni convertir roadmap en comportamiento sin un issue que
lo autorice. Aplicar `repo-standards`, y activar las skills especializadas según
el mapa de impacto.
