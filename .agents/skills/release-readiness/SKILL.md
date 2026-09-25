---
name: release-readiness
description: 'Validar que una rama release de Student Groups Hub puede promoverse a main sin romper comportamiento, autorización, datos, documentación ni despliegue. Use when: release candidate, pre-release, merge a main, tag, producción, Azure, estabilización o cierre de fase. Triggers: release, release-readiness, promoción, main, producción, deploy, go/no-go.'
argument-hint: 'Rama release, versión objetivo y commit/base de comparación'
---

# Release Readiness

Emitir `GO`, `GO CON RIESGOS ACEPTADOS` o `NO-GO` con evidencia, comandos,
fallos, severidad, owner y condición de desbloqueo.

## Branch strategy

La rama de integración previa a producción es `release`:

```text
feature/* → release → main
```

Los cambios se integran y validan en `release`; `main` solo recibe una
promoción aprobada por este gate. Configurar protección de ramas y requerir PR
si la política de GitHub lo permite; no depender únicamente de esta skill.

## Gates

1. Comparar rama/base, issue, alcance y criterios; ejecutar navigator.
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
