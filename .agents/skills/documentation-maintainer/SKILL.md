---
name: documentation-maintainer
description: 'Mantener sincronizada la documentación de Student Groups Hub con cada cambio de código, contrato, seguridad, UI, datos, despliegue o roadmap. Use when: actualizar README, arquitectura, modelo de datos, API, roles, workflows, casos de prueba, diseño, changelog o planes. Triggers: documentación, docs, README, contrato, API, arquitectura, modelo de datos, workflow, changelog, roadmap, ADR.'
argument-hint: 'Cambio realizado y documentos potencialmente afectados'
---

# Documentation Maintainer

La documentación explica el comportamiento vigente. Si código y documento
contradicen, verificar el runtime, actualizar lo incluido en alcance y registrar
el desfase restante.

## Matriz de impacto

| Cambio | Documentos mínimos |
|---|---|
| Auth, roles, alcance por grupo | `01`, `02`, `05`, `08`, `09` |
| Entidades, estados, migraciones | `03`, `04`, workflows y casos |
| Endpoint, DTO o error | `04`, `08`, `09` |
| UI, navegación o estilos | `DESIGN.md`, `06`, `07`, workflows/casos |
| Notificaciones, correo o archivos | `03`, `04`, `05`, seguridad y casos |
| Roadmap o Definition of Done | `11`, `10` si cambia medición |
| CI/CD, release o commits | `12`, README, workflow y CHANGELOG |
| Skills/proceso | `docs/13-collaboration-skills.md`, `docs/README.md`, `.agents/skills/README.md` |
| Arquitectura/dependencias | `graphify-out/`, `docs/13-collaboration-skills.md` |

## Procedimiento

1. Revisar diff y clasificar impacto.
2. Leer documentos antes de editar y conservar numeración/tono.
3. Actualizar rutas, estados, permisos, ejemplos y criterios que cambiaron.
4. Mantener prosa en español e identificadores técnicos en inglés.
5. Verificar enlaces y ejemplos contra el código.
6. Registrar la transición histórica React/API separada → Blazor sin ocultarla.
7. Pasar cambios estructurales a `graphify-maintainer`.

No afirmar “implementado” sin evidencia ni inventar endpoints, tablas o fases.
