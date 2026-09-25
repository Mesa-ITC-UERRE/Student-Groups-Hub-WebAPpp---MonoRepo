---
name: release-readiness
description: 'Validar que una versión candidata de Student Groups Hub puede desplegarse sin romper comportamiento, autorización, datos, documentación ni operación. Use when: release candidate, pre-release, tag, producción, Azure, estabilización, despliegue o cierre de fase. Triggers: release, release-readiness, versión, producción, deploy, go/no-go.'
---

# Release Readiness

Emitir `GO`, `GO CON RIESGOS ACEPTADOS` o `NO-GO` con evidencia, comandos,
fallos, severidad, owner y condición de desbloqueo.

## Gates

1. Comparar la versión candidata contra su base, issue, alcance y criterios;
   ejecutar navigator. Seguir el flujo Git indicado por el usuario sin imponer
   una estrategia de ramas desde esta skill.
2. Ejecutar:
   - `dotnet restore blazor/StudentGroupsHub.csproj`
   - `dotnet build blazor/StudentGroupsHub.csproj --configuration Release`
   - tests disponibles y smoke de rutas cambiadas.
3. Aplicar `security-review`: bloquear auth bypass, BOLA/IDOR, fuga de PII,
   secretos, migración insegura o Storage sin ownership/validación.
4. Confirmar DTOs, controllers, workflows, TC-*, CHANGELOG, roadmap y salida de
   `documentation-maintainer`/`graphify-maintainer`.
5. Revisar `.github/workflows/deploy-main.yml`, secretos, OIDC, health check,
   destino real, rollback y perfil de publicación.

## Política

- `NO-GO`: fallo funcional, seguridad, build, migración, secreto o evidencia crítica.
- `GO CON RIESGOS ACEPTADOS`: riesgo no crítico con owner, fecha y mitigación.
- `GO`: todos los gates aplicables tienen evidencia.

Registrar el reporte como `docs/release-readiness/<version>.md` cuando se requiera.
Marcar siempre los hooks que aún apuntan a `frontend/`.
