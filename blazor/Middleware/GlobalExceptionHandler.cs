using Microsoft.AspNetCore.Diagnostics;
using StudentGroupsHub.Services;
using System.Diagnostics;
using System.Text.Json;

namespace StudentGroupsHub.Middleware;

public static class GlobalExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var ex = feature?.Error;
                var errorId = Activity.Current?.Id ?? context.TraceIdentifier;
                var logger = context.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("GlobalExceptionHandler");

                // Only intercept API routes — let Blazor handle its own errors
                var isApiRequest = context.Request.Path.StartsWithSegments("/api")
                                || context.Request.Path.StartsWithSegments("/health");

                if (!isApiRequest)
                {
                    logger.LogError(ex, "Unhandled UI request error {ErrorId}", errorId);
                    context.Response.Redirect(
                        $"/Error?errorId={Uri.EscapeDataString(errorId)}");
                    return;
                }

                var (statusCode, message) = ex switch
                {
                    UserVisibleException => (400, ex.Message),
                    UnauthorizedAccessException => (403, "Acceso no autorizado."),
                    KeyNotFoundException => (404, "Recurso no encontrado."),
                    _ => (500, "Ocurrió un error interno. Por favor intenta de nuevo.")
                };

                if (statusCode == 500)
                    logger.LogError(ex, "Unhandled API error {ErrorId}", errorId);

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message,
                    errorId,
                    timestamp = DateTime.UtcNow
                }));
            });
        });
        return app;
    }
}
