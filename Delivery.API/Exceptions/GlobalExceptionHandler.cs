using FluentValidation;
using Delivery.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;

namespace Delivery.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Registramos el error internamente 
        _logger.LogError(exception, "Ocurrió un error inesperado: {Message}", exception.Message);

        // 2. Preparamos la respuesta estándar (ProblemDetails)
        ProblemDetails problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        // 3. Clasificamos la excepción
        switch (exception)
        {
            case ValidationException fluentException:
                problemDetails.Title = "Error de Validación";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Detail = "La petición contiene uno o más errores de validación.";

                problemDetails.Extensions["errores"] = fluentException.Errors
                    .Select(e => new { Campo = e.PropertyName, Mensaje = e.ErrorMessage });
                break;

            case NotFoundException notFoundException:
                problemDetails.Title = "Recurso no encontrado";
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Detail = notFoundException.Message;
                break;
                
            default:
                problemDetails.Title = "Error Interno del Servidor";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Detail = "Ha ocurrido un error inesperado. Por favor, contacte a soporte.";
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status 
            ?? StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
