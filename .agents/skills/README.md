# Skills de colaboración — Student Groups Hub

Este directorio usa el mecanismo de ARIA: cada skill vive bajo
`.agents/skills/<nombre>/` y su contrato principal es `SKILL.md` o el formato
instalado por el gestor de skills.

## Skills del proyecto

| Skill | Responsabilidad | Usar cuando |
|---|---|---|
| [`student-groups-navigator`](./student-groups-navigator/SKILL.md) | Mapa de capas, dominio, evidencia e invariantes reales | Antes de diseñar o tocar una feature |
| [`repo-standards`](./repo-standards/SKILL.md) | Arquitectura vigente, límites, estado, DTOs, datos, errores y calidad | En cualquier cambio de código o configuración |
| [`issue-to-change`](./issue-to-change/SKILL.md) | Issue → cambio proporcional → comprobaciones y evidencia | Al iniciar trabajo desde un issue |
| [`security-review`](./security-review/SKILL.md) | Autorización, privacidad, validación y secretos | En cambios de acceso, datos o entradas externas |
| [`documentation-maintainer`](./documentation-maintainer/SKILL.md) | Sincronización de documentación afectada | Cuando cambia comportamiento, arquitectura o contrato |
| [`graphify-maintainer`](./graphify-maintainer/SKILL.md) | Mantener el grafo mediante Graphify | Cuando cambia arquitectura, dependencias o roadmap |
| [`release-readiness`](./release-readiness/SKILL.md) | Gate de preparación de una versión candidata | Antes de desplegar una versión |
| [`graphify`](./graphify/SKILL.md) | Motor de knowledge graph, consultas y visualización | Para construir o consultar el grafo del repositorio |

Las skills externas de Supabase permanecen junto a estas en `supabase/` y
`supabase-postgres-best-practices/`. En este producto aplican a PostgreSQL y
Storage; Microsoft Entra ID sigue siendo el sistema de autenticación.

## Orden recomendado

1. `student-groups-navigator`
2. `repo-standards`
3. `issue-to-change` si el trabajo nace de un issue
4. `security-review` para cambios sensibles
5. `documentation-maintainer`
6. `graphify-maintainer` + `graphify` si cambia la estructura
7. `release-readiness` antes de un despliegue solicitado

Las skills son contratos de trabajo para agentes; no sustituyen CI, pruebas ni
revisión humana.
