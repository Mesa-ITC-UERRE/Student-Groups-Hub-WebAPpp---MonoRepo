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

                var (statusCode, message) = ex switch
                {
                    InvalidOperationException => (400, ex.Message),
                    UnauthorizedAccessException => (403, "Acceso no autorizado."),
                    KeyNotFoundException => (404, "Recurso no encontrado."),
                    _ => (500, "Ocurrió un error interno. Por favor intenta de nuevo.")
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                var error = new
                {
                    status = statusCode,
                    message,
                    timestamp = DateTime.UtcNow
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(error));
            });
        });
        return app;
    }
}
