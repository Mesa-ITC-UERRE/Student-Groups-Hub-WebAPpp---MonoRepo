---
name: security-review
description: 'Revisar límites de confianza, autenticación Entra, autorización por recurso, circuitos Blazor, API Bearer, validación, errores, privacidad, PostgreSQL, Supabase Storage, archivos, logs y despliegue de Student Groups Hub. Use when: tocar login/sesión, controllers, componentes interactivos, roles, grupos, membresías, eventos, admin, DTOs, formularios, uploads, datos, RLS/grants, configuración o integraciones. Triggers: security, seguridad, auth, autorización, privacidad, PII, secreto, validación, IDOR, BOLA, antiforgery, upload, Storage, RLS, error, log.'
---

# Security Review

Revisar el flujo completo desde la entrada hasta el dato o integración. Ocultar
controles en UI mejora UX, pero nunca concede autorización.

## Identidad, sesión y transporte

- Confirmar tenant, issuer, audience, firma, expiración y clock skew de Entra.
- Distinguir cookie/OpenID Connect de Blazor y JWT Bearer de REST; no aceptar el
  esquema equivocado ni asumir que una validación cubre ambas entradas.
- Revisar logout, return URLs locales, antiforgery en mutaciones, HTTPS/HSTS,
  CORS cuando aplique y confianza mínima de forwarded headers/proxies.
- En Blazor Server, tratar cada evento como entrada no confiable; no conservar
  decisiones de autorización únicamente en estado scoped o campos del componente.

## Autorización por recurso

Para cada lectura o mutación comprobar autenticación, usuario activo, rol,
existencia/estado del recurso y relación con `groupId`, `membershipId`, `eventId`
o `userId`. Un líder requiere `RoleAssignment` activa del grupo; solo admin tiene
alcance global explícito.

Probar al menos: estudiante, líder del grupo A, líder A contra recurso del grupo B,
admin, usuario inactivo, recurso inexistente, ID manipulado y repetición de la
transición. Revisar Blazor y controller REST cuando ambos expongan el flujo.

## Entrada, salida y privacidad

- Validar en la frontera formato/tamaño y en negocio estado, ownership,
  capacidad, fechas, duplicados y transiciones. Evitar mass assignment.
- Entidades EF no salen a UI/API. DTOs y modelos minimizan correo, nombre, notas,
  participantes, reportes y metadatos internos.
- No renderizar HTML no confiable con `MarkupString`; validar y restringir URLs,
  redirects, nombres de archivo, SVG y contenido activo.
- No registrar JWT, cookies, client secrets, service keys, passwords, connection
  strings, payloads PII ni excepciones que los contengan.

## Archivos y Supabase

- Validar límite antes de bufferizar, MIME declarado, extensión y firma/contenido;
  generar nombre/ruta en servidor y comprobar ownership, bucket y visibilidad.
- No usar URL aportada por cliente como autorización ni exponer `service_role`.
- Supabase en este producto significa PostgreSQL y Storage, no Supabase Auth.
  Para esquema, índices, RLS, grants, vistas o funciones leer las dos skills
  externas; no usar service role para omitir autorización del producto.

## Errores, disponibilidad y dependencias

- Separar fallo esperado de error inesperado. No devolver stack trace, SQL ni
  `Exception.Message` no clasificado al navegador/API.
- No usar fallback vacío para confirmar una escritura ni `catch {}` para ocultar
  errores. Las degradaciones de lectura deben ser visibles y registradas sin PII.
- Revisar timeouts, cancelación, retries idempotentes, límites y respuestas de
  Storage, PostgreSQL, Entra y correo.

## Salida de revisión

```text
[CRITICAL|HIGH|MEDIUM|LOW] título
Evidencia: archivo/símbolo y condición reproducible.
Impacto: dato o acción comprometida.
Corrección: cambio mínimo verificable.
Prueba: escenario negativo y positivo.
```

Emitir `NO-GO` ante secreto real expuesto, privilege escalation, BOLA/IDOR,
bypass de auth, escritura no autorizada o fuga de PII sin mitigación.
