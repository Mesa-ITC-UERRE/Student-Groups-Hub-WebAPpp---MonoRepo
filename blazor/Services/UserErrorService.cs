using System.Diagnostics;

namespace StudentGroupsHub.Services;

public sealed class UserErrorService(ILogger<UserErrorService> logger)
{
    public string Handle(Exception exception, string safeMessage)
    {
        var errorId = Activity.Current?.Id ?? Guid.NewGuid().ToString("N");
        logger.LogError(exception, "Unexpected UI error {ErrorId}", errorId);
        return $"{safeMessage} Referencia: {errorId}";
    }
}
