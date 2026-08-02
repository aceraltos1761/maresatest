using Maresa.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maresa.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            ClienteInvalidoException => (
                StatusCodes.Status400BadRequest,
                "Datos de pedido invalidos",
                exception.Message),
            ClienteValidacionException => (
                StatusCodes.Status502BadGateway,
                "Falla del servicio de validacion de cliente",
                exception.Message),
            DbUpdateException => (
                StatusCodes.Status500InternalServerError,
                "Error al acceder a la base de datos",
                "Ocurrio un error al guardar la informacion. Intente nuevamente."),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Error interno del servidor",
                "Ocurrio un error inesperado. Intente nuevamente.")
        };

        _logger.LogError(exception, "Excepcion no controlada al procesar {Path}: {Title}", context.Request.Path, title);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
