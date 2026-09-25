---
name: student-groups-navigator
description: 'Navegar el dominio y la arquitectura reales de Student Groups Hub antes de diseñar o implementar cambios. Use when: entender una feature, localizar código o documentación canónica, resolver contradicciones entre docs y runtime, revisar roles, grupos, membresías, eventos, notificaciones, temporadas, publicaciones, auth Entra, Supabase o despliegue. Triggers: student groups, grupo, membresía, líder, evento, RSVP, dashboard, admin, arquitectura, dominio, roadmap, Supabase, Entra.'
---

# Student Groups Navigator

## Propósito

Ubicar una tarea dentro del producto real, separar estado implementado,
planificado y documentación desactualizada, y devolver los invariantes que el
cambio no puede romper. Es el equivalente de `aria-spec-navigator`.

## Fuentes de verdad

1. Código compilable y configuración ejecutable: runtime actual.
2. `docs/01-project-overview.md`, `03-data-model.md`, `05-user-roles.md`,
   `08-workflows.md` y `11-development-plan.md`: negocio y plan.
3. `DESIGN.md`: identidad visual.
4. `docs/04-api-contract.md`: contrato previsto; compararlo con controllers y
   DTOs antes de tratarlo como vigente.

El runtime actual es `blazor/`, .NET 10, Interactive Server, EF Core/Npgsql,
Microsoft Entra ID, Supabase PostgreSQL/Storage, Resend y el workflow de
`.github/workflows/deploy-main.yml`. No inventar una tercera arquitectura ante
la transición histórica React/API separada → Blazor.

## Mapa rápido

| Concepto | Código | Documentación |
|---|---|---|
| Arranque, auth, DI, middleware | `blazor/Program.cs` | `docs/02-architecture.md` |
| Entidades y estados | `blazor/Models/Entities.cs` | `docs/03-data-model.md` |
| Persistencia y migraciones | `blazor/Data/` | `docs/03-data-model.md` |
| Reglas de negocio | `blazor/Services/` | `docs/01-project-overview.md`, `08-workflows.md` |
| API HTTP | `blazor/Controllers/`, `blazor/DTOs/` | `docs/04-api-contract.md` |
| UI | `blazor/Components/` | `docs/06-modules.md`, `07-ui-design.md`, `DESIGN.md` |
| Roles y alcance de líder | `blazor/Extensions/`, `blazor/Services/` | `docs/05-user-roles.md` |
| Skills | `.agents/skills/` | `docs/13-collaboration-skills.md` |
| Grafo | `graphify-out/` | `docs/13-collaboration-skills.md` |

## Invariantes

- Solo cuentas `@uerre.mx` autenticadas por Entra usan operaciones protegidas.
- `student`, `group_leader` y `admin` tienen alcances distintos.
- La UI nunca es frontera de seguridad; el servidor vuelve a comprobar el grupo.
- No hay solicitudes activas de membresía duplicadas.
- Eventos cancelados o llenos no aceptan RSVP.
- Notificaciones, correos y enlaces no filtran PII a destinatarios incorrectos.
- Credenciales de Entra, Supabase y Resend solo viven en configuración de
  servidor.
- Si se toca Supabase, aplicar también las dos skills Supabase instaladas.

## Procedimiento

1. Clasificar la tarea: auth, usuarios, grupos, membresías, eventos/RSVP,
   notificaciones, dashboards, admin, archivos, datos, UI, despliegue o docs.
2. Leer el documento primario y la implementación correspondiente.
3. Buscar callers, estados, roles y contratos afectados.
4. Marcar qué está implementado, planificado o desactualizado.
5. Entregar archivos, invariantes, pruebas, documentación y grafo afectados.
6. Registrar contradicciones; no ocultarlas ni cambiar la arquitectura sin issue.

## Skills relacionadas

`repo-standards`, `issue-to-change`, `security-review`,
`documentation-maintainer`, `graphify-maintainer`.
