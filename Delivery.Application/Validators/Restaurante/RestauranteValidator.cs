using FluentValidation;
using Delivery.Application.Features.Commands.Restaurantes;

namespace Delivery.Application.Validators.Restaurante;

public class CrearRestauranteValidator : AbstractValidator<CreateRestauranteCommand>
{
    public CrearRestauranteValidator()
    {
        RuleFor(r => r.Nombre)
            .NotEmpty().WithMessage("El nombre del restaurante es obligatorio.")
            .MaximumLength(100).WithMessage("El tamaño máximo del nombre es de 100 caracteres.");
    }
}

public class ActualizarRestauranteValidator : AbstractValidator<UpdateRestauranteCommand>
{
    public ActualizarRestauranteValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty().WithMessage("El Id no debe estar vacío.");

        RuleFor(r => r.Nombre)
            .MaximumLength(100).WithMessage("El tamaño máximo del nombre es de 100 caracteres.");
    }
}

public class EliminarRestauranteValidator : AbstractValidator<DeleteRestauranteCommand>
{
    public EliminarRestauranteValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty().WithMessage("El Id no debe estar vacío.");
    }
}