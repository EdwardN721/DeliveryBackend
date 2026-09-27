using FluentValidation;
using Delivery.Application.Features.Commands.Transaction.Pedidos;

namespace Delivery.Application.Validators.Pedidos;

public class CrearPedioValidator : AbstractValidator<CreatePedidoCommand>
{
    public CrearPedioValidator()
    {
        RuleFor(p => p.RestauranteId)
            .NotEmpty().WithMessage("Debe seleccionar un restaurante");

        RuleFor(p => p.DireccionEntregaId)
            .NotEmpty().WithMessage("Debe seleccionar una dirección de entraga.");

        RuleFor(p => p.Carrito)
            .NotNull().WithMessage("Debe agregar por lo menos un arituculo al carrito")
            .Must(NoTenerProductosDuplicados).WithMessage("El carrito contiene productos duplicados. Sume las cantidades en una sola línea.");

        RuleForEach(p => p.Carrito).ChildRules(detalle =>
        {
            detalle.RuleFor(d => d.ProductoId)
                .NotEmpty().WithMessage("El Id del producto es obligatorio.");

            detalle.RuleFor(d => d.Cantidad)
                .GreaterThan(0).WithMessage("La cantidad de cada producto debe ser mayor a cero.");
        });
    }

    private bool NoTenerProductosDuplicados(List<PedidoDetalleCommand> carrito)
    {
        if (carrito == null || carrito.Count == 0) return true;

        return carrito.Select(c => c.ProductoId).Distinct().Count() == carrito.Count;
    }
}


public class CambiarEstadoPedidoValidator : AbstractValidator<CambiarEstadoPedidoCommand>
{
    public CambiarEstadoPedidoValidator()
    {
        RuleFor(p => p.IdPedido)
            .NotEmpty().WithMessage("Ingrese un pedido válido.");
        
        RuleFor(p => p.NuevoEstadoId)
            .GreaterThan(0).WithMessage("Debe especificar un estado válido.");
    }
}

public class CancelarPedidoValidator : AbstractValidator<CancelarPedidoCommand>
{
    public CancelarPedidoValidator()
    {
        RuleFor(p => p.IdPedido)
            .NotEmpty().WithMessage("Ingrese un pedido válido.");

        RuleFor(p => p.MotivoCancelacion)
            .NotEmpty().WithMessage("Debe especificar un motivo de cancelación.")
            .MaximumLength(200).WithMessage("El motivo no puede exceder los 200 caracteres.");
    }
}