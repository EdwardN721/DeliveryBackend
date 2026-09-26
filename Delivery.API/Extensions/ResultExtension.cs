using Delivery.Core.Result;
using Microsoft.AspNetCore.Mvc;

namespace Delivery.Extensions;

/// <summary>
/// Extensiones para convertir un <see cref="Result"/> en una respuesta HTTP adecuada.
/// </summary>
public static class ResultExtension
{
    /// <summary>
    /// Convierte un <see cref="Result"/> fallido en la acción HTTP correspondiente:
    /// 404 si el error indica recurso no encontrado, de lo contrario 400.
    /// </summary>
    public static IActionResult ToErrorActionResult(this Result result)
    {
        return result.Error.Code.EndsWith(".NotFound", StringComparison.Ordinal)
            ? new NotFoundObjectResult(result.Error)
            : new BadRequestObjectResult(result.Error);
    }
}