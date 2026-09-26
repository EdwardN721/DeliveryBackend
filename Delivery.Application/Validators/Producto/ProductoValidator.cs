using FluentValidation;
using Delivery.Application.Features.Commands.Productos;

namespace Delivery.Application.Validators.Producto;

public class CrearProductoValidator : AbstractValidator<CreateProductoCommand>
{
    public CrearProductoValidator()
    {
        RuleFor(p => p.Nombre)
            .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
            .MaximumLength(150).WithMessage("El tamaño máximo es de 150 caracteres.");

        RuleFor(p => p.Precio)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

        RuleFor(p => p.RestauranteId)
            .NotEmpty().WithMessage("Debe asignar un restaurante válido.");

        RuleFor(p => p.CategoriaId)
            .GreaterThan(0).WithMessage("Debe asignar una categoría válida.");
    }
}

public class ActualizarProductoValidator : AbstractValidator<UpdateProductoCommand>
{
    public ActualizarProductoValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("El Id no debe estar vacío.");

        RuleFor(p => p.Nombre)
            .MaximumLength(150).WithMessage("El tamaño máximo es de 150 caracteres.");

        RuleFor(p => p.Precio)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

        RuleFor(p => p.RestauranteId)
            .NotEmpty().WithMessage("Debe asignar un restaurante válido.");

        RuleFor(p => p.CategoriaId)
            .GreaterThan(0).WithMessage("Debe asignar una categoría válida.");
    }
}

public class EliminarProductoValidator : AbstractValidator<DeleteProductoCommand>
{
    public EliminarProductoValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("El Id no debe estar vacío.");
    }
}
