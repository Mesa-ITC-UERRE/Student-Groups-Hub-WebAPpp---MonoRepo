---
name: release-readiness
description: 'Evaluar con evidencia si una versión candidata de Student Groups Hub puede desplegarse sin romper build, comportamiento, autorización, datos, configuración, documentación u operación. Use when: preparar release, PR hacia main, tag, producción, Azure Container Apps, estabilización, despliegue o cierre de fase. Triggers: release, release-readiness, versión, producción, deploy, PR main, go/no-go, rollback.'
---

# Release Readiness

Emitir `GO`, `GO CON RIESGOS ACEPTADOS` o `NO-GO`. Seguir el flujo Git indicado
por el usuario sin imponer ramas, merge strategy o PRs adicionales.

## Alcance y evidencia

1. Comparar candidato/base y enumerar issues, commits, archivos, migraciones,
   configuración e integraciones afectadas. Aplicar `student-groups-navigator`.
2. Vincular criterios con evidencia y separar checks ejecutados, no disponibles,
   bloqueados y no aplicables.
3. Detectar cambios fuera de alcance, artefactos locales/ignorados o documentación
   que afirma comportamiento inexistente.

## Gates aplicables

- **Build:** `dotnet restore blazor/StudentGroupsHub.csproj` y
  `dotnet build blazor/StudentGroupsHub.csproj --configuration Release`.
- **Pruebas:** ejecutar proyectos disponibles y smoke de rutas/circuitos cambiados.
  Si no hay tests automatizados, declararlo como riesgo; no reportarlos como passed.
- **Seguridad:** ejecutar `security-review` para auth, roles, PII, entradas,
  Storage, datos, errores y configuración; bloquear hallazgos críticos/altos.
- **Datos:** revisar migraciones, snapshot, compatibilidad, constraints, operación
  transaccional, backup/rollback y cambio destructivo antes de aplicar.
- **Contratos/UI:** confirmar DTOs, mappers, controllers, fachadas, estados de UI,
  accesibilidad, workflows y casos afectados.
- **Documentación/grafo:** ejecutar skills correspondientes; Graphify debe estar
  estructuralmente actualizado y no contener archivos ignorados o sensibles.
- **Operación:** revisar `.github/workflows/deploy-main.yml`, OIDC/secrets,
  publish profile, Azure Container Apps, health check, observabilidad y rollback.

## Política

- `NO-GO`: build roto, criterio crítico fallido, secreto, bypass de auth,
  BOLA/IDOR, fuga de PII, migración insegura o rollback inexistente para cambio
  destructivo.
- `GO CON RIESGOS ACEPTADOS`: deuda no crítica con impacto, owner, mitigación y
  fecha/condición de seguimiento.
- `GO`: todos los gates aplicables tienen evidencia y no quedan bloqueantes.

Reportar veredicto, evidencia, fallos, riesgos y condición de desbloqueo. Crear
`docs/release-readiness/<version>.md` solo cuando el reporte deba persistir.
