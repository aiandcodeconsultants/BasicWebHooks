namespace BasicWebHooks.InMemory.Tests;

using BasicWebHooks.Core;
using FluentAssertions;

public class InMemoryWebHookTargetManagerTests
{
    private readonly InMemoryWebHookTargetManager manager = new();

    [Fact]
    public async Task GetTargetInvocationById_ReturnsCorrectInvocation()
    {
        // Assign
        var invocation = new WebHookTargetInvocation { Id = 1, WebHookInvocationId = 100 };
        await manager.Upsert(invocation);

        // Act
        var result = await manager.GetTargetInvocationById(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task ListTargetInvocationByInvocationId_ReturnsAllMatchingInvocations()
    {
        // Assign
        var invocation1 = new WebHookTargetInvocation { Id = 1, WebHookInvocationId = 100 };
        var invocation2 = new WebHookTargetInvocation { Id = 2, WebHookInvocationId = 100 };
        await manager.Upsert(invocation1);
        await manager.Upsert(invocation2);

        // Act
        var results = await manager.ListTargetInvocationByInvocationId(100);

        // Assert
        results.Should().HaveCount(2);
        results.Select(i => i.Id).Should().Contain(new List<long> { 1, 2 });
    }

    [Fact]
    public async Task Remove_RemovesTargetInvocation()
    {
        // Assign
        var invocation = new WebHookTargetInvocation { Id = 1, WebHookInvocationId = 100 };
        await manager.Upsert(invocation);

        // Act
        await manager.Remove(invocation);

        // Assert
        var result = await manager.GetTargetInvocationById(1);
        result.Should().BeNull();
    }

    [Fact]
    public async Task Upsert_AddsNewInvocation()
    {
        // Assign
        var newInvocation = new WebHookTargetInvocation { WebHookInvocationId = 200 };

        // Act
        var id = await manager.Upsert(newInvocation);
        var savedInvocation = await manager.GetTargetInvocationById(id);

        // Assert
        savedInvocation.Should().NotBeNull();
        savedInvocation!.Id.Should().Be(id);
    }

    [Fact]
    public async Task Upsert_UpdatesExistingInvocation()
    {
        // Assign
        var invocation = new WebHookTargetInvocation { Id = 1, WebHookInvocationId = 100 };
        await manager.Upsert(invocation);
        var updatedInvocation = new WebHookTargetInvocation { Id = 1, WebHookInvocationId = 200 };

        // Act
        await manager.Upsert(updatedInvocation);

        // Assert
        var result = await manager.GetTargetInvocationById(1);
        result.Should().NotBeNull();
        result!.WebHookInvocationId.Should().Be(200);
    }
}