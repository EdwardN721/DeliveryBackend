# CRUD Schema Identity

Implementación del CRUD de las tres entidades del schema `Identity` (`Rol`, `Usuario`, `UsuarioDireccion`), siguiendo el mismo patrón que `Catalog` y `Business`: Commands + Queries por entidad, `Mapper`, `Validator` y `Controller` en `Controllers/v1`.

## Decisiones de diseño

- **Password con BCrypt**: se agrega la abstracción `IPasswordHasher` en `Delivery.Core` y su implementación con `BCrypt.Net-Next` en `Delivery.Infrastructure`. El hash se calcula en el command handler y se inyecta al mapper, porque los mappers son `static` y no tienen acceso a DI. El hash nunca se expone en los DTO.
- **RolIds en el CRUD de Usuario**: `Create` y `Update` reciben `RolIds`; el handler carga los `Rol` y llama `SincronizarRoles`, que sincroniza la tabla `UsuarioRoles`.
- **Soft delete en Usuario**: `UsuarioDireccion.UsuarioId` está configurado con `DeleteBehavior.Restrict`, por lo que `DELETE` marca `EsEliminado = true` y `EsActivo = false` en vez de borrar la fila. Las queries filtran por `!EsEliminado`.
- **Delete de Rol bloqueado si tiene usuarios**: `Rol` no extiende `BaseEntity` (no tiene `EsActivo`/`EsEliminado`), así que no admite soft delete. El handler valida que no tenga usuarios asignados y devuelve `Rol.ConUsuarios`.
- **Longitudes de validación**: tomadas de la migración `20260919072615_InitialCreate`, no de las entidades. `UsuarioDirecciones.Calle` es `varchar(20)` y `CodigoPostal` es `varchar(5)` (a diferencia de `RestauranteDireccion`, que usa 100 y 10).

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| POST | `api/v1/rol` | Crea rol (devuelve `int`) |
| GET | `api/v1/rol/{id:int}` | Rol por Id |
| GET | `api/v1/rol` | Lista roles |
| PUT | `api/v1/rol/{id:int}` | Actualiza rol |
| DELETE | `api/v1/rol/{id:int}` | Elimina rol (400 si tiene usuarios) |
| POST | `api/v1/usuario` | Crea usuario (devuelve `Guid`) |
| GET | `api/v1/usuario/{id:guid}` | Usuario por Id |
| GET | `api/v1/usuario?coreo=&soloActivos=` | Lista usuarios con filtros |
| PUT | `api/v1/usuario/{id:guid}` | Actualiza usuario |
| DELETE | `api/v1/usuario/{id:guid}` | Soft delete de usuario |
| POST | `api/v1/usuariodireccion` | Crea dirección (devuelve `Guid`) |
| GET | `api/v1/usuariodireccion/{id:guid}` | Dirección por Id |
| GET | `api/v1/usuariodireccion?usuarioId=` | Lista direcciones |
| PUT | `api/v1/usuariodireccion/{id:guid}` | Actualiza dirección |
| DELETE | `api/v1/usuariodireccion/{id:guid}` | Elimina dirección |

## Archivos

### `Delivery.Core/Interfaces/IPasswordHasher.cs`

```c#
namespace Delivery.Core.Interfaces;

/// <summary>
/// Contrato para transformar contraseñas en texto plano a hashes de un solo sentido y viceversa.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Genera un hash seguro para la contraseña recibida.
    /// </summary>
    /// <param name="password">Contraseña en texto plano.</param>
    /// <returns>Contraseña cifrada, apta para almacenarse.</returns>
    string Hash(string password);

    /// <summary>
    /// Verifica si una contraseña en texto plano corresponde a un hash previamente generado.
    /// </summary>
    /// <param name="password">Contraseña en texto plano a comprobar.</param>
    /// <param name="hash">Hash almacenado con el que se comparará.</param>
    /// <returns>Regresa true si la contraseña coincide con el hash, false en caso contrario.</returns>
    bool Verify(string password, string hash);
}
```

### `Delivery.Infrastructure/Implementation/PasswordHasher.cs`

```c#
using Delivery.Core.Interfaces;
using CoreBCrypt = BCrypt.Net.BCrypt;

namespace Delivery.Infrastructure.Implementation;

/// <summary>
/// Implementación de <see cref="IPasswordHasher"/> basada en el algoritmo BCrypt.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return CoreBCrypt.HashPassword(password);
    }

    public bool Verify(string password, string hash)
    {
        return CoreBCrypt.Verify(password, hash);
    }
}
```

### `Delivery.Application/Dto/Response/RolDto.cs`

```c#
namespace Delivery.Application.Dto.Response;

public record RolDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
}
```

### `Delivery.Application/Dto/Response/UsuarioDto.cs`

```c#
namespace Delivery.Application.Dto.Response;

public record UsuarioDto
{
    public Guid Id { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string PrimerApellido { get; init; } = string.Empty;
    public string? SegundoApellido { get; init; }
    public string NombreCompleto { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
    public IReadOnlyList<RolDto> Roles { get; init; } = [];
    public bool EsActivo { get; init; }
    public bool EsEliminado { get; init; }
}
```

### `Delivery.Application/Dto/Response/UsuarioDireccionDto.cs`

```c#
namespace Delivery.Application.Dto.Response;

public record UsuarioDireccionDto
{
    public Guid Id { get; init; }
    public string Calle { get; init; } = string.Empty;
    public string Numero { get; init; } = string.Empty;
    public string Colonia { get; init; } = string.Empty;
    public string Ciudad { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public string CodigoPostal { get; init; } = string.Empty;
    public Guid UsuarioId { get; init; }
    public string? UsuarioNombre { get; init; }
    public bool EsActivo { get; init; }
}
```

### `Delivery.Application/Mappers/RolMapper.cs`

```c#
using Delivery.Core.Entities.Identity;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Commands.Identity.Roles;

namespace Delivery.Application.Mappers;

public static class RolMapper
{
    public static Rol MapToEntity(this CreateRolCommand command)
    {
        return new Rol
        {
            Nombre = command.Nombre
        };
    }

    public static void UpdateEntity(this Rol rol, UpdateRolCommand updateCommand)
    {
        rol.Nombre = !string.IsNullOrWhiteSpace(updateCommand.Nombre) ? updateCommand.Nombre : rol.Nombre;
    }

    public static RolDto MapToDto(this Rol rol)
    {
        return new RolDto
        {
            Id = rol.Id,
            Nombre = rol.Nombre
        };
    }

    public static IEnumerable<RolDto> MapToDto(this IEnumerable<Rol>? roles)
    {
        return roles?.Select(MapToDto) ?? [];
    }
}
```

### `Delivery.Application/Mappers/UsuarioMapper.cs`

```c#
using Delivery.Core.Entities.Identity;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Commands.Identity.Usuarios;

namespace Delivery.Application.Mappers;

public static class UsuarioMapper
{
    public static Usuario MapToEntity(this CreateUsuarioCommand command, string passwordHash)
    {
        return new Usuario
        {
            Nombres = command.Nombres,
            PrimerApellido = command.PrimerApellido,
            SegundoApellido = command.SegundoApellido,
            Telefono = command.Telefono,
            Correo = command.Correo,
            Password = passwordHash
        };
    }

    public static void UpdateEntity(this Usuario usuario, UpdateUsuarioCommand updateCommand, string? passwordHash)
    {
        usuario.Nombres = !string.IsNullOrWhiteSpace(updateCommand.Nombres) ? updateCommand.Nombres : usuario.Nombres;
        usuario.PrimerApellido = !string.IsNullOrWhiteSpace(updateCommand.PrimerApellido) ? updateCommand.PrimerApellido : usuario.PrimerApellido;
        usuario.SegundoApellido = updateCommand.SegundoApellido ?? usuario.SegundoApellido;
        usuario.Telefono = !string.IsNullOrWhiteSpace(updateCommand.Telefono) ? updateCommand.Telefono : usuario.Telefono;
        usuario.Correo = !string.IsNullOrWhiteSpace(updateCommand.Correo) ? updateCommand.Correo : usuario.Correo;

        if (!string.IsNullOrWhiteSpace(passwordHash))
        {
            usuario.Password = passwordHash;
        }
    }

    /// <summary>
    /// Reemplaza el conjunto de roles del usuario por el recibido, sincronizando la tabla UsuarioRoles.
    /// </summary>
    public static void SincronizarRoles(this Usuario usuario, IEnumerable<Rol> roles)
    {
        usuario.Roles.Clear();

        foreach (Rol rol in roles)
        {
            usuario.Roles.Add(rol);
        }
    }

    public static UsuarioDto MapToDto(this Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            Nombres = usuario.Nombres,
            PrimerApellido = usuario.PrimerApellido,
            SegundoApellido = usuario.SegundoApellido,
            NombreCompleto = ConstruirNombreCompleto(usuario),
            Telefono = usuario.Telefono,
            Correo = usuario.Correo,
            Roles = usuario.Roles.MapToDto().ToList(),
            EsActivo = usuario.EsActivo,
            EsEliminado = usuario.EsEliminado
        };
    }

    public static IEnumerable<UsuarioDto> MapToDto(this IEnumerable<Usuario>? usuarios)
    {
        return usuarios?.Select(MapToDto) ?? [];
    }

    private static string ConstruirNombreCompleto(Usuario usuario)
    {
        return string.Join(" ",
            new[] { usuario.Nombres, usuario.PrimerApellido, usuario.SegundoApellido }
                .Where(parte => !string.IsNullOrWhiteSpace(parte)));
    }
}
```

### `Delivery.Application/Mappers/UsuarioDireccionMapper.cs`

```c#
using Delivery.Core.Entities.Identity;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Commands.Identity.UsuarioDirecciones;

namespace Delivery.Application.Mappers;

public static class UsuarioDireccionMapper
{
    public static UsuarioDireccion MapToEntity(this CreateUsuarioDireccionCommand command)
    {
        return new UsuarioDireccion
        {
            Calle = command.Calle,
            Numero = command.Numero,
            Colonia = command.Colonia,
            Ciudad = command.Ciudad,
            Estado = command.Estado,
            CodigoPostal = command.CodigoPostal,
            UsuarioId = command.UsuarioId
        };
    }

    public static void UpdateEntity(this UsuarioDireccion direccion, UpdateUsuarioDireccionCommand updateCommand)
    {
        direccion.Calle = !string.IsNullOrWhiteSpace(updateCommand.Calle) ? updateCommand.Calle : direccion.Calle;
        direccion.Numero = !string.IsNullOrWhiteSpace(updateCommand.Numero) ? updateCommand.Numero : direccion.Numero;
        direccion.Colonia = !string.IsNullOrWhiteSpace(updateCommand.Colonia) ? updateCommand.Colonia : direccion.Colonia;
        direccion.Ciudad = !string.IsNullOrWhiteSpace(updateCommand.Ciudad) ? updateCommand.Ciudad : direccion.Ciudad;
        direccion.Estado = !string.IsNullOrWhiteSpace(updateCommand.Estado) ? updateCommand.Estado : direccion.Estado;
        direccion.CodigoPostal = !string.IsNullOrWhiteSpace(updateCommand.CodigoPostal) ? updateCommand.CodigoPostal : direccion.CodigoPostal;
        direccion.UsuarioId = updateCommand.UsuarioId != direccion.UsuarioId ? updateCommand.UsuarioId : direccion.UsuarioId;
    }

    public static UsuarioDireccionDto MapToDto(this UsuarioDireccion direccion)
    {
        return new UsuarioDireccionDto
        {
            Id = direccion.Id,
            Calle = direccion.Calle,
            Numero = direccion.Numero,
            Colonia = direccion.Colonia,
            Ciudad = direccion.Ciudad,
            Estado = direccion.Estado,
            CodigoPostal = direccion.CodigoPostal,
            UsuarioId = direccion.UsuarioId,
            UsuarioNombre = direccion.Usuario is null
                ? null
                : string.Join(" ",
                    new[] { direccion.Usuario.Nombres, direccion.Usuario.PrimerApellido, direccion.Usuario.SegundoApellido }
                        .Where(parte => !string.IsNullOrWhiteSpace(parte))),
            EsActivo = direccion.EsActivo
        };
    }

    public static IEnumerable<UsuarioDireccionDto> MapToDto(this IEnumerable<UsuarioDireccion>? direcciones)
    {
        return direcciones?.Select(MapToDto) ?? [];
    }
}
```

### `Delivery.Application/Validators/Rol/RolValidator.cs`

```c#
using FluentValidation;
using Delivery.Application.Features.Commands.Identity.Roles;

namespace Delivery.Application.Validators.Rol;

public class CrearRolValidator : AbstractValidator<CreateRolCommand>
{
    public CrearRolValidator()
    {
        RuleFor(r => r.Nombre)
            .NotEmpty().WithMessage("El nombre del rol es obligatorio.")
            .MaximumLength(50).WithMessage("El tamaño máximo del nombre del rol es de 50 caracteres.");
    }
}

public class ActualizarRolValidator : AbstractValidator<UpdateRolCommand>
{
    public ActualizarRolValidator()
    {
        RuleFor(r => r.Id)
            .GreaterThan(0).WithMessage("El Id del rol debe ser mayor a 0.");

        RuleFor(r => r.Nombre)
            .MaximumLength(50).WithMessage("El tamaño máximo del nombre del rol es de 50 caracteres.");
    }
}

public class EliminarRolValidator : AbstractValidator<DeleteRolCommand>
{
    public EliminarRolValidator()
    {
        RuleFor(r => r.Id)
            .GreaterThan(0).WithMessage("El Id del rol debe ser mayor a 0.");
    }
}
```

### `Delivery.Application/Validators/Usuario/UsuarioValidator.cs`

```c#
using FluentValidation;
using Delivery.Application.Features.Commands.Identity.Usuarios;

namespace Delivery.Application.Validators.Usuario;

public class CrearUsuarioValidator : AbstractValidator<CreateUsuarioCommand>
{
    public CrearUsuarioValidator()
    {
        RuleFor(u => u.Nombres)
            .NotEmpty().WithMessage("Los nombres son obligatorios.")
            .MaximumLength(100).WithMessage("El tamaño máximo de los nombres es de 100 caracteres.");

        RuleFor(u => u.PrimerApellido)
            .NotEmpty().WithMessage("El primer apellido es obligatorio.")
            .MaximumLength(50).WithMessage("El tamaño máximo del primer apellido es de 50 caracteres.");

        RuleFor(u => u.SegundoApellido)
            .MaximumLength(50).WithMessage("El tamaño máximo del segundo apellido es de 50 caracteres.");

        RuleFor(u => u.Telefono)
            .NotEmpty().WithMessage("El teléfono es obligatorio.")
            .Matches(@"^\d{10}$").WithMessage("El teléfono debe tener 10 dígitos.");

        RuleFor(u => u.Correo)
            .NotEmpty().WithMessage("El correo es obligatorio.")
            .EmailAddress().WithMessage("El correo no tiene un formato válido.")
            .MaximumLength(300).WithMessage("El tamaño máximo del correo es de 300 caracteres.");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");

        RuleFor(u => u.RolIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("No se deben enviar Ids de rol repetidos.")
            .When(u => u.RolIds is not null);
    }
}

public class ActualizarUsuarioValidator : AbstractValidator<UpdateUsuarioCommand>
{
    public ActualizarUsuarioValidator()
    {
        RuleFor(u => u.Id)
            .NotEmpty().WithMessage("El Id no debe estar vacío.");

        RuleFor(u => u.Nombres)
            .MaximumLength(100).WithMessage("El tamaño máximo de los nombres es de 100 caracteres.");

        RuleFor(u => u.PrimerApellido)
            .MaximumLength(50).WithMessage("El tamaño máximo del primer apellido es de 50 caracteres.");

        RuleFor(u => u.SegundoApellido)
            .MaximumLength(50).WithMessage("El tamaño máximo del segundo apellido es de 50 caracteres.");

        RuleFor(u => u.Telefono)
            .Matches(@"^\d{10}$").WithMessage("El teléfono debe tener 10 dígitos.");

        RuleFor(u => u.Correo)
            .EmailAddress().WithMessage("El correo no tiene un formato válido.")
            .MaximumLength(300).WithMessage("El tamaño máximo del correo es de 300 caracteres.");

        RuleFor(u => u.Password)
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");

        RuleFor(u => u.RolIds)
            .Must(ids => ids is not null && ids.Distinct().Count() == ids.Count)
            .WithMessage("No se deben enviar Ids de rol repetidos.")
            .When(u => u.RolIds is not null);
    }
}

public class EliminarUsuarioValidator : AbstractValidator<DeleteUsuarioCommand>
{
    public EliminarUsuarioValidator()
    {
        RuleFor(u => u.Id)
            .NotEmpty().WithMessage("El Id no debe estar vacío.");
    }
}
```

### `Delivery.Application/Validators/UsuarioDireccion/UsuarioDireccionValidator.cs`

```c#
using FluentValidation;
using Delivery.Application.Features.Commands.Identity.UsuarioDirecciones;

namespace Delivery.Application.Validators.UsuarioDireccion;

public class CrearUsuarioDireccionValidator : AbstractValidator<CreateUsuarioDireccionCommand>
{
    public CrearUsuarioDireccionValidator()
    {
        RuleFor(ud => ud.Calle)
            .NotEmpty().WithMessage("La calle es obligatoria.")
            .MaximumLength(20).WithMessage("El tamaño máximo de la calle es de 20 caracteres.");

        RuleFor(ud => ud.Numero)
            .NotEmpty().WithMessage("El número es obligatorio.")
            .MaximumLength(10).WithMessage("El tamaño máximo del número es de 10 caracteres.");

        RuleFor(ud => ud.Colonia)
            .NotEmpty().WithMessage("La colonia es obligatoria.")
            .MaximumLength(50).WithMessage("El tamaño máximo de la colonia es de 50 caracteres.");

        RuleFor(ud => ud.Ciudad)
            .NotEmpty().WithMessage("La ciudad es obligatoria.")
            .MaximumLength(50).WithMessage("El tamaño máximo de la ciudad es de 50 caracteres.");

        RuleFor(ud => ud.Estado)
            .NotEmpty().WithMessage("El estado es obligatorio.");

        RuleFor(ud => ud.CodigoPostal)
            .NotEmpty().WithMessage("El código postal es obligatorio.")
            .MaximumLength(5).WithMessage("El tamaño máximo del código postal es de 5 caracteres.");

        RuleFor(ud => ud.UsuarioId)
            .NotEmpty().WithMessage("Debe asignar un usuario válido.");
    }
}

public class ActualizarUsuarioDireccionValidator : AbstractValidator<UpdateUsuarioDireccionCommand>
{
    public ActualizarUsuarioDireccionValidator()
    {
        RuleFor(ud => ud.Id)
            .NotEmpty().WithMessage("El Id no debe estar vacío.");

        RuleFor(ud => ud.Calle)
            .MaximumLength(20).WithMessage("El tamaño máximo de la calle es de 20 caracteres.");

        RuleFor(ud => ud.Numero)
            .MaximumLength(10).WithMessage("El tamaño máximo del número es de 10 caracteres.");

        RuleFor(ud => ud.Colonia)
            .MaximumLength(50).WithMessage("El tamaño máximo de la colonia es de 50 caracteres.");

        RuleFor(ud => ud.Ciudad)
            .MaximumLength(50).WithMessage("El tamaño máximo de la ciudad es de 50 caracteres.");

        RuleFor(ud => ud.Estado)
            .NotEmpty().WithMessage("El estado es obligatorio.");

        RuleFor(ud => ud.CodigoPostal)
            .MaximumLength(5).WithMessage("El tamaño máximo del código postal es de 5 caracteres.");

        RuleFor(ud => ud.UsuarioId)
            .NotEmpty().WithMessage("Debe asignar un usuario válido.");
    }
}

public class EliminarUsuarioDireccionValidator : AbstractValidator<DeleteUsuarioDireccionCommand>
{
    public EliminarUsuarioDireccionValidator()
    {
        RuleFor(ud => ud.Id)
            .NotEmpty().WithMessage("El Id no debe estar vacío.");
    }
}
```

### `Delivery.Application/Features/Commands/Identity/Roles/CreateRolCommand.cs`

```c#
using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.Roles;

public record CreateRolCommand : IRequest<Result<int>>
{
    public string Nombre { get; init; } = string.Empty;
}
```

### `Delivery.Application/Features/Commands/Identity/Roles/UpdateRolCommand.cs`

```c#
using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.Roles;

public record UpdateRolCommand : IRequest<Result>
{
    public int Id { get; init; }
    public string? Nombre { get; init; }
}
```

### `Delivery.Application/Features/Commands/Identity/Roles/DeleteRolCommand.cs`

```c#
using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.Roles;

public record DeleteRolCommand : IRequest<Result>
{
    public int Id { get; init; }
}
```

### `Delivery.Application/Features/Commands/Identity/Roles/RolCommandHandler.cs`

```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Identity;

namespace Delivery.Application.Features.Commands.Identity.Roles;

public class RolCommandHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<CreateRolCommand, Result<int>>,
    IRequestHandler<UpdateRolCommand, Result>,
    IRequestHandler<DeleteRolCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<int>> Handle(CreateRolCommand request, CancellationToken cancellationToken = default)
    {
        Result validacionNombre = await ValidarNombreDisponible(request.Nombre, null, cancellationToken);

        if (validacionNombre.IsFailure)
        {
            return Result<int>.Failure(validacionNombre.Error);
        }

        Rol rol = request.MapToEntity();

        await _unitOfWork.Roles.AddAsync(rol, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<int>.Success(rol.Id);
    }

    public async Task<Result> Handle(UpdateRolCommand request, CancellationToken cancellationToken = default)
    {
        Rol? rol = await BuscarRolPorId(request.Id, cancellationToken);

        if (rol == null)
        {
            return Result.Failure(new ErrorResult("Rol.NotFound", $"No se encontró el rol con el Id: {request.Id}"));
        }

        Result validacionNombre = await ValidarNombreDisponible(request.Nombre, request.Id, cancellationToken);

        if (validacionNombre.IsFailure)
        {
            return validacionNombre;
        }

        rol.UpdateEntity(request);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Handle(DeleteRolCommand request, CancellationToken cancellationToken = default)
    {
        Rol? rol = await BuscarRolPorId(request.Id, cancellationToken);

        if (rol == null)
        {
            return Result.Failure(new ErrorResult("Rol.NotFound", $"No se encontró el rol con el Id: {request.Id}"));
        }

        Result validacionUsuarios = await ValidarRolSinUsuarios(request.Id, cancellationToken);

        if (validacionUsuarios.IsFailure)
        {
            return validacionUsuarios;
        }

        _unitOfWork.Roles.Delete(rol);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    #region MetodosPrivados

    private async Task<Rol?> BuscarRolPorId(int id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Roles.GetByIdAsync(id, false, cancellationToken);
    }

    private async Task<Result> ValidarNombreDisponible(string? nombre, int? idExcluir, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return Result.Success();
        }

        bool existe = await _unitOfWork.Roles.AnyAsync(
            predicate: r => r.Nombre == nombre && (idExcluir == null || r.Id != idExcluir.Value),
            cancellationToken: cancellationToken);

        if (existe)
        {
            return Result.Failure(new ErrorResult("Rol.NombreDuplicado", $"Ya existe un rol con el nombre: {nombre}"));
        }

        return Result.Success();
    }

    private async Task<Result> ValidarRolSinUsuarios(int id, CancellationToken cancellationToken = default)
    {
        bool tieneUsuarios = await _unitOfWork.Usuarios.AnyAsync(
            predicate: u => u.Roles.Any(r => r.Id == id),
            cancellationToken: cancellationToken);

        if (tieneUsuarios)
        {
            return Result.Failure(new ErrorResult("Rol.ConUsuarios", $"No se puede eliminar el rol con el Id: {id} porque tiene usuarios asignados"));
        }

        return Result.Success();
    }

    #endregion
}
```

### `Delivery.Application/Features/Queries/Identity/Roles/RolByIdQuery.cs`

```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.Roles;

public record RolByIdQuery : IRequest<Result<RolDto>>
{
    public int Id { get; init; }
}
```

### `Delivery.Application/Features/Queries/Identity/Roles/RolListQuery.cs`

```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.Roles;

public record RolListQuery : IRequest<Result<IEnumerable<RolDto>>>;
```

### `Delivery.Application/Features/Queries/Identity/Roles/RolQueryHandler.cs`

```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Identity;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.Roles;

public class RolQueryHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<RolByIdQuery, Result<RolDto>>,
    IRequestHandler<RolListQuery, Result<IEnumerable<RolDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<RolDto>> Handle(RolByIdQuery request, CancellationToken cancellationToken = default)
    {
        Rol? rol = await _unitOfWork.Roles.GetByIdAsync(request.Id, true, cancellationToken);

        if (rol == null)
        {
            return Result<RolDto>.Failure(new ErrorResult("Rol.NotFound", $"No se encontró el rol con el Id: {request.Id}"));
        }

        return Result<RolDto>.Success(rol.MapToDto());
    }

    public async Task<Result<IEnumerable<RolDto>>> Handle(RolListQuery request, CancellationToken cancellationToken = default)
    {
        IEnumerable<Rol> roles = await _unitOfWork.Roles.GetAllAsync(true, cancellationToken);
        return Result<IEnumerable<RolDto>>.Success(roles.MapToDto());
    }
}
```

### `Delivery.Application/Features/Commands/Identity/Usuarios/CreateUsuarioCommand.cs`

```c#
using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.Usuarios;

public record CreateUsuarioCommand : IRequest<Result<Guid>>
{
    public string Nombres { get; init; } = string.Empty;
    public string PrimerApellido { get; init; } = string.Empty;
    public string? SegundoApellido { get; init; }
    public string Telefono { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public IReadOnlyList<int> RolIds { get; init; } = [];
}
```

### `Delivery.Application/Features/Commands/Identity/Usuarios/UpdateUsuarioCommand.cs`

```c#
using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.Usuarios;

public record UpdateUsuarioCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string? Nombres { get; init; }
    public string? PrimerApellido { get; init; }
    public string? SegundoApellido { get; init; }
    public string? Telefono { get; init; }
    public string? Correo { get; init; }
    public string? Password { get; init; }
    public IReadOnlyList<int>? RolIds { get; init; }
}
```

### `Delivery.Application/Features/Commands/Identity/Usuarios/DeleteUsuarioCommand.cs`

```c#
using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.Usuarios;

public record DeleteUsuarioCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}
```

### `Delivery.Application/Features/Commands/Identity/Usuarios/UsuarioCommandHandler.cs`

```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Identity;

namespace Delivery.Application.Features.Commands.Identity.Usuarios;

public class UsuarioCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher) :
    IRequestHandler<CreateUsuarioCommand, Result<Guid>>,
    IRequestHandler<UpdateUsuarioCommand, Result>,
    IRequestHandler<DeleteUsuarioCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Result<Guid>> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken = default)
    {
        Result validacionCorreo = await ValidarCorreoDisponible(request.Correo, null, cancellationToken);

        if (validacionCorreo.IsFailure)
        {
            return Result<Guid>.Failure(validacionCorreo.Error);
        }

        Result<IReadOnlyList<Rol>> validacionRoles = await ObtenerRolesExistentes(request.RolIds, cancellationToken);

        if (validacionRoles.IsFailure)
        {
            return Result<Guid>.Failure(validacionRoles.Error);
        }

        Usuario usuario = request.MapToEntity(_passwordHasher.Hash(request.Password));
        usuario.SincronizarRoles(validacionRoles.Value!);

        await _unitOfWork.Usuarios.AddAsync(usuario, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<Guid>.Success(usuario.Id);
    }

    public async Task<Result> Handle(UpdateUsuarioCommand request, CancellationToken cancellationToken = default)
    {
        Usuario? usuario = await BuscarUsuarioPorId(request.Id, cancellationToken);

        if (usuario == null)
        {
            return Result.Failure(new ErrorResult("Usuario.NotFound", $"No se encontró el usuario con el Id: {request.Id}"));
        }

        Result validacionCorreo = await ValidarCorreoDisponible(request.Correo, request.Id, cancellationToken);

        if (validacionCorreo.IsFailure)
        {
            return validacionCorreo;
        }

        if (request.RolIds is not null)
        {
            Result<IReadOnlyList<Rol>> validacionRoles = await ObtenerRolesExistentes(request.RolIds, cancellationToken);

            if (validacionRoles.IsFailure)
            {
                return Result.Failure(validacionRoles.Error);
            }

            usuario.SincronizarRoles(validacionRoles.Value!);
        }

        string? passwordHash = string.IsNullOrWhiteSpace(request.Password)
            ? null
            : _passwordHasher.Hash(request.Password);

        usuario.UpdateEntity(request, passwordHash);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// Elimina lógicamente al usuario. No se borra físicamente porque UsuarioDireccion.UsuarioId
    /// está configurado con DeleteBehavior.Restrict.
    /// </summary>
    public async Task<Result> Handle(DeleteUsuarioCommand request, CancellationToken cancellationToken = default)
    {
        Usuario? usuario = await BuscarUsuarioPorId(request.Id, cancellationToken);

        if (usuario == null)
        {
            return Result.Failure(new ErrorResult("Usuario.NotFound", $"No se encontró el usuario con el Id: {request.Id}"));
        }

        usuario.EsEliminado = true;
        usuario.EsActivo = false;

        _unitOfWork.Usuarios.Update(usuario);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    #region MetodosPrivados

    private async Task<Usuario?> BuscarUsuarioPorId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Usuarios.FirstOrDefaultAsync(
            predicate: u => u.Id == id && !u.EsEliminado,
            disableTracking: false,
            cancellationToken: cancellationToken,
            includes: u => u.Roles);
    }

    private async Task<Result> ValidarCorreoDisponible(string? correo, Guid? idExcluir, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(correo))
        {
            return Result.Success();
        }

        bool existe = await _unitOfWork.Usuarios.AnyAsync(
            predicate: u => u.Correo == correo && !u.EsEliminado && (idExcluir == null || u.Id != idExcluir.Value),
            cancellationToken: cancellationToken);

        if (existe)
        {
            return Result.Failure(new ErrorResult("Usuario.CorreoDuplicado", $"Ya existe un usuario con el correo: {correo}"));
        }

        return Result.Success();
    }

    private async Task<Result<IReadOnlyList<Rol>>> ObtenerRolesExistentes(IReadOnlyList<int> rolIds, CancellationToken cancellationToken = default)
    {
        int[] ids = rolIds.Distinct().ToArray();

        if (ids.Length == 0)
        {
            return Result<IReadOnlyList<Rol>>.Success([]);
        }

        List<Rol> roles = (await _unitOfWork.Roles.GetAsync(
            predicate: r => ids.Contains(r.Id),
            disableTracking: false,
            cancellationToken: cancellationToken)).ToList();

        if (roles.Count != ids.Length)
        {
            return Result<IReadOnlyList<Rol>>.Failure(
                new ErrorResult("Usuario.RolInvalido", "Uno o más de los roles indicados no existen."));
        }

        return Result<IReadOnlyList<Rol>>.Success(roles);
    }

    #endregion
}
```

### `Delivery.Application/Features/Queries/Identity/Usuarios/UsuarioByIdQuery.cs`

```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.Usuarios;

public record UsuarioByIdQuery : IRequest<Result<UsuarioDto>>
{
    public Guid Id { get; init; }
}
```

### `Delivery.Application/Features/Queries/Identity/Usuarios/UsuarioListQuery.cs`

```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.Usuarios;

public record UsuarioListQuery : IRequest<Result<IEnumerable<UsuarioDto>>>
{
    public string? Correo { get; init; }
    public bool SoloActivos { get; init; }
}
```

### `Delivery.Application/Features/Queries/Identity/Usuarios/UsuarioQueryHandler.cs`

```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Identity;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.Usuarios;

public class UsuarioQueryHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<UsuarioByIdQuery, Result<UsuarioDto>>,
    IRequestHandler<UsuarioListQuery, Result<IEnumerable<UsuarioDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<UsuarioDto>> Handle(UsuarioByIdQuery request, CancellationToken cancellationToken = default)
    {
        Usuario? usuario = await _unitOfWork.Usuarios.FirstOrDefaultAsync(
            predicate: u => u.Id == request.Id && !u.EsEliminado,
            disableTracking: true,
            cancellationToken: cancellationToken,
            includes: u => u.Roles);

        if (usuario == null)
        {
            return Result<UsuarioDto>.Failure(new ErrorResult("Usuario.NotFound", $"No se encontró el usuario con el Id: {request.Id}"));
        }

        return Result<UsuarioDto>.Success(usuario.MapToDto());
    }

    public async Task<Result<IEnumerable<UsuarioDto>>> Handle(UsuarioListQuery request, CancellationToken cancellationToken = default)
    {
        IEnumerable<Usuario> usuarios = await _unitOfWork.Usuarios.GetAsync(
            predicate: u => !u.EsEliminado
                && (!request.SoloActivos || u.EsActivo)
                && (string.IsNullOrWhiteSpace(request.Correo) || u.Correo == request.Correo),
            disableTracking: true,
            cancellationToken: cancellationToken,
            includes: u => u.Roles);

        return Result<IEnumerable<UsuarioDto>>.Success(usuarios.MapToDto());
    }
}
```

### `Delivery.Application/Features/Commands/Identity/UsuarioDirecciones/CreateUsuarioDireccionCommand.cs`

```c#
using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.UsuarioDirecciones;

public record CreateUsuarioDireccionCommand : IRequest<Result<Guid>>
{
    public string Calle { get; init; } = string.Empty;
    public string Numero { get; init; } = string.Empty;
    public string Colonia { get; init; } = string.Empty;
    public string Ciudad { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public string CodigoPostal { get; init; } = string.Empty;
    public Guid UsuarioId { get; init; }
}
```

### `Delivery.Application/Features/Commands/Identity/UsuarioDirecciones/UpdateUsuarioDireccionCommand.cs`

```c#
using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.UsuarioDirecciones;

public record UpdateUsuarioDireccionCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Calle { get; init; } = string.Empty;
    public string Numero { get; init; } = string.Empty;
    public string Colonia { get; init; } = string.Empty;
    public string Ciudad { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public string CodigoPostal { get; init; } = string.Empty;
    public Guid UsuarioId { get; init; }
}
```

### `Delivery.Application/Features/Commands/Identity/UsuarioDirecciones/DeleteUsuarioDireccionCommand.cs`

```c#
using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.UsuarioDirecciones;

public record DeleteUsuarioDireccionCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}
```

### `Delivery.Application/Features/Commands/Identity/UsuarioDirecciones/UsuarioDireccionCommandHandler.cs`

```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Identity;

namespace Delivery.Application.Features.Commands.Identity.UsuarioDirecciones;

public class UsuarioDireccionCommandHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<CreateUsuarioDireccionCommand, Result<Guid>>,
    IRequestHandler<UpdateUsuarioDireccionCommand, Result>,
    IRequestHandler<DeleteUsuarioDireccionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateUsuarioDireccionCommand request, CancellationToken cancellationToken = default)
    {
        Result validacionUsuario = await ValidarUsuarioExiste(request.UsuarioId, cancellationToken);

        if (validacionUsuario.IsFailure)
        {
            return Result<Guid>.Failure(validacionUsuario.Error);
        }

        UsuarioDireccion direccion = request.MapToEntity();

        await _unitOfWork.UsuarioDirecciones.AddAsync(direccion, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<Guid>.Success(direccion.Id);
    }

    public async Task<Result> Handle(UpdateUsuarioDireccionCommand request, CancellationToken cancellationToken = default)
    {
        UsuarioDireccion? direccion = await BuscarDireccionPorId(request.Id, cancellationToken);

        if (direccion == null)
        {
            return Result.Failure(new ErrorResult("UsuarioDireccion.NotFound", $"No se encontró la dirección con el Id: {request.Id}"));
        }

        Result validacionUsuario = await ValidarUsuarioExiste(request.UsuarioId, cancellationToken);

        if (validacionUsuario.IsFailure)
        {
            return validacionUsuario;
        }

        direccion.UpdateEntity(request);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Handle(DeleteUsuarioDireccionCommand request, CancellationToken cancellationToken = default)
    {
        UsuarioDireccion? direccion = await BuscarDireccionPorId(request.Id, cancellationToken);

        if (direccion == null)
        {
            return Result.Failure(new ErrorResult("UsuarioDireccion.NotFound", $"No se encontró la dirección con el Id: {request.Id}"));
        }

        _unitOfWork.UsuarioDirecciones.Delete(direccion);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    #region MetodosPrivados

    private async Task<UsuarioDireccion?> BuscarDireccionPorId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.UsuarioDirecciones.GetByIdAsync(id, false, cancellationToken);
    }

    private async Task<Result> ValidarUsuarioExiste(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        bool existe = await _unitOfWork.Usuarios.AnyAsync(u => u.Id == usuarioId && !u.EsEliminado, cancellationToken);

        if (!existe)
        {
            return Result.Failure(new ErrorResult("UsuarioDireccion.UsuarioInvalido",
                $"No se encontró el usuario con el Id: {usuarioId}"));
        }

        return Result.Success();
    }

    #endregion
}
```

### `Delivery.Application/Features/Queries/Identity/UsuarioDirecciones/UsuarioDireccionByIdQuery.cs`

```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.UsuarioDirecciones;

public record UsuarioDireccionByIdQuery : IRequest<Result<UsuarioDireccionDto>>
{
    public Guid Id { get; init; }
}
```

### `Delivery.Application/Features/Queries/Identity/UsuarioDirecciones/UsuarioDireccionListQuery.cs`

```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.UsuarioDirecciones;

public record UsuarioDireccionListQuery : IRequest<Result<IEnumerable<UsuarioDireccionDto>>>
{
    public Guid? UsuarioId { get; init; }
}
```

### `Delivery.Application/Features/Queries/Identity/UsuarioDirecciones/UsuarioDireccionQueryHandler.cs`

```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Identity;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.UsuarioDirecciones;

public class UsuarioDireccionQueryHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<UsuarioDireccionByIdQuery, Result<UsuarioDireccionDto>>,
    IRequestHandler<UsuarioDireccionListQuery, Result<IEnumerable<UsuarioDireccionDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<UsuarioDireccionDto>> Handle(UsuarioDireccionByIdQuery request, CancellationToken cancellationToken = default)
    {
        UsuarioDireccion? direccion = await _unitOfWork.UsuarioDirecciones.FirstOrDefaultAsync(
            predicate: ud => ud.Id == request.Id,
            disableTracking: true,
            cancellationToken: cancellationToken,
            includes: ud => ud.Usuario!);

        if (direccion == null)
        {
            return Result<UsuarioDireccionDto>.Failure(new ErrorResult("UsuarioDireccion.NotFound", $"No se encontró la dirección con el Id: {request.Id}"));
        }

        return Result<UsuarioDireccionDto>.Success(direccion.MapToDto());
    }

    public async Task<Result<IEnumerable<UsuarioDireccionDto>>> Handle(UsuarioDireccionListQuery request, CancellationToken cancellationToken = default)
    {
        IEnumerable<UsuarioDireccion> direcciones = request.UsuarioId is null
            ? await _unitOfWork.UsuarioDirecciones.GetAllAsync(true, cancellationToken, ud => ud.Usuario!)
            : await _unitOfWork.UsuarioDirecciones.GetAsync(
                predicate: ud => ud.UsuarioId == request.UsuarioId.Value,
                disableTracking: true,
                cancellationToken: cancellationToken,
                includes: ud => ud.Usuario!);

        return Result<IEnumerable<UsuarioDireccionDto>>.Success(direcciones.MapToDto());
    }
}
```

### `Delivery.API/Controllers/v1/RolController.cs`

```c#
using MediatR;
using Asp.Versioning;
using Delivery.Core.Result;
using Delivery.Extensions;
using Microsoft.AspNetCore.Mvc;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Queries.Identity.Roles;
using Delivery.Application.Features.Commands.Identity.Roles;

namespace Delivery.Controllers.v1;

/// <summary>
/// Controlador que administra los roles
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/[controller]")]
public class RolController(ISender sender, ILogger<RolController> logger) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly ILogger<RolController> _logger = logger;

    /// <summary>
    /// Crear Rol.
    /// </summary>
    /// <param name="command">Información para crear un rol.</param>
    /// <returns>Rol creado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearRol([FromBody] CreateRolCommand command)
    {
        Result<int> result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito con Id: {Id}", nameof(RolController), nameof(CrearRol), result.Value);
        return CreatedAtAction(nameof(ObtenerRolPorId), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Obtener rol por su Id.
    /// </summary>
    /// <param name="id">Id del rol.</param>
    /// <returns>Rol encontrado.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RolDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerRolPorId([FromRoute] int id)
    {
        RolByIdQuery query = new RolByIdQuery { Id = id };
        Result<RolDto> result = await _sender.Send(query);

        if (result.IsFailure) return NotFound(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito consultando Id: {Id}", nameof(RolController), nameof(ObtenerRolPorId), id);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtener todos los roles.
    /// </summary>
    /// <returns>Listado de roles.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RolDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerRoles()
    {
        RolListQuery query = new RolListQuery();
        Result<IEnumerable<RolDto>> result = await _sender.Send(query);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito", nameof(RolController), nameof(ObtenerRoles));
        return Ok(result.Value);
    }

    /// <summary>
    /// Actualizar rol.
    /// </summary>
    /// <param name="id">Id del rol a actualizar.</param>
    /// <param name="command">Información para actualizar el rol.</param>
    /// <returns>Estado de la actualización.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarRol([FromRoute] int id, [FromBody] UpdateRolCommand command)
    {
        UpdateRolCommand commandSeguro = command with { Id = id };

        Result result = await _sender.Send(commandSeguro);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito actualizando Id: {Id}", nameof(RolController), nameof(ActualizarRol), id);
        return NoContent();
    }

    /// <summary>
    /// Eliminar un rol por su Id. No se permite si tiene usuarios asignados.
    /// </summary>
    /// <param name="id">Id del rol a eliminar.</param>
    /// <returns>Estado de la eliminación.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarRol([FromRoute] int id)
    {
        DeleteRolCommand command = new DeleteRolCommand { Id = id };
        Result result = await _sender.Send(command);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito eliminando Id: {Id}", nameof(RolController), nameof(EliminarRol), id);
        return NoContent();
    }
}
```

### `Delivery.API/Controllers/v1/UsuarioController.cs`

```c#
using MediatR;
using Asp.Versioning;
using Delivery.Core.Result;
using Delivery.Extensions;
using Microsoft.AspNetCore.Mvc;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Queries.Identity.Usuarios;
using Delivery.Application.Features.Commands.Identity.Usuarios;

namespace Delivery.Controllers.v1;

/// <summary>
/// Controlador que administra los usuarios
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/[controller]")]
public class UsuarioController(ISender sender, ILogger<UsuarioController> logger) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly ILogger<UsuarioController> _logger = logger;

    /// <summary>
    /// Crear Usuario. La contraseña se almacena cifrada con BCrypt.
    /// </summary>
    /// <param name="command">Información para crear un usuario.</param>
    /// <returns>Usuario creado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearUsuario([FromBody] CreateUsuarioCommand command)
    {
        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito con Id: {Id}", nameof(UsuarioController), nameof(CrearUsuario), result.Value);
        return CreatedAtAction(nameof(ObtenerUsuarioPorId), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Obtener usuario por su Id.
    /// </summary>
    /// <param name="id">Id del usuario.</param>
    /// <returns>Usuario encontrado.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerUsuarioPorId([FromRoute] Guid id)
    {
        UsuarioByIdQuery query = new UsuarioByIdQuery { Id = id };
        Result<UsuarioDto> result = await _sender.Send(query);

        if (result.IsFailure) return NotFound(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito consultando Id: {Id}", nameof(UsuarioController), nameof(ObtenerUsuarioPorId), id);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtener usuarios. Se puede filtrar por correo y por activos.
    /// </summary>
    /// <param name="correo">Correo exacto para filtrar.</param>
    /// <param name="soloActivos">Si es true, solo devuelve usuarios activos.</param>
    /// <returns>Listado de usuarios.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UsuarioDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerUsuarios([FromQuery] string? correo, [FromQuery] bool soloActivos = false)
    {
        UsuarioListQuery query = new UsuarioListQuery { Correo = correo, SoloActivos = soloActivos };
        Result<IEnumerable<UsuarioDto>> result = await _sender.Send(query);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito", nameof(UsuarioController), nameof(ObtenerUsuarios));
        return Ok(result.Value);
    }

    /// <summary>
    /// Actualizar usuario. Si se envía Password, se recalcula el hash. Si se envía RolIds, se sincronizan los roles.
    /// </summary>
    /// <param name="id">Id del usuario a actualizar.</param>
    /// <param name="command">Información para actualizar el usuario.</param>
    /// <returns>Estado de la actualización.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarUsuario([FromRoute] Guid id, [FromBody] UpdateUsuarioCommand command)
    {
        UpdateUsuarioCommand commandSeguro = command with { Id = id };

        Result result = await _sender.Send(commandSeguro);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito actualizando Id: {Id}", nameof(UsuarioController), nameof(ActualizarUsuario), id);
        return NoContent();
    }

    /// <summary>
    /// Eliminar lógicamente un usuario por su Id (soft delete).
    /// </summary>
    /// <param name="id">Id del usuario a eliminar.</param>
    /// <returns>Estado de la eliminación.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarUsuario([FromRoute] Guid id)
    {
        DeleteUsuarioCommand command = new DeleteUsuarioCommand { Id = id };
        Result result = await _sender.Send(command);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito eliminando Id: {Id}", nameof(UsuarioController), nameof(EliminarUsuario), id);
        return NoContent();
    }
}
```

### `Delivery.API/Controllers/v1/UsuarioDireccionController.cs`

```c#
using MediatR;
using Asp.Versioning;
using Delivery.Core.Result;
using Delivery.Extensions;
using Microsoft.AspNetCore.Mvc;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Queries.Identity.UsuarioDirecciones;
using Delivery.Application.Features.Commands.Identity.UsuarioDirecciones;

namespace Delivery.Controllers.v1;

/// <summary>
/// Controlador que administra las direcciones de los usuarios
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/[controller]")]
public class UsuarioDireccionController(ISender sender, ILogger<UsuarioDireccionController> logger) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly ILogger<UsuarioDireccionController> _logger = logger;

    /// <summary>
    /// Crear dirección de usuario.
    /// </summary>
    /// <param name="command">Información para crear una dirección.</param>
    /// <returns>Dirección creada.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearUsuarioDireccion([FromBody] CreateUsuarioDireccionCommand command)
    {
        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito con Id: {Id}", nameof(UsuarioDireccionController), nameof(CrearUsuarioDireccion), result.Value);
        return CreatedAtAction(nameof(ObtenerUsuarioDireccionPorId), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Obtener dirección por su Id.
    /// </summary>
    /// <param name="id">Id de la dirección.</param>
    /// <returns>Dirección encontrada.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UsuarioDireccionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerUsuarioDireccionPorId([FromRoute] Guid id)
    {
        UsuarioDireccionByIdQuery query = new UsuarioDireccionByIdQuery { Id = id };
        Result<UsuarioDireccionDto> result = await _sender.Send(query);

        if (result.IsFailure) return NotFound(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito consultando Id: {Id}", nameof(UsuarioDireccionController), nameof(ObtenerUsuarioDireccionPorId), id);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtener direcciones. Se puede filtrar por usuario con el query param usuarioId.
    /// </summary>
    /// <param name="usuarioId">Id opcional del usuario para filtrar.</param>
    /// <returns>Listado de direcciones.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UsuarioDireccionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerUsuarioDirecciones([FromQuery] Guid? usuarioId)
    {
        UsuarioDireccionListQuery query = new UsuarioDireccionListQuery { UsuarioId = usuarioId };
        Result<IEnumerable<UsuarioDireccionDto>> result = await _sender.Send(query);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito", nameof(UsuarioDireccionController), nameof(ObtenerUsuarioDirecciones));
        return Ok(result.Value);
    }

    /// <summary>
    /// Actualizar dirección de usuario.
    /// </summary>
    /// <param name="id">Id de la dirección a actualizar.</param>
    /// <param name="command">Información para actualizar la dirección.</param>
    /// <returns>Estado de la actualización.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarUsuarioDireccion([FromRoute] Guid id, [FromBody] UpdateUsuarioDireccionCommand command)
    {
        UpdateUsuarioDireccionCommand commandSeguro = command with { Id = id };

        Result result = await _sender.Send(commandSeguro);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito actualizando Id: {Id}", nameof(UsuarioDireccionController), nameof(ActualizarUsuarioDireccion), id);
        return NoContent();
    }

    /// <summary>
    /// Eliminar una dirección por su Id.
    /// </summary>
    /// <param name="id">Id de la dirección a eliminar.</param>
    /// <returns>Estado de la eliminación.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarUsuarioDireccion([FromRoute] Guid id)
    {
        DeleteUsuarioDireccionCommand command = new DeleteUsuarioDireccionCommand { Id = id };
        Result result = await _sender.Send(command);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito eliminando Id: {Id}", nameof(UsuarioDireccionController), nameof(EliminarUsuarioDireccion), id);
        return NoContent();
    }
}
```

### `Delivery.Application/Features/Queries/Business/RestauranteDirecciones/RestauranteDireccionQueryHandler.cs`

```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Business;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Business.RestauranteDirecciones;

public class RestauranteDireccionQueryHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<RestauranteDireccionByIdQuery, Result<RestauranteDireccionDto>>,
    IRequestHandler<RestauranteDireccionListQuery, Result<IEnumerable<RestauranteDireccionDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<RestauranteDireccionDto>> Handle(RestauranteDireccionByIdQuery request, CancellationToken cancellationToken = default)
    {
        RestauranteDireccion? direccion = await _unitOfWork.RestauranteDirecciones.FirstOrDefaultAsync(
            predicate: rd => rd.Id == request.Id,
            disableTracking: true,
            cancellationToken: cancellationToken,
            includes: rd => rd.Restaurante!);

        if (direccion == null)
        {
            return Result<RestauranteDireccionDto>.Failure(new ErrorResult("RestauranteDireccion.NotFound", $"No se encontró la dirección con el Id: {request.Id}"));
        }

        return Result<RestauranteDireccionDto>.Success(direccion.MapToDto());
    }

    public async Task<Result<IEnumerable<RestauranteDireccionDto>>> Handle(RestauranteDireccionListQuery request, CancellationToken cancellationToken = default)
    {
        IEnumerable<RestauranteDireccion> direcciones = request.RestauranteId is null
            ? await _unitOfWork.RestauranteDirecciones.GetAllAsync(true, cancellationToken, rd => rd.Restaurante!)
            : await _unitOfWork.RestauranteDirecciones.GetAsync(
                predicate: rd => rd.RestauranteId == request.RestauranteId.Value,
                disableTracking: true,
                cancellationToken: cancellationToken,
                includes: rd => rd.Restaurante!);

        return Result<IEnumerable<RestauranteDireccionDto>>.Success(direcciones.MapToDto());
    }
}
```

## Cambios de configuración

### `Delivery.Infrastructure/Delivery.Infrastructure.csproj`
Se agrega el paquete para BCrypt:
```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.2.0" />
```

### `Delivery.Infrastructure/Extension/InfrastructureExtension.cs`
Registro del hasher:
```csharp
public static IServiceCollection AddPasswordHasherConfig(this IServiceCollection services)
{
    services.AddSingleton<IPasswordHasher, PasswordHasher>();

    return services;
}
```

### `Delivery.API/Program.cs`
```csharp
// Registrar cifrado de contraseñas
builder.Services.AddPasswordHasherConfig();
```

## Fix de warnings (CS8603)

`RestauranteDireccionQueryHandler` y `UsuarioDireccionQueryHandler` pasaban navegaciones nullable (`Restaurante?`, `Usuario?`) al parámetro `params Expression<Func<T, object>>[] includes`, que exige un `object` no nulo. Se resolvió con el operador dealv-forzado null en la expresión:

```csharp
includes: rd => rd.Restaurante!   // antes: rd => rd.Restaurante
includes: ud => ud.Usuario!       // antes: ud => ud.Usuario
```

El `!` es solo de compilación, no altera el árbol de expresión, por lo que el `Include` generado por EF es idéntico. Con esto el proyecto queda en **0 errores y 0 advertencias**.

## Nota

No se requiere migración nueva: el esquema `Identity` ya existía completo en `20260919072615_InitialCreate` (tablas `Roles`, `Usuarios`, `UsuarioDirecciones` y la tabla puente `UsuarioRoles`).
