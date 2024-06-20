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
        var result = await invoker.TryInvoke(new(), out Exception? exception);

        // Assert
        _ = result.Should().BeFalse();
        _ = exception.Should().NotBeNull();
        _ = exception!.Message.Should().Be(nameof(ExceptionInvoker));
    }
}