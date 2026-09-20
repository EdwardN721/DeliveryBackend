using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Categorias;

public class CreateCategoriaCommand : IRequest<Result<int>>
{
    public string Nombre { get; init; } = string.Empty;
    public string? Decripcion { get; init; }
}
