---
name: graphify
description: 'Consultar, actualizar y verificar el knowledge graph de Student Groups Hub. Use when: responder preguntas sobre arquitectura, dependencias, callers, rutas de impacto, comunidades o relaciones del repositorio; regenerar graphify-out después de cambios; explicar nodos o encontrar caminos. Triggers: graphify, grafo, graph, query, path, affected, explain, god-nodes, arquitectura, dependencias.'
---

# Graphify

Usar primero el grafo existente para preguntas sobre el repositorio y leer código
directamente cuando el resultado sea insuficiente o necesite confirmación.

## Procedimiento

1. Ejecutar los comandos desde la raíz del repositorio.
2. Preparar el entorno una vez con `scripts/setup-graphify.sh`.
3. Consultar con `scripts/graphify.sh query "pregunta"`, `path`, `affected`,
   `explain` o `god-nodes`.
4. Actualizar código modificado con `scripts/graphify.sh update .`.
5. Reconstruir cuando cambie el corpus o el manifiesto con
   `scripts/graphify.sh extract . --out .` y las opciones apropiadas.
6. Ejecutar `scripts/check-graphify.sh` y revisar `graphify-out/GRAPH_REPORT.md`.
7. No ingerir secretos, configuración local, PII ni artefactos generados.

Para opciones avanzadas de extracción o exportación, consultar
`skill-codex.md` únicamente cuando la tarea las requiera.
