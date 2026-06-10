using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.Middleware;
using StudentGroupsHub.Services;

var builder = WebApplication.CreateBuilder(args);

// ─── CORS ─────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                      ?? ["http://localhost:5173"];
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ─── Authentication — Microsoft Entra ID JWT Bearer ───────────────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/v2.0";
        options.Audience = builder.Configuration["AzureAd:Audience"];
        options.TokenValidationParameters.ValidateIssuer = true;
    });

builder.Services.AddAuthorization();

// ─── Database — EF Core + Supabase PostgreSQL ─────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─── Controllers + FluentValidation ──────────────────────────────────────────
builder.Services.AddControllers();

// ─── Application Services ─────────────────────────────────────────────────────
builder.Services.AddScoped<UserService>();

// ─── Health checks ────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

// ─── OpenAPI ──────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// ─── Middleware pipeline ──────────────────────────────────────────────────────
app.UseGlobalExceptionHandler();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
