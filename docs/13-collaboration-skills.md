# 13 — Colaboración por Skills

## Propósito

Student Groups Hub adopta el mecanismo de ARIA para que los agentes trabajen con
reglas del repositorio, contexto de dominio, gates especializados y evidencia
trazable. Una skill es un contrato Markdown activado por intención; no es un
servicio en runtime ni un reemplazo de CI, tests o revisión humana.

## Ubicación y contrato

Cada skill del proyecto vive en `.agents/skills/<nombre>/` y contiene:

1. frontmatter con `name`, `description` y, cuando ayuda, `argument-hint`;
2. propósito, reglas duras, procedimiento y verificación.

El índice operativo está en [`.agents/skills/README.md`](../.agents/skills/README.md).
Las skills externas de Supabase permanecen en el mismo directorio.

## Flujo estándar

```text
issue
  ↓
student-groups-navigator → repo-standards
  ↓
issue-to-change
  ├─ security-review       si toca auth, roles, PII, validación, Storage o datos
  ├─ documentation-maintainer
  └─ graphify-maintainer   si cambia arquitectura, dependencias o roadmap
       ↓
     graphify → graphify-out/graph.json + GRAPH_REPORT.md + graph.html
  ↓
pruebas + smoke + build
  ↓
release-readiness → main
```

## Catálogo

| Skill | Entrega principal | Gate |
|---|---|---|
| `student-groups-navigator` | Mapa de dominio, estado real e invariantes | Antes de diseñar |
| `repo-standards` | Estructura, estilo y seguridad | Todo cambio |
| `issue-to-change` | Plan `docs/plans/issue-<id>-<slug>.md` | Todo issue |
| `security-review` | Hallazgos con severidad y evidencia | Cambios sensibles |
| `documentation-maintainer` | Documentos y contratos sincronizados | Cambios observables |
| `graphify-maintainer` + `graphify` | Knowledge graph de arquitectura, dependencias y roadmap | Cambios estructurales |
| `release-readiness` | Veredicto GO/NO-GO | Promoción a `main` |

## Contexto del producto

- Runtime: Blazor Server .NET 10 en `blazor/`.
- Auth: Microsoft Entra ID; autorización por rol y por grupo.
- Roles: `student`, `group_leader`, `admin`.
- Datos: EF Core/Npgsql sobre Supabase PostgreSQL; Storage para archivos.
- Integraciones: Resend y Azure según workflow vigente.
- UX: español, identidad U-ERRE y restricciones de `DESIGN.md`.
- Pruebas funcionales: `TC-*` en `docs/09-test-cases.md`.

## Política de evidencia

Distinguir `ejecutado`, `observado`, `planificado` y `bloqueado`. No declarar
build, smoke, render, migración o revisión de seguridad no ejecutados.

## Deuda conocida

- `.husky/pre-commit` y `.husky/commit-msg` referencian `frontend/`, aunque el
  runtime está en `blazor/`.
- Parte de la documentación conserva React/API separada y App Service; debe
  reconciliarse por issue.
- Roadmap y código pueden divergir; release-readiness exige evidencia actual.
