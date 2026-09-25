---
name: security-review
description: 'Revisar autorización, privacidad, validación, autenticación, archivos, Supabase y datos sensibles de Student Groups Hub. Use when: tocar Entra ID, JWT, cookies, roles, líderes, membresías, grupos, eventos, Storage, PostgreSQL, DTOs, búsqueda, formularios, logs, errores o despliegue. Triggers: security, seguridad, auth, autorización, privacidad, PII, secreto, validación, IDOR, BOLA, upload, Storage, RLS.'
---

# Security Review

## Revisar

### Identidad y sesión

Verificar issuer, audience, firma, expiración y tenant de Entra; esquemas Cookie y
Bearer; logout, expiración, return URL, CORS, forwarded headers y antiforgery.
Nunca filtrar tokens en errores o logs.

### Autorización y BOLA/IDOR

Para cada endpoint sensible comprobar autenticación, usuario activo, rol,
recurso/estado válido, pertenencia de `groupId`, `membershipId`, `eventId` y
`userId`, `RoleAssignment` activa para el líder y alcance global exclusivo de
admin. Probar estudiante, líder de grupo A contra grupo B, usuario inactivo,
recurso inexistente y repetición de la transición.

### Validación y privacidad

Validar límites, formatos, estados, fechas, capacidad, ownership y duplicados.
Usar EF Core parametrizado. Minimizar email, nombres, notas, participantes y
reportes en respuestas. No registrar JWT, cookies, keys, connection strings ni
payloads PII.

### Storage

Validar tamaño, MIME, extensión, contenido, nombre, ruta, ownership y visibilidad.
Prevenir path traversal, SVG/script no permitido y URLs del cliente como permiso.

### Supabase

Leer primero `.agents/skills/supabase/SKILL.md` y
`supabase-postgres-best-practices` para RLS, grants, vistas, funciones, Storage y
migraciones. No usar service role para saltar autorización del producto.

## Hallazgo

```text
[CRITICAL|HIGH|MEDIUM|LOW] título
Evidencia: archivo/símbolo y condición reproducible.
Impacto: dato o acción comprometida.
Corrección: cambio mínimo verificable.
Prueba: escenario que debe pasar.
```

`NO-GO` para secreto expuesto, privilege escalation, BOLA/IDOR, bypass de auth o
fuga de PII sin mitigación.
