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
