using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EventSphere.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Skicka vidare till nästa middleware
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Mappa exception-typ till rätt HTTP-statuskod
        var (statusCode, message, errors) = exception switch
        {
            // Valideringsfel → 400 Bad Request
            ValidationException ve => (
                HttpStatusCode.BadRequest,
                "Validation failed.",
                ve.Errors.Select(e => e.ErrorMessage).ToList()
            ),
            // Resurs hittades ej → 404 Not Found
            KeyNotFoundException => (
                HttpStatusCode.NotFound,
                exception.Message,
                (List<string>?)null
            ),
            // Affärsregelbrott → 409 Conflict
            InvalidOperationException => (
                HttpStatusCode.Conflict,
                exception.Message,
                (List<string>?)null
            ),
            // Fel lösenord/ej inloggad → 401 Unauthorized
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                exception.Message,
                (List<string>?)null
            ),
            // Allt annat → 500 Internal Server Error
            _ => (
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.",
                (List<string>?)null
            )
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            statusCode = (int)statusCode,
            message,
            errors,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}