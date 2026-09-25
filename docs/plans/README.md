# Planes de issues

Los cambios estructurales o sensibles iniciados desde un issue usan
`issue-to-change` y guardan su plan durable como:

```text
docs/plans/issue-<id>-<slug>.md
```

Un issue pequeño puede servir como su propio plan. Persistir un plan cuando el
cambio abarque varias capas, implique decisiones durables o toque auth,
autorización, PII, migraciones, pérdida de datos, integraciones o despliegue.

Cada plan persistido debe contener problema, alcance, criterios verificables,
mapa de impacto, seguridad, implementación, pruebas, documentación, grafo,
riesgos, rollback cuando aplique y evidencia final. No guardar secretos, tokens
ni datos personales reales.
