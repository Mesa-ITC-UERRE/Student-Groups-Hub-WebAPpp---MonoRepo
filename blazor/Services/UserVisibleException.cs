namespace StudentGroupsHub.Services;

public sealed class UserVisibleException(string message) : InvalidOperationException(message);
