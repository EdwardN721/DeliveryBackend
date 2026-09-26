using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Business.Productos;

public record DeleteProductoCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}
