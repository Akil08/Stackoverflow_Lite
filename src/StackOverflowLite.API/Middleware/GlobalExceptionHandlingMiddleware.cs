using System.Net;
using System.Text.Json;
using FluentValidation;

namespace StackOverflowLite.API.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred while processing request.");
            await WriteErrorResponseAsync(context, ex);
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = MapException(exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var payload = new
        {
            Message = message,
            StatusCode = statusCode
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }

    private static (int StatusCode, string Message) MapException(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => ((int)HttpStatusCode.BadRequest, string.Join("; ", validationException.Errors.Select(x => x.ErrorMessage))),
            KeyNotFoundException notFoundException => ((int)HttpStatusCode.NotFound, notFoundException.Message),
            ArgumentException argumentException => ((int)HttpStatusCode.BadRequest, argumentException.Message),
            _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };
    }
}