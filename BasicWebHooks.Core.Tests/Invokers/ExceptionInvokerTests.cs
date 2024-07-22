using BasicWebHooks.Core.Invokers;
using FluentAssertions;

namespace BasicWebHooks.Core.Tests.Invokers;

public class ExceptionInvokerTests
{
    [Fact]
    public async Task TryInvokeThrowsException()
    {
        // Arrange
        var invoker = new ExceptionInvoker();

        // Act
        var exception = await invoker.TryInvoke(new());

        // Assert
        _ = exception.Should().NotBeNull();
        _ = exception!.Message.Should().Be(nameof(ExceptionInvoker));
    }
}