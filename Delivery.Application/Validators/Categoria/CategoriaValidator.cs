using FluentValidation;
using Delivery.Application.Features.Commands.Categorias;

namespace Delivery.Application.Validators.Categoria;

public class CrearCategoriaValidator : AbstractValidator<CreateCategoriaCommand>
{
    public CrearCategoriaValidator()
    {
        RuleFor(c => c.Nombre)
            .NotEmpty().WithMessage("El nombre no puede estar vacío")
            .MaximumLength(100).WithMessage("El tamaño máximo del nombre es de 100 caracteres");
        
        RuleFor(c => c.Descripcion)
            .MaximumLength(500).WithMessage("El tamaño máximo de la descripción es de 500 caracteres");
    }
}

public class ActualizarCategoriaValidator : AbstractValidator<UpdateCategoriaCommand>
{
    public ActualizarCategoriaValidator()
    {
        RuleFor(c => c.Id)
            .GreaterThan(0).WithMessage("El Id de la categoría debe ser mayor a 0.");

        RuleFor(c => c.Nombre)
            .MaximumLength(100).WithMessage("El tamaño máximo del nombre es de 100 caracteres.");
        
        RuleFor(c => c.Descripcion)
            .MaximumLength(500).WithMessage("El tamaño máximo de la descripción es de 500 caracteres.");
    }
}
