---
name: repo-standards
description: 'Aplicar los estándares técnicos y la arquitectura vigente de Student Groups Hub al escribir, revisar o refactorizar C#, Razor, EF Core, controllers, servicios, DTOs, estado, persistencia, integraciones, configuración, pruebas o documentación. Use when: cualquier cambio de producto o estructura en blazor/, creación de componentes, casos de uso, endpoints, migraciones, manejo de errores o revisión de calidad. Triggers: estándares, convenciones, arquitectura, estructura, estilo, calidad, review, C#, Razor, Blazor, EF Core, DTO, mapper, estado, servicio, controller, persistencia, error, pruebas.'
---

# Repository Standards

Evolucionar el monolito modular Blazor existente. No imponer Clean Architecture,
repositorios, managers, interfaces o proyectos adicionales sin una necesidad
demostrable del cambio.

Leer [references/current-architecture.md](references/current-architecture.md)
antes de crear una feature, mover responsabilidades o revisar límites de capas.

## Límites de responsabilidad

- **Presentación — `Components/`:** renderizar, capturar entrada y mantener estado
  visual local. No acceder a EF Core, secretos, Storage ni autorización final.
- **Aplicación — `*ApiService`, `CurrentUserService`:** orquestar casos de uso para
  Blazor, comprobar actor y recurso, coordinar servicios y mapear a `AppModels`.
  Son fachadas internas; no realizan HTTP.
- **HTTP — `Controllers/`, `DTOs/`:** autenticar esquema, binding, validación de
  borde, autorización y traducción HTTP. Delegar reglas y no devolver entidades.
- **Negocio y persistencia — servicios de feature:** proteger invariantes y
  transiciones; usar `IDbContextFactory<AppDbContext>` por operación. EF Core ya
  cumple repository/unit-of-work; no envolver CRUD sin valor adicional.
- **Infraestructura — `Data/`, `StorageService`, middleware e integraciones:**
  aislar PostgreSQL, Supabase Storage, Entra, correo, red y configuración.
- **Estado compartido — servicios scoped como `UserStateService`:** conservar
  solo estado de circuito pequeño y notificable; nunca `DbContext`, secretos o
  estado global de autorización.

## Reglas de construcción

1. Extender la feature y el flujo existentes antes de crear otra abstracción.
2. Crear componentes compartidos para UI repetida o páginas difíciles de revisar;
   no seguir creciendo archivos monolíticos al tocar una sección aislable.
3. Crear nuevas fachadas `*ApiService` en archivos por feature; no ampliar
   indefinidamente `Services/ApiServices.cs`.
4. Mantener reglas de negocio fuera de Razor y controllers. Revalidar permisos en
   servidor aunque la UI o ruta esté protegida.
5. No agregar una segunda SPA, backend, mecanismo de estado o contenedor DI.
6. Usar interfaces solo en fronteras externas, múltiples implementaciones o
   pruebas donde sustitución aporte valor; no por cada clase.

## Modelos, DTOs y mappers

- `Models/Entities.cs` representa persistencia; no cruzar entidades EF hacia UI o
  respuestas HTTP.
- `DTOs/Requests` y `DTOs/Responses` son el contrato REST y minimizan PII.
- `Models/AppModels.cs` contiene modelos/requests de presentación Blazor.
- Mapear en la frontera consumidora: `ToResponse` para HTTP y `Map*` en la fachada
  Blazor. Extraer un mapper dedicado solo cuando exista duplicación real.
- No reutilizar un tipo solo porque sus campos coinciden si mezcla contrato HTTP,
  formulario, persistencia o semántica de dominio distinta.

## Datos, estado y concurrencia

- Crear un contexto por operación; no conservarlo en componentes ni servicios de
  circuito. Usar async/await, consultas proyectadas, paginación y lotes; evitar N+1.
- Usar transacción para cambios multi-entidad que deban ser atómicos y constraint
  de base de datos para invariantes de unicidad/concurrencia cuando corresponda.
- Estado efímero de pantalla vive en el componente. Estado entre componentes del
  mismo circuito vive en un servicio scoped con alta/baja segura de eventos.
- No usar estado de UI como fuente de autorización o verdad persistente.

## Errores y observabilidad

- Distinguir validación, conflicto de negocio, no encontrado, autorización,
  dependencia externa y error inesperado; no usar un único mensaje/excepción para
  todos los casos nuevos.
- No agregar `catch {}`, no ocultar fallos de escritura y no mostrar excepciones
  inesperadas al usuario. Registrar contexto seguro sin tokens ni PII.
- Limitar `DbSafe` a lecturas degradables explícitas; una mutación debe confirmar
  éxito o devolver un fallo observable.
- Si se introduce `Result` o una excepción propia, aplicarla a un flujo completo y
  actualizar middleware, UI y pruebas; no crear dos contratos paralelos a medias.
- REST mantiene un formato consistente y Blazor muestra mensajes accionables y
  seguros con estados loading, vacío, error y éxito.

## Estilo, seguridad y entrega

- Nullable e implicit usings; PascalCase para tipos/métodos y camelCase para
  parámetros/campos privados. UI en español e identificadores técnicos en inglés.
- Seguir `DESIGN.md`: semántica HTML, labels, foco visible, teclado, reflow y
  estados accesibles. Cancelar/debounciar búsquedas y evitar dobles envíos.
- Entra autentica; el servidor autoriza por usuario activo, rol, recurso y grupo.
  Credenciales solo en configuración de servidor; uploads validan contenido,
  tamaño, tipo, nombre, destino, ownership y visibilidad.
- Aplicar las skills Supabase solo a PostgreSQL/Storage; Supabase Auth no forma
  parte de la arquitectura vigente.
- Para cambios no triviales declarar issue, alcance, invariantes, checks y riesgos.
  Usar plan persistente solo según la escala definida por `issue-to-change`.

## Verificación mínima

1. Ejecutar `dotnet restore` si cambian dependencias o falta `assets`.
2. Ejecutar `dotnet build blazor/StudentGroupsHub.csproj --configuration Release`.
3. Ejecutar pruebas existentes y smoke del flujo afectado; si no existen pruebas
   automatizadas, declararlo como riesgo, no como check aprobado.
4. Revisar warnings, secretos, autorización, migraciones, documentación y grafo
   según el alcance. No afirmar una comprobación no ejecutada.
