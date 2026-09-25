using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Tokens;
using StudentGroupsHub.Components;
using StudentGroupsHub.Data;
using StudentGroupsHub.Middleware;
using StudentGroupsHub.Services;var builder = WebApplication.CreateBuilder(args);

// ─── Entra ID — Cookie auth for Blazor (OpenIdConnect) ───────────────────────
var tenantId = builder.Configuration["EntraId:TenantId"];
var oidcAuthority = $"https://login.microsoftonline.com/{tenantId}/v2.0";

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(
        openIdConnectOptions =>
        {
            builder.Configuration.GetSection("EntraId").Bind(openIdConnectOptions);

            // Use the tenant-specific metadata document directly.
            // This skips the "instance discovery" call to the common endpoint
            // which can take 20-30s on slow networks.
            openIdConnectOptions.MetadataAddress =
                $"{oidcAuthority}/.well-known/openid-configuration";
            openIdConnectOptions.Authority = oidcAuthority;

            // Disable extra round-trips
            openIdConnectOptions.GetClaimsFromUserInfoEndpoint = false;
            openIdConnectOptions.SaveTokens = true;

            // Refresh metadata only once per day — not on every request
            openIdConnectOptions.RefreshOnIssuerKeyNotFound = false;
            openIdConnectOptions.ConfigurationManager =
                new Microsoft.IdentityModel.Protocols.ConfigurationManager<
                    Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration>(
                    openIdConnectOptions.MetadataAddress,
                    new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfigurationRetriever(),
                    new Microsoft.IdentityModel.Protocols.HttpDocumentRetriever())
                {
                    AutomaticRefreshInterval = TimeSpan.FromDays(1),
                    RefreshInterval = TimeSpan.FromHours(1),
                };
        },
        cookieOptions => { })
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

// ─── Entra ID — JWT Bearer for REST API controllers ──────────────────────────
builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        var tenantId = builder.Configuration["EntraId:TenantId"];
        options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
        options.Audience  = builder.Configuration["EntraId:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer   = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew        = TimeSpan.FromSeconds(30),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser()
              .AddRequirements(new AdminRoleRequirement()));
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    // Azure Container Apps forwards these headers from the ingress proxy.
    // Clear the trust lists so the proxy headers are honored in container hosting.
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// Register the custom admin role authorization handler
builder.Services.AddSingleton<IAuthorizationHandler, AdminRoleHandler>();

// ─── MVC controllers (REST API + Identity.Web.UI) ────────────────────────────
builder.Services.AddControllersWithViews(options =>
{
    // Blazor pages require authenticated users by default via cookie auth.
    // REST controllers override this with [Authorize(AuthenticationSchemes="Bearer")]
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

// ─── Razor Components + Interactive Server ────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

// ─── Database — EF Core + Supabase PostgreSQL ────────────────────────────────
// Use DbContextFactory so each service/component gets its own context instance.
// This prevents the "A second operation was started on this context" error
// that occurs in Blazor Server when prerender + interactive phases overlap.
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null)));

// Also register as scoped for EF Core tools (migrations, health checks)
builder.Services.AddScoped<AppDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());

// ─── Health checks ────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

// ─── Domain Services (direct injection — no HTTP) ────────────────────────────
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<GroupRegistrationRequestService>();
builder.Services.AddScoped<LeadershipRequestService>();
builder.Services.AddScoped<MembershipService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<GroupTermService>();
builder.Services.AddScoped<GroupPostService>();
builder.Services.AddScoped<EventPostService>();
builder.Services.AddScoped<GroupSeasonService>();

// Current user context (reads ClaimsPrincipal, upserts user on first call)
builder.Services.AddScoped<CurrentUserService>();

// ─── Supabase Storage (server-side uploads via REST API) ─────────────────────
builder.Services.AddHttpClient();
builder.Services.AddScoped<StorageService>();

// ─── Cross-component user state (avatar broadcast within a circuit) ───────────
builder.Services.AddScoped<UserStateService>();

// ─── Blazor API wrapper services (inject domain services, no HttpClient) ──────
builder.Services.AddScoped<GroupApiService>();
builder.Services.AddScoped<EventApiService>();
builder.Services.AddScoped<UserApiService>();
builder.Services.AddScoped<NotificationApiService>();
builder.Services.AddScoped<GroupRegistrationApiService>();
builder.Services.AddScoped<LeadershipRequestApiService>();
builder.Services.AddScoped<DashboardApiService>();
builder.Services.AddScoped<AdminApiService>();
builder.Services.AddScoped<GroupTermApiService>();
builder.Services.AddScoped<GroupPostApiService>();
builder.Services.AddScoped<EventPostApiService>();
builder.Services.AddScoped<MisGruposApiService>();
builder.Services.AddScoped<CalendarApiService>();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// ─── Middleware pipeline ──────────────────────────────────────────────────────
app.UseGlobalExceptionHandler();
app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapHealthChecks("/health");

app.Run();

// Expose the minimal-host entry point to the integration test host.
public partial class Program;
