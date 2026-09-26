using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Business.Restaurantes;

public record RestauranteListQuery : IRequest<Result<IEnumerable<RestauranteDto>>>;