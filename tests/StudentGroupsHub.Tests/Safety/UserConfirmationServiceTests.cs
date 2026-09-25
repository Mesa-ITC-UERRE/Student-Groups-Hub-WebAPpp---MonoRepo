using Microsoft.JSInterop;
using StudentGroupsHub.Services;

namespace StudentGroupsHub.Tests.Safety;

public sealed class UserConfirmationServiceTests
{
    [Fact]
    public async Task ConfirmAsync_UsesNativeConfirmationWithImpactMessage()
    {
        var js = new RecordingJsRuntime(result: false);
        var service = new UserConfirmationService(js);

        var confirmed = await service.ConfirmAsync(
            "¿Eliminar esta publicación? Esta acción no se puede deshacer.");

        Assert.False(confirmed);
        Assert.Equal("confirm", js.Identifier);
        Assert.Contains("no se puede deshacer", js.Message, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class RecordingJsRuntime(bool result) : IJSRuntime
    {
        public string? Identifier { get; private set; }
        public string? Message { get; private set; }

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            object?[]? args)
        {
            Identifier = identifier;
            Message = args?.FirstOrDefault()?.ToString();
            return ValueTask.FromResult((TValue)(object)result);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            CancellationToken cancellationToken,
            object?[]? args)
            => InvokeAsync<TValue>(identifier, args);
    }
}
