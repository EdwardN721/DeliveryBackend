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
