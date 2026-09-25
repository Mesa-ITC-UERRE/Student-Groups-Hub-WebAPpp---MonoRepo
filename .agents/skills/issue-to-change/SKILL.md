---
name: issue-to-change
description: 'Convertir un issue de Student Groups Hub en un plan trazable, implementación completa, pruebas, documentación y evidencia. Use when: iniciar una feature, bug, mejora de seguridad, cambio de contrato, migración, tarea de UI o trabajo de roadmap desde un issue. Triggers: issue, ticket, bug, feature, plan, implementación, acceptance criteria, criterios de aceptación, PR.'
argument-hint: 'Issue, identificador y criterios de aceptación a convertir en cambio'
---

# Issue to Change

Un issue se entrega solo cuando comportamiento, callers, pruebas, documentos y
grafo están alineados.

## Flujo

1. Extraer problema, actor, comportamiento actual/esperado, alcance, exclusiones,
   criterios, restricciones y riesgos.
2. Crear `docs/plans/issue-<id>-<slug>.md` con problema, resultado observable,
   criterios verificables, mapa de impacto, seguridad, implementación,
   documentación, grafo, riesgos y evidencia.
3. Aplicar `student-groups-navigator` y `repo-standards`; localizar callers,
   estados, roles, DTOs, migraciones y UI.
4. Implementar verticalmente: datos → service → controller/API → UI, según aplique.
5. Convertir cada criterio en una comprobación observable; ejecutar tests, smoke y
   build. No declarar checks no ejecutados.
6. Activar `security-review`, `documentation-maintainer` y
   `graphify-maintainer` cuando corresponda.
7. Usar `release-readiness` antes de `main`.

## Definition of Done

- Todos los criterios tienen evidencia.
- Callers, DTOs, tests, documentos y grafo están actualizados o justificados.
- Autorización en servidor y PII protegida.
- Sin stubs, no-ops, mocks de producción ni TODOs engañosos.
- Build/smoke/test ejecutados y registrados.
- Changelog y Conventional Commit ajustados al repositorio.
