---
name: issue-to-change
description: 'Convertir un issue de Student Groups Hub en un cambio proporcional, trazable y verificable desde diagnóstico hasta implementación, pruebas, documentación y evidencia. Use when: resolver un bug, feature, deuda, cambio UI, seguridad, contrato, datos, migración, integración o roadmap identificado por issue/ticket. Triggers: issue, ticket, bug, feature, fix, implementación, acceptance criteria, criterios de aceptación, plan, PR.'
---

# Issue to Change

Entregar el resultado observable del issue sin ampliar el alcance ni generar
ceremonia innecesaria. Seguir el flujo Git indicado por el usuario.

## Graduar el trabajo

- **Pequeño:** una capa, riesgo bajo y criterios claros. Usar el issue como plan;
  no crear `docs/plans/` salvo que aporte contexto durable.
- **Medio:** varias capas/callers o decisiones no triviales. Mantener un plan de
  trabajo durante la implementación y persistirlo si será útil para revisión.
- **Estructural/sensible:** auth, autorización, PII, migración, pérdida de datos,
  integración externa, despliegue o cambio arquitectónico. Crear
  `docs/plans/issue-<id>-<slug>.md` con riesgos, rollback y evidencia.

## Flujo

1. Extraer ID, problema, actor, comportamiento actual/esperado, alcance,
   exclusiones, criterios, restricciones, dependencias y riesgos.
2. Reproducir o reunir evidencia antes de cambiar código; marcar incertidumbres.
3. Aplicar `student-groups-navigator` y `repo-standards`; construir el mapa de
   impacto por las capas realmente afectadas, no una secuencia obligatoria.
4. Definir para cada criterio una comprobación observable y el nivel de plan.
5. Implementar el corte vertical mínimo. Mantener alineadas las dos entradas
   Blazor/REST si comparten el caso de uso; localizar callers antes de cambiar
   firma, estado, DTO, mapper o esquema.
6. Activar `security-review` para límites de confianza; actualizar documentación
   solo si cambia un contrato o conocimiento durable; regenerar Graphify si cambia
   código, estructura o dependencias relevantes.
7. Ejecutar build, pruebas disponibles y smoke dirigido. Registrar exactamente lo
   ejecutado, lo no disponible y cualquier riesgo residual.
8. Revisar el diff contra alcance y preparar commits coherentes que referencien el
   issue cuando el repositorio/usuario lo requiera.

## Definition of Done

- Criterios cubiertos con evidencia o excepción explícita.
- Callers, UI, fachadas, servicios, DTOs/modelos, datos y errores afectados están
  actualizados o justificados.
- Autorización se valida en servidor y no se expone PII ni secretos.
- No hay stubs, no-ops, mocks de producción, capturas silenciosas ni TODOs que
  aparenten completar el alcance.
- Build y checks aplicables se ejecutaron; ausencia de tests se declara como riesgo.
- Documentación, migraciones, changelog y grafo están actualizados solo cuando el
  cambio lo exige.

Usar `release-readiness` para evaluar una versión candidata, no como sustituto de
la verificación de cada issue.
