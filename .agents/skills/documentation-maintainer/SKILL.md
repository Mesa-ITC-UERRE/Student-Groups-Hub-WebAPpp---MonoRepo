---
name: documentation-maintainer
description: 'Mantener documentación útil y verificable de Student Groups Hub cuando cambia comportamiento, arquitectura, contratos, seguridad, UI, datos, operación, despliegue, roadmap o skills. Use when: actualizar README, docs, DESIGN, API, modelo de datos, roles, workflows, casos de prueba, changelog, planes o ADR; reconciliar documentación histórica React/API con el runtime Blazor. Triggers: documentación, docs, README, contrato, API, arquitectura, modelo de datos, workflow, changelog, roadmap, ADR, desactualizado.'
---

# Documentation Maintainer

Documentar conocimiento durable y comportamiento comprobado. No modificar todos
los documentos de una categoría si el cambio no afecta su contrato.

## Tipos de evidencia

- **Vigente:** código/configuración ejecutable, migraciones y checks observados.
- **Normativo:** reglas de negocio, seguridad, diseño y estándares acordados.
- **Planificado:** roadmap o contrato futuro; rotularlo sin afirmar implementación.
- **Histórico:** React/API separada u otra decisión reemplazada; mantenerla solo
  cuando explique una transición y marcarla como histórica.

Ante contradicción, comprobar runtime y alcance. Corregir el documento afectado o
registrar explícitamente la deuda; no propagar información vieja a otros archivos.

## Matriz de impacto selectiva

| Cambio observable | Revisar y actualizar si aplica |
|---|---|
| Auth, roles, autorización por grupo | overview, arquitectura, roles, workflows, casos |
| Entidad, estado, constraint o migración | modelo de datos, API, workflows, rollback |
| Endpoint, DTO o formato de error | contrato API, consumidores, casos |
| Componente, navegación o interacción | `DESIGN.md`, módulos, UI, casos accesibles |
| Fachada, servicio, estado o integración | arquitectura y mapa de colaboración |
| Notificaciones, correo o archivos | datos, API, privacidad, workflows y operación |
| CI/CD, configuración o release | README, versionado, workflow, changelog/runbook |
| Skills o proceso | docs de colaboración e índice de skills |
| Arquitectura/dependencias | referencia vigente, ADR si hay decisión y Graphify |

## Procedimiento

1. Revisar diff/issue y clasificar qué conocimiento cambió y quién lo consume.
2. Leer la sección antes de editar; conservar idioma, numeración y enlaces útiles.
3. Verificar rutas, estados, permisos, ejemplos y comandos contra el repositorio.
4. Actualizar el conjunto mínimo coherente y distinguir vigente, histórico y plan.
5. Crear ADR solo para una decisión arquitectónica con alternativas y consecuencias,
   no para cada refactor o bug.
6. Mantener prosa en español e identificadores técnicos en inglés.
7. Verificar enlaces locales y pasar cambios estructurales a Graphify.

No declarar “implementado”, “probado”, “seguro” o “desplegado” sin evidencia. No
copiar secretos, PII, tokens ni configuración local a ejemplos o artefactos.
