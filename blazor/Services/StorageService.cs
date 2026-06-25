using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace StudentGroupsHub.Services;

/// <summary>
/// Wraps the Supabase Storage REST API for server-side file uploads.
///
/// Auth token priority:
///   1. Supabase:ServiceKey  — used if it looks like a real JWT (starts with "eyJ").
///      Set the service_role key in appsettings.Development.json for full RLS bypass.
///   2. Supabase:AnonKey     — fallback (already configured in appsettings.json).
///      Works via Storage RLS policies that permit anon INSERT+UPDATE+SELECT on
///      the avatars/, group-logos/, group-banners/, and group-posts/ paths.
/// </summary>
public class StorageService(
    IHttpClientFactory httpFactory,
    IConfiguration config,
    ILogger<StorageService> logger)
{
    public static readonly string[] AllowedMimeTypes =
        ["image/jpeg", "image/png", "image/webp", "image/gif"];
    public const long MaxBytes = 5L * 1024 * 1024; // 5 MB

    private string SupabaseUrl => config["Supabase:Url"]!;
    private string Bucket      => config["Supabase:StorageBucket"]!;

    /// <summary>
    /// Resolves the auth token: service key if configured as a real JWT, anon key otherwise.
    /// </summary>
    private string AuthToken
    {
        get
        {
            var sk = config["Supabase:ServiceKey"];
            // Real JWTs always start with "eyJ" — treat any other value as unconfigured
            return sk?.StartsWith("eyJ") == true
                ? sk
                : config["Supabase:AnonKey"]!;
        }
    }

    private bool UsingServiceKey =>
        config["Supabase:ServiceKey"]?.StartsWith("eyJ") == true;

    /// <summary>
    /// Uploads a stream to Supabase Storage.
    /// Returns the public URL on success; null on HTTP error.
    /// </summary>
    public async Task<string?> UploadAsync(
        string folder, string fileName, Stream data, string contentType)
    {
        var path   = $"{folder}/{fileName}";
        var url    = $"{SupabaseUrl}/storage/v1/object/{Bucket}/{path}";
        var client = httpFactory.CreateClient();

        using var content = new StreamContent(data);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
        request.Headers.Add("x-upsert", "true");
        request.Content = content;

        logger.LogDebug(
            "Supabase Storage upload: POST {Url} [auth={AuthMode}]",
            url, UsingServiceKey ? "service_role" : "anon");

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            logger.LogError(
                "Supabase Storage upload failed: {Status} {Reason} — path={Path} body={Body}",
                (int)response.StatusCode, response.ReasonPhrase, path, body);
            return null;
        }

        var publicUrl = $"{SupabaseUrl}/storage/v1/object/public/{Bucket}/{path}";
        logger.LogDebug("Supabase Storage upload succeeded: {Url}", publicUrl);
        return publicUrl;
    }

    /// <summary>
    /// Deletes the object at the given public URL.
    /// Silently ignores failures — orphaned files are not worth failing over.
    /// </summary>
    public async Task DeleteByUrlAsync(string? publicUrl)
    {
        if (string.IsNullOrWhiteSpace(publicUrl)) return;

        var marker = $"/storage/v1/object/public/{Bucket}/";
        var idx = publicUrl.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return;

        var storagePath = publicUrl[(idx + marker.Length)..];

        try
        {
            var client = httpFactory.CreateClient();
            var body   = JsonSerializer.Serialize(new { prefixes = new[] { storagePath } });

            using var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"{SupabaseUrl}/storage/v1/object/{Bucket}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Supabase Storage delete failed (non-critical): {Status} — path={Path}",
                    (int)response.StatusCode, storagePath);
            }
        }
        catch (Exception ex)
        {
            // Intentionally silenced for the caller — delete failures are non-critical
            logger.LogWarning(ex, "Supabase Storage delete exception (non-critical): {Path}", storagePath);
        }
    }
}
