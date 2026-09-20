# Revisa implementacion
Tengo errores

CategoriaCommandHandler
```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Catalog;

namespace Delivery.Application.Features.Commands.Categorias;

public class CategoriaCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoriaCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<int>> Handle(CreateCategoriaCommand request, CancellationToken cancellationToken = default)
    {
        Categoria categoria = request.MapToEntity();
        await _unitOfWork.Categorias.AddAsync(categoria, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        
        return Result<int>.Success(categoria.Id);
    }
}

```

---
UpdateCategoriaCommand
```c#
using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Categorias;

public class UpdateCategoriaCommand :  IRequest<Result>
{
    public int Id { get; init; }
    public string? Nombre { get; init; }
    public string? Descripcion { get; init; }
}
```

---
CategoriaCommandHandler
```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Catalog;

namespace Delivery.Application.Features.Commands.Categorias;

public class CategoriaCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoriaCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<int>> Handle(CreateCategoriaCommand request, CancellationToken cancellationToken = default)
    {
        Categoria categoria = request.MapToEntity();
        await _unitOfWork.Categorias.AddAsync(categoria, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        
        return Result<int>.Success(categoria.Id);
    }
}
```

---
CategoriaQuery
```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Categorias;

public record CategoriaQuery : IRequest<Result<CategoriaDto>>
{
    public int Id { get; init;}
}
```

---
CategoriaQueryHandler
```c#
using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Catalog;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Categorias;

public class CategoriaQueryHandler(IUnitOfWork unitOfWork) : 
    IRequestHandler<Result<CategoriaDto>>, 
    IRequestHandler<Result<IEnumerable<CategoriaDto>>>    
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CategoriaDto>> Handle(CategoriaQuery response, CancellationToken cancellationToken = default)
    {
        Categoria? categoria = await _unitOfWork.Categorias.GetByIdAsync(response.Id, false, cancellationToken)
            ?? throw new KeyNotFoundException("Categoria no encontrada.");
        

        return Result<CategoriaDto>.Success(categoria.MapToDto());
    }

    public async Task<IEnumerable<CategoriaDto>> Handle(CancellationToken cancellationToken = default)
    {
        IEnumerable<Categoria> categorias = await _unitOfWork.Categorias.GetAllAsync(false, cancellationToken);

        return Result<IEnumerable<CategoriaDto>>.Success(categorias.MapToDto());
    }
}
```

---
mapper
```c#
using Delivery.Core.Entities.Catalog;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Commands.Categorias;

namespace Delivery.Application.Mappers;

public static class CategoriaMapper
{
    public static Categoria MapToEntity(this CreateCategoriaCommand command)
    {
        return new Categoria
        {
            Nombre = command.Nombre,
            Descripcion = command.Descripcion ?? "S/D"
        };
    }

    public static void UpdateEntity(this Categoria categoria, UpdateCategoriaCommand updateCommand)
    {
        categoria.Nombre = !string.IsNullOrWhiteSpace(updateCommand.Nombre) ? updateCommand.Nombre : categoria.Nombre;
        categoria.Descripcion = !string.IsNullOrWhiteSpace(updateCommand.Descripcion) ? updateCommand.Descripcion : categoria.Descripcion;
    }

    public static CategoriaDto MapToDto(this Categoria categoria)
    {
        return new CategoriaDto
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion,
            EsActivo = categoria.EsActivo,
            EsEliminado = categoria.EsEliminado
        };
    }

    public static IEnumerable<CategoriaDto> MapToDto(this IEnumerable<Categoria>? categorias)
    {
        return categorias?.Select(MapToDto) ?? [];
    }
}
```