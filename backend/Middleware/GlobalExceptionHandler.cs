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
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var feature = context.Features.Get<IExceptionHandlerFeature>();
                if (feature?.Error is not null)
                {
                    var error = new
                    {
                        status = 500,
                        message = "Ocurrió un error interno. Por favor intenta de nuevo.",
                        timestamp = DateTime.UtcNow
                    };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(error));
                }
            });
        });
        return app;
    }
}
