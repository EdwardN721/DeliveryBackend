using FluentValidation;
using Delivery.Application.Features.Commands.RestauranteDirecciones;

namespace Delivery.Application.Validators.RestauranteDireccion;

public class CrearRestauranteDireccionValidator : AbstractValidator<CreateRestauranteDireccionCommand>
{
    public CrearRestauranteDireccionValidator()
    {
        RuleFor(rd => rd.Calle)
            .NotEmpty().WithMessage("La calle es obligatoria.")
            .MaximumLength(100).WithMessage("El tamaño máximo de la calle es de 100 caracteres.");

        RuleFor(rd => rd.Numero)
            .NotEmpty().WithMessage("El número es obligatorio.")
            .MaximumLength(10).WithMessage("El tamaño máximo del número es de 10 caracteres.");

        RuleFor(rd => rd.Colonia)
            .NotEmpty().WithMessage("La colonia es obligatoria.")
            .MaximumLength(100).WithMessage("El tamaño máximo de la colonia es de 100 caracteres.");

        RuleFor(rd => rd.Ciudad)
            .NotEmpty().WithMessage("La ciudad es obligatoria.")
            .MaximumLength(100).WithMessage("El tamaño máximo de la ciudad es de 100 caracteres.");

        RuleFor(rd => rd.Estado)
            .NotEmpty().WithMessage("El estado es obligatorio.")
            .MaximumLength(100).WithMessage("El tamaño máximo del estado es de 100 caracteres.");

        RuleFor(rd => rd.CodigoPostal)
            .NotEmpty().WithMessage("El código postal es obligatorio.")
            .MaximumLength(10).WithMessage("El tamaño máximo del código postal es de 10 caracteres.");

        RuleFor(rd => rd.RestauranteId)
            .NotEmpty().WithMessage("Debe asignar un restaurante válido.");
    }
}

public class ActualizarRestauranteDireccionValidator : AbstractValidator<UpdateRestauranteDireccionCommand>
{
    public ActualizarRestauranteDireccionValidator()
    {
        RuleFor(rd => rd.Id)
            .NotEmpty().WithMessage("El Id no debe estar vacío.");

        RuleFor(rd => rd.Calle)
            .MaximumLength(100).WithMessage("El tamaño máximo de la calle es de 100 caracteres.");

        RuleFor(rd => rd.Numero)
            .MaximumLength(10).WithMessage("El tamaño máximo del número es de 10 caracteres.");

        RuleFor(rd => rd.Colonia)
            .MaximumLength(100).WithMessage("El tamaño máximo de la colonia es de 100 caracteres.");

        RuleFor(rd => rd.Ciudad)
            .MaximumLength(100).WithMessage("El tamaño máximo de la ciudad es de 100 caracteres.");

        RuleFor(rd => rd.Estado)
            .MaximumLength(100).WithMessage("El tamaño máximo del estado es de 100 caracteres.");

        RuleFor(rd => rd.CodigoPostal)
            .MaximumLength(10).WithMessage("El tamaño máximo del código postal es de 10 caracteres.");

        RuleFor(rd => rd.RestauranteId)
            .NotEmpty().WithMessage("Debe asignar un restaurante válido.");
    }
}

public class EliminarRestauranteDireccionValidator : AbstractValidator<DeleteRestauranteDireccionCommand>
{
    public EliminarRestauranteDireccionValidator()
    {
        RuleFor(rd => rd.Id)
            .NotEmpty().WithMessage("El Id no debe estar vacío.");
    }
}