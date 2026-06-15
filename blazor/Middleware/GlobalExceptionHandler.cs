using Microsoft.AspNetCore.Diagnostics;
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

                // Only intercept API routes — let Blazor handle its own errors
                var isApiRequest = context.Request.Path.StartsWithSegments("/api")
                                || context.Request.Path.StartsWithSegments("/health");

                if (!isApiRequest)
                {
                    // Re-execute to the Blazor error page
                    context.Response.Redirect("/Error");
                    return;
                }

                var (statusCode, message) = ex switch
                {
                    InvalidOperationException => (400, ex.Message),
                    UnauthorizedAccessException => (403, "Acceso no autorizado."),
                    KeyNotFoundException => (404, "Recurso no encontrado."),
                    _ => (500, "Ocurrió un error interno. Por favor intenta de nuevo.")
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message,
                    timestamp = DateTime.UtcNow
                }));
            });
        });
        return app;
    }
}
