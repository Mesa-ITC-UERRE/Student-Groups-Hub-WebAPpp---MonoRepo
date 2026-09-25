using Microsoft.Extensions.Logging.Abstractions;
using StudentGroupsHub.Services;

namespace StudentGroupsHub.Tests.Security;

public sealed class ErrorContractTests
{
    [Fact]
    public void UiError_DoesNotExposeUnexpectedExceptionDetails()
    {
        var service = new UserErrorService(NullLogger<UserErrorService>.Instance);
        const string secretDetail = "database password leaked in stack";

        var message = service.Handle(
            new InvalidOperationException(secretDetail),
            "No se pudo completar la operación.");

        Assert.DoesNotContain(secretDetail, message, StringComparison.Ordinal);
        Assert.Contains("Referencia:", message, StringComparison.Ordinal);
    }

    [Fact]
    public void UserVisibleException_RemainsAnExpectedValidationError()
    {
        var exception = new UserVisibleException("Valor no válido.");

        Assert.IsAssignableFrom<InvalidOperationException>(exception);
        Assert.Equal("Valor no válido.", exception.Message);
    }
}
