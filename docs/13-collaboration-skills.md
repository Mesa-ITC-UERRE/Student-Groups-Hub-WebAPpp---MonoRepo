# 13 — Colaboración por Skills

## Propósito

Student Groups Hub adopta el mecanismo de ARIA para que los agentes trabajen con
reglas del repositorio, contexto de dominio, gates especializados y evidencia
trazable. Una skill es un contrato Markdown activado por intención; no es un
servicio en runtime ni un reemplazo de CI, tests o revisión humana.

## Ubicación y contrato

Cada skill del proyecto vive en `.agents/skills/<nombre>/` y contiene:

1. `SKILL.md` con frontmatter `name` y `description`;
2. `agents/openai.yaml` con nombre, descripción y prompt de interfaz;
3. propósito, reglas duras, procedimiento y verificación.

El índice operativo está en [`.agents/skills/README.md`](../.agents/skills/README.md).
Las skills externas de Supabase permanecen en el mismo directorio.

## Flujo estándar

```text
issue
  ↓
student-groups-navigator → repo-standards + mapa de capas vigente
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
release-readiness → versión candidata validada
```

## Catálogo

| Skill | Entrega principal | Gate |
|---|---|---|
| `student-groups-navigator` | Mapa de capas, dominio, evidencia e invariantes | Antes de diseñar |
| `repo-standards` | Arquitectura, límites, estado, contratos, errores y calidad | Todo cambio |
| `issue-to-change` | Cambio proporcional; plan durable solo cuando aporta valor | Todo issue |
| `security-review` | Hallazgos con severidad y evidencia | Cambios sensibles |
| `documentation-maintainer` | Documentos y contratos sincronizados | Cambios observables |
| `graphify-maintainer` + `graphify` | Knowledge graph de arquitectura, dependencias y roadmap | Cambios estructurales |
| `release-readiness` | Veredicto GO/NO-GO | Despliegue de una versión candidata |

## Contexto del producto

- Runtime: Blazor Server .NET 10 en `blazor/`.
- Auth: Microsoft Entra ID; autorización por rol y por grupo.
- Roles: `student`, `group_leader`, `admin`.
- Datos: EF Core/Npgsql sobre Supabase PostgreSQL; Storage para archivos.
- Presentación: Razor Components; estado visual local y servicios scoped de
  circuito para señales compartidas pequeñas.
- Aplicación: fachadas internas `*ApiService` que orquestan servicios y mapean a
  modelos de presentación; no son clientes HTTP.
- API REST: controllers + DTOs sobre los mismos servicios de feature.
- Integraciones: Resend y Azure según workflow vigente.
- UX: español, identidad U-ERRE y restricciones de `DESIGN.md`.
- Pruebas funcionales: `TC-*` en `docs/09-test-cases.md`.

## Política de evidencia

Distinguir `ejecutado`, `observado`, `planificado` y `bloqueado`. No declarar
build, smoke, render, migración o revisión de seguridad no ejecutados.

## Deuda arquitectónica conocida

- `ApiServices.cs` y varias páginas Razor concentran demasiadas responsabilidades;
  separar por feature al tocar cada flujo, sin reescritura masiva.
- El manejo de errores mezcla excepciones genéricas, mensajes crudos y fallbacks;
  normalizar de forma vertical por issue.
- No existe todavía un proyecto de pruebas automatizadas .NET.
- Parte de la documentación conserva React/API separada y App Service; debe
  reconciliarse y marcarse histórica por issue.
- Roadmap y código pueden divergir; release-readiness exige evidencia actual.
