# Resumen de cambios: Componente Restaurante + Fix Producto

## 1. Nuevo componente: Restaurante (CRUD completo)

### Delivery.Core
- `Delivery.Core/Entities/Business/Restaurante.cs` (ya existía, sin cambios)

### Delivery.Application
- `Delivery.Application/Dto/Response/RestauranteDto.cs` (nuevo) — Id, Nombre, EsActivo, EsEliminado
- `Delivery.Application/Mappers/RestauranteMapper.cs` (nuevo) — MapToEntity / UpdateEntity / MapToDto
- `Delivery.Application/Validators/Restaurante/RestauranteValidator.cs` (nuevo) — Crear, Actualizar, Eliminar
- Commands:
  - `Delivery.Application/Features/Commands/Business/Restaurantes/CreateRestauranteCommand.cs`
  - `Delivery.Application/Features/Commands/Business/Restaurantes/UpdateRestauranteCommand.cs`
  - `Delivery.Application/Features/Commands/Business/Restaurantes/DeleteRestauranteCommand.cs`
  - `Delivery.Application/Features/Commands/Business/Restaurantes/RestauranteCommandHandler.cs`
- Queries:
  - `Delivery.Application/Features/Queries/Business/Restaurantes/RestauranteListQuery.cs`
  - `Delivery.Application/Features/Queries/Business/Restaurantes/RestauranteByIdQuery.cs`
  - `Delivery.Application/Features/Queries/Business/Restaurantes/RestauranteQueryHandler.cs`

### Delivery.API
- `Delivery.API/Controllers/v1/RestauranteController.cs` (nuevo) — endpoints:
  - `POST api/v1/restaurante` → crea restaurante (devuelve Guid)
  - `GET api/v1/restaurante/{id:guid}` → obtiene por Id (404 si no existe)
  - `GET api/v1/restaurante` → lista todos
  - `PUT api/v1/restaurante/{id:guid}` → actualiza (204 NoContent)
  - `DELETE api/v1/restaurante/{id:guid}` → elimina (204 NoContent)

## 2. Nuevo componente: RestauranteDireccion (CRUD completo)

### Delivery.Application
- `Delivery.Application/Dto/Response/RestauranteDireccionDto.cs` (nuevo) — datos de dirección + RestauranteId + RestauranteNombre
- `Delivery.Application/Mappers/RestauranteDireccionMapper.cs` (nuevo)
- `Delivery.Application/Validators/RestauranteDireccion/RestauranteDireccionValidator.cs` (nuevo)
- Commands:
  - `Delivery.Application/Features/Commands/Business/RestauranteDirecciones/CreateRestauranteDireccionCommand.cs`
  - `Delivery.Application/Features/Commands/Business/RestauranteDirecciones/UpdateRestauranteDireccionCommand.cs`
  - `Delivery.Application/Features/Commands/Business/RestauranteDirecciones/DeleteRestauranteDireccionCommand.cs`
  - `Delivery.Application/Features/Commands/Business/RestauranteDirecciones/RestauranteDireccionCommandHandler.cs` — valida que el Restaurante exista (create y update)
- Queries:
  - `Delivery.Application/Features/Queries/Business/RestauranteDirecciones/RestauranteDireccionListQuery.cs` — filtro opcional por `RestauranteId`
  - `Delivery.Application/Features/Queries/Business/RestauranteDirecciones/RestauranteDireccionByIdQuery.cs`
  - `Delivery.Application/Features/Queries/Business/RestauranteDirecciones/RestauranteDireccionQueryHandler.cs` — incluye `Restaurante` para el nombre

### Delivery.API
- `Delivery.API/Controllers/v1/RestauranteDireccionController.cs` (nuevo) — endpoints:
  - `POST api/v1/restaurantedireccion` → crea dirección (devuelve Guid)
  - `GET api/v1/restaurantedireccion/{id:guid}` → obtiene por Id (404 si no existe)
  - `GET api/v1/restaurantedireccion?restauranteId={guid}` → lista todas o filtra por restaurante
  - `PUT api/v1/restaurantedireccion/{id:guid}` → actualiza (204)
  - `DELETE api/v1/restaurantedireccion/{id:guid}` → elimina (204)

## 3. Fix Producto (ya se puede crear)

El FK `Producto.RestauranteId` existía en BD, pero el command no lo pedía, por lo que se insertaba `Guid.Empty` y fallaba la FK.

Cambios:
- `Delivery.Application/Features/Commands/Business/Productos/CreateProductoCommand.cs` — agregado `RestauranteId` (Guid)
- `Delivery.Application/Features/Commands/Business/Productos/UpdateProductoCommand.cs` — agregado `RestauranteId`
- `Delivery.Application/Mappers/ProductoMapper.cs` — asigna `RestauranteId` en create/update; mapea `RestauranteId` y `RestauranteNombre` al DTO
- `Delivery.Application/Features/Commands/Business/Productos/ProductoCommandHandler.cs` — valida que exista el Restaurante antes de crear/actualizar (`Producto.RestauranteInvalido`)
- `Delivery.Application/Validators/Producto/ProductoValidator.cs` — `RestauranteId` obligatorio en crear y actualizar
- `Delivery.Application/Dto/Response/ProductoDto.cs` — agregado `RestauranteId` y `RestauranteNombre`
- `Delivery.Application/Features/Queries/Business/Productos/ProductoQueryHandler.cs` — incluye `Categoria` y `Restaurante` en las consultas por Id y listado

## 4. Fixes adicionales

- `Delivery.API/Controllers/v1/ProductoController.cs`:
  - PUT/DELETE ahora manejan `result.IsFailure` (antes siempre devolvían 204 aunque fallara)
  - `ProducesResponseType` del POST corregido de `int` a `Guid`
- `Delivery.Infrastructure/Implementation/RepositoryGeneric.cs`:
  - Bug en `GetAsync`: usaba `include.ToString()` en vez del `Expression<Func<T,object>>`, causaba error en los Include de navegación. Corregido a `query.Include(include)`.

## Nota
No se requiere nueva migración: el esquema ya contenía `FK_Producto_Restaurante_RestauranteId` en la migración `20260919072615_InitialCreate`. Solo faltaba la capa de aplicación/API.