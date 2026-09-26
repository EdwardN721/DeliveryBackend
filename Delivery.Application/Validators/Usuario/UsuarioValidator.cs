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
