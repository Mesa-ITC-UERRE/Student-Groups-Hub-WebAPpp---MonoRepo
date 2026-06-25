namespace StudentGroupsHub.Services;

/// <summary>
/// Scoped (per-circuit) service that broadcasts in-process user state changes.
/// NavMenu subscribes to <see cref="OnChange"/> so it can refresh the displayed
/// avatar immediately after the user uploads a new photo on the Profile page —
/// without waiting for a full page reload.
/// </summary>
public class UserStateService
{
    public string? AvatarUrl { get; private set; }

    /// <summary>Fired whenever <see cref="SetAvatar"/> is called.</summary>
    public event Action? OnChange;

    /// <summary>Updates the cached avatar URL and notifies all subscribers.</summary>
    public void SetAvatar(string? url)
    {
        AvatarUrl = url;
        OnChange?.Invoke();
    }
}
