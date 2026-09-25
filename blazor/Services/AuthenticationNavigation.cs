namespace StudentGroupsHub.Services;

public static class AuthenticationNavigation
{
    public static string BuildSignInUrl(string baseUri, string currentUri)
    {
        var baseAddress = new Uri(baseUri, UriKind.Absolute);
        var appPath = baseAddress.AbsolutePath.TrimEnd('/');
        var signInPath = $"{appPath}/MicrosoftIdentity/Account/SignIn";
        var returnPath = GetLocalReturnPath(baseAddress, currentUri, appPath);

        return $"{signInPath}?redirectUri={Uri.EscapeDataString(returnPath)}";
    }

    private static string GetLocalReturnPath(Uri baseAddress, string currentUri, string appPath)
    {
        if (!Uri.TryCreate(currentUri, UriKind.Absolute, out var current)
            || !string.Equals(current.Scheme, baseAddress.Scheme, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(current.Authority, baseAddress.Authority, StringComparison.OrdinalIgnoreCase)
            || !current.AbsolutePath.StartsWith(
                baseAddress.AbsolutePath,
                StringComparison.OrdinalIgnoreCase))
        {
            return string.IsNullOrEmpty(appPath) ? "/" : $"{appPath}/";
        }

        var relative = baseAddress.MakeRelativeUri(current).ToString();
        if (string.IsNullOrEmpty(relative))
            return string.IsNullOrEmpty(appPath) ? "/" : $"{appPath}/";

        return $"{appPath}/{relative.TrimStart('/')}";
    }
}
