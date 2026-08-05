using System.Text.Json;
using WatchLog.Application.Common;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Api.Middleware;

/// <summary>Maps Application-layer exceptions to RFC 7807 `ProblemDetails` responses.</summary>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var (status, title) = ex switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
                ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
                ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden"),
                AppValidationException => (StatusCodes.Status400BadRequest, "Validation Failed"),
                TmdbNotConfiguredException => (StatusCodes.Status503ServiceUnavailable, "TMDB Not Configured"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
            };

            if (status == StatusCodes.Status500InternalServerError)
            {
                logger.LogError(ex, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = status;

            var problem = new
            {
                type = $"https://httpstatuses.io/{status}",
                title,
                status,
                detail = ex.Message,
                errors = ex is AppValidationException validationEx ? validationEx.Errors : null,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
}
