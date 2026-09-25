---
name: graphify
description: 'Consultar, actualizar y verificar el knowledge graph de Student Groups Hub para localizar símbolos, callers, dependencias, rutas de impacto, comunidades y archivos relacionados. Use when: explorar arquitectura antes de leer código, estimar impacto, regenerar graphify-out o diagnosticar un manifiesto desactualizado. Triggers: graphify, grafo, graph, query, path, affected, explain, god-nodes, caller, arquitectura, dependencias.'
---

# Graphify

Usar el grafo como índice. Código, migraciones y configuración ejecutable siguen
siendo la evidencia final; confirmar resultados `INFERRED` o incompletos.

## Operación

1. Ejecutar desde la raíz y preparar una vez con `scripts/setup-graphify.sh`.
2. Antes de exploración amplia, ejecutar `scripts/check-graphify.sh`. Si falla por
   frescura, actualizar antes de confiar en rutas de impacto.
3. Consultar con `scripts/graphify.sh query "pregunta"`, `path`, `affected`,
   `explain` o `god-nodes`; abrir después los archivos candidatos.
4. Usar `scripts/graphify.sh update .` tras cambios normales de código/docs.
5. Reconstruir con `scripts/graphify.sh extract . --out .` si cambia el corpus,
   `.gitignore`, `.graphifyignore`, la herramienta o hay entradas obsoletas.
6. Ejecutar de nuevo el check y revisar reporte, JSON, HTML y diff generado.

No ingerir secretos, configuración local, PII, archivos ignorados, dependencias
vendorizadas ni outputs. Para opciones no cubiertas, consultar `skill-codex.md`
solo cuando la tarea avanzada lo requiera.
