---
name: graphify-maintainer
description: 'Mantener el grafo de arquitectura, dependencias y roadmap de Student Groups Hub usando la skill Graphify instalada en .agents/skills/graphify. Use when: crear o mover módulos, cambiar dependencias, auth, persistencia, despliegue, workflows, integraciones, fases, roadmap o límites entre UI, services, controllers y datos. Triggers: grafo, Graphify, graph, arquitectura, dependencias, roadmap, módulo, integración, despliegue.'
argument-hint: 'Cambio estructural, dependencia o fase a reflejar con Graphify'
---

# Graphify Maintainer

## Fuente de verdad

El artefacto del grafo es `graphify-out/`, generado por la skill `graphify`.
Sus entregables principales son:

- `graphify-out/graph.json`: knowledge graph consultable;
- `graphify-out/graph.html`: visualización interactiva;
- `graphify-out/GRAPH_REPORT.md`: reporte legible;
- `graphify-out/graph.svg` cuando se exporta explícitamente.

No crear un Mermaid paralelo como fuente de verdad.

## Procedimiento

1. Ejecutar `student-groups-navigator` y clasificar el cambio.
2. Para reconstrucción completa usar:
   `python -m graphify extract . --out .`.
3. Para actualización incremental usar:
   `python -m graphify update .`.
4. Para consultar relaciones usar `python -m graphify query "..."`,
   `path`, `affected`, `explain` o `god-nodes`.
5. Verificar que Graphify respeta `.gitignore` y no ingiere secretos,
   `appsettings.Development.json`, binarios ni artefactos sensibles.
6. Revisar `GRAPH_REPORT.md` y `graph.json` antes de entregar.
7. Si cambia el roadmap, actualizar también `docs/11-development-plan.md`; el
   grafo muestra evidencia o plan, no debe inventar implementación.

## Invariantes

- El runtime es `blazor/` .NET 10; React/API separada es histórico.
- Dependencias deben derivar de código, configuración, workflow o contrato real.
- Auth, datos, despliegue y límites de capas requieren `security-review` y
  `documentation-maintainer`.
- No publicar Graphify outputs que contengan secretos, PII o rutas sensibles.
