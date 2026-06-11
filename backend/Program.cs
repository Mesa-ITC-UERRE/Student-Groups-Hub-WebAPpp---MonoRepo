using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

// ─── Authentication — Supabase JWT (ES256 via JWKS) ──────────────────────────
var supabaseUrl = builder.Configuration["Supabase:Url"]
    ?? throw new InvalidOperationException("Supabase:Url is not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{supabaseUrl}/auth/v1";
        options.MetadataAddress = $"{supabaseUrl}/auth/v1/.well-known/openid-configuration";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidIssuer = $"{supabaseUrl}/auth/v1",
            ValidateAudience = true,
            ValidAudience = "authenticated",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
        };
        // Supabase does not expose a standard OIDC discovery document — use JWKS directly
        options.ConfigurationManager = null;
        options.TokenValidationParameters.IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
        {
            // Fetch JWKS from Supabase and return matching key
            var jwksUri = $"{supabaseUrl}/auth/v1/.well-known/jwks.json";
            var handler = new System.Net.Http.HttpClient();
            var jwksJson = handler.GetStringAsync(jwksUri).GetAwaiter().GetResult();
            var jwks = new Microsoft.IdentityModel.Tokens.JsonWebKeySet(jwksJson);
            return jwks.Keys;
        };
    });

builder.Services.AddAuthorization();

// ─── Database — EF Core + Supabase PostgreSQL ─────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─── Controllers ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ─── Application Services ─────────────────────────────────────────────────────
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<GroupRegistrationRequestService>();

// ─── Health checks ────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

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
