using LibraryApi.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace LibraryApi.Api.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, message) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            DomainException => (StatusCodes.Status409Conflict, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        var correlationId = context.Items["CorrelationId"]?.ToString();
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new { error = message, correlationId }, ct);
        return true;
    }
}
