using FluentValidation;
using Delivery.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // 2. Clasificamos la excepción y creamos el objeto principal
        ProblemDetails problemDetails = exception switch
        {
            // 1. Excepciones de Dominio / Negocio
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
            // 2. Excepciones comunes de .NET
            UnauthorizedAccessException => new ProblemDetails
            {
                Title = "Acceso Denegado",
                Status = StatusCodes.Status403Forbidden,
                Detail = "No tienes los permisos necesarios para realizar esta acción."
            },
            KeyNotFoundException => new ProblemDetails
            {
                Title = "Elemento no encontrado",
                Status = StatusCodes.Status404NotFound,
                Detail = "La clave o identificador proporcionado no existe en el sistema."
            },
            ArgumentException argException => new ProblemDetails
            {
                Title = "Argumento Inválido",
                Status = StatusCodes.Status400BadRequest,
                Detail = argException.Message
            },
            // 3. Excepciones de Base de Datos (EF Core)
            DbUpdateConcurrencyException => new ProblemDetails
            {
                Title = "Conflicto de Concurrencia",
                Status = StatusCodes.Status409Conflict,
                Detail = "El registro fue modificado por otro usuario al mismo tiempo. Por favor, recarga los datos y vuelve a intentarlo."
            },
            DbUpdateException dbException => ManejarExcepcionBaseDatos(dbException),
            // 4. Excepción por defecto (Fallback)
            _ => new ProblemDetails {
                Title = "Error Interno del Servidor",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Ha ocurrido un error inesperado. Por favor, contacte a soporte."
            }
        };

        // 3. Agregamos la ruta en la que ocurrió el error 
        problemDetails.Instance = httpContext.Request.Path;

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    #region MetodosPrivados

    /// <summary>
    /// Método agnóstico para manejar errores de BD. 
    /// Funciona buscando palabras clave en el InnerException generado por Postgres, SQL Server o MySQL.
    /// </summary>
    private ProblemDetails ManejarExcepcionBaseDatos(DbUpdateException exception)
    {
        // Obtenemos el mensaje del motor de base de datos en minúsculas para facilitar la búsqueda
        string mensajeDb = exception.InnerException?.Message.ToLower() ?? string.Empty;

        // Violación de índice único (Ej: Email ya registrado)
        // Postgres: "duplicate key", SQL Server: "violation of unique" o "cannot insert duplicate"
        if (mensajeDb.Contains("unique") || mensajeDb.Contains("duplicate"))
        {
            return new ProblemDetails
            {
                Title = "Registro Duplicado",
                Status = StatusCodes.Status409Conflict,
                Detail = "Ya existe un registro con esta información en el sistema (posible duplicado de clave única)."
            };
        }

        // Violación de llave foránea (Ej: Intentar borrar un estado que está siendo usado por un pedido)
        // Postgres: "violates foreign key", SQL Server: "conflicted with the foreign key"
        if (mensajeDb.Contains("foreign key") || mensajeDb.Contains("reference"))
        {
            return new ProblemDetails
            {
                Title = "Operación Relacional Inválida",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Está intentando hacer referencia a un registro que no existe, o intentando eliminar un registro que está en uso."
            };
        }

        // Violación de restricción Check o longitud máxima
        if (mensajeDb.Contains("check constraint") || mensajeDb.Contains("truncation"))
        {
            return new ProblemDetails
            {
                Title = "Datos Inválidos para la Base de Datos",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Los datos enviados superan el tamaño permitido o no cumplen con las reglas de integridad de la base de datos."
            };
        }

        // Error genérico de BD si no coincide con los anteriores
        return new ProblemDetails
        {
            Title = "Error de Base de Datos",
            Status = StatusCodes.Status500InternalServerError,
            Detail = "Ocurrió un error al intentar guardar los cambios en la base de datos."
        };
    }

    #endregion
}