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

        problemDetails = exception switch
        {
            ValidationException fluentException => new ProblemDetails
            {
                Title = "Error de Validación",
                Status = StatusCodes.Status400BadRequest,
                Detail = "La petición contiene uno o más errores de validación.",

                Extensions =
                {
                    ["errores"] = fluentException.Errors
                        .Select(e => new
                        {
                            Campo = e.PropertyName,
                            Mensaje = e.ErrorMessage
                        })
                }
            },
            NotFoundException notFoundException => new ProblemDetails
            {
                Title = "Recurso no encontrado",
                Status = StatusCodes.Status404NotFound,
                Detail = notFoundException.Message
            },
            _ => new ProblemDetails {
                Title = "Error Interno del Servidor",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Ha ocurrido un error inesperado. Por favor, contacte a soporte."
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status
            ?? StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
