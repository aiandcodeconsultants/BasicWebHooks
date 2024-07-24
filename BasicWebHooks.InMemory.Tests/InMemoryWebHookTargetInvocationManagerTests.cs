namespace BasicWebHooks.InMemory.Tests;

using BasicWebHooks.Core;
using FluentAssertions;

public class InMemoryWebHookTargetInvocationManagerTests
{
    private readonly InMemoryWebHookTargetInvocationManager manager = new();

    [Fact]
    public async Task GetTargetInvocationById_ReturnsCorrectInvocation()
    {
        var expectedInvocation = new WebHookTargetInvocation { Id = 1, WebHookInvocationId = 100 };
        await manager.Upsert(expectedInvocation);

        var result = await manager.GetTargetInvocationById(1);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedInvocation);
    }

    [Fact]
    public async Task GetTargetInvocationById_ReturnsNullIfNotFound()
    {
        var result = await manager.GetTargetInvocationById(999);
        result.Should().BeNull();
    }

    [Fact]
    public async Task ListTargetInvocationByInvocationId_ReturnsAllMatchingInvocations()
    {
        var invocation1 = new WebHookTargetInvocation { Id = 1, WebHookInvocationId = 100 };
        var invocation2 = new WebHookTargetInvocation { Id = 2, WebHookInvocationId = 100 };
        await manager.Upsert(invocation1);
        await manager.Upsert(invocation2);

        var results = await manager.ListTargetInvocationByInvocationId(100);

        results.Should().HaveCount(2);
        results.Should().Contain(x => x.Id == 1 && x.WebHookInvocationId == 100);
        results.Should().Contain(x => x.Id == 2 && x.WebHookInvocationId == 100);
    }

    [Fact]
    public async Task Remove_RemovesTargetInvocation()
    {
        var invocation = new WebHookTargetInvocation { Id = 1, WebHookInvocationId = 100 };
        await manager.Upsert(invocation);
        await manager.Remove(invocation);

        var result = await manager.GetTargetInvocationById(1);
        result.Should().BeNull();
    }

    [Fact]
    public async Task Upsert_AddsNewInvocationWhenNotExists()
    {
        var newInvocation = new WebHookTargetInvocation { WebHookInvocationId = 200 };

        var id = await manager.Upsert(newInvocation);
        var savedInvocation = await manager.GetTargetInvocationById(id);

        savedInvocation.Should().NotBeNull();
        savedInvocation.Id.Should().Be(id);
        savedInvocation.WebHookInvocationId.Should().Be(200);
    }

    [Fact]
    public async Task Upsert_UpdatesExistingInvocation()
    {
        var existingInvocation = new WebHookTargetInvocation { Id = 1, WebHookInvocationId = 100 };
        await manager.Upsert(existingInvocation);

        var updatedInvocation = new WebHookTargetInvocation { Id = 1, WebHookInvocationId = 200 };
        await manager.Upsert(updatedInvocation);

        var result = await manager.GetTargetInvocationById(1);
        result.Should().NotBeNull();
        result!.WebHookInvocationId.Should().Be(200);
    }
}