---
name: graphify-maintainer
description: 'Mantener limpio, actual y verificable el grafo de arquitectura, dependencias, callers y roadmap de Student Groups Hub mediante Graphify. Use when: cambia código, módulos, límites entre Components/ApiServices/services/controllers/data, auth, persistencia, integraciones, configuración, despliegue, documentación estructural o exclusiones del corpus. Triggers: grafo, Graphify, graph, arquitectura, dependencias, caller, affected, roadmap, módulo, integración, manifiesto, corpus.'
---

# Graphify Maintainer

`graphify-out/` es el artefacto generado de navegación, no autoridad superior al
código. Confirmar relaciones inferidas antes de tomar decisiones.

## Corpus

- Incluir código, configuración versionable y documentos de producto/roadmap.
- Excluir bin/obj/vendor, outputs, skills y metadatos de colaboración,
  implementaciones de herramientas, configuración local, secretos, PII y tooling
  personal. Las reglas del agente no son módulos del producto.
- `.gitignore` y `.graphifyignore` forman parte del contrato. Un archivo ignorado
  no puede permanecer en `manifest.json`, aunque existiera en una ejecución previa.

## Procedimiento

1. Ejecutar `student-groups-navigator` y clasificar el cambio y sus callers.
2. Preparar el entorno una vez con `scripts/setup-graphify.sh`.
3. Usar `scripts/graphify.sh update .` para cambios estructurales ordinarios.
4. Usar `scripts/graphify.sh extract . --out .` cuando cambien exclusiones,
   corpus, manifiesto, versión de Graphify o existan entradas obsoletas.
5. Consultar `query`, `path`, `affected`, `explain` o `god-nodes` y comprobar en
   código cualquier resultado inferido o ambiguo.
6. Ejecutar `scripts/check-graphify.sh`; debe validar artefactos, hashes
   estructurales, consulta y ausencia de archivos ignorados/sensibles.
7. Revisar `GRAPH_REPORT.md`, comunidades, god nodes y conexiones sorprendentes.
   Eliminar ruido o relaciones locales antes de publicar.
8. Revisar diff de `graphify-out/` y actualizar roadmap/documentación solo cuando
   cambie conocimiento real, no por inferencia automática.

## Invariantes

- Runtime: `blazor/` .NET 10; React/API separada solo puede aparecer rotulada como
  arquitectura histórica.
- El grafo puede haberse generado sobre el commit base mientras incluye cambios
  del working tree; la frescura se decide por el manifiesto/hashes, no solo por el
  hash informativo del reporte.
- Auth, datos, despliegue y límites de capas activan `security-review` y
  `documentation-maintainer` cuando corresponda.
- No entregar outputs con secretos, rutas locales sensibles, tooling personal o
  archivos que Git considera ignorados.
