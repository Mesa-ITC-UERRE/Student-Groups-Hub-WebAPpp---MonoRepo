using Microsoft.JSInterop;

namespace StudentGroupsHub.Services;

public sealed class UserConfirmationService(IJSRuntime js)
{
    public ValueTask<bool> ConfirmAsync(string message)
        => js.InvokeAsync<bool>("confirm", message);
}
