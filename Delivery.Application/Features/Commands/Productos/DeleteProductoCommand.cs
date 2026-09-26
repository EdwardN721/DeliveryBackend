using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Productos;

public record DeleteProductoCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}
