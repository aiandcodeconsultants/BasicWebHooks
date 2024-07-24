namespace BasicWebHooks.InMemory.Tests;

using FluentAssertions;
using BasicWebHooks.Core;

public class InMemoryWebHookInvocationManagerTests
{
    private readonly InMemoryWebHookInvocationManager manager = new InMemoryWebHookInvocationManager();

    [Fact]
    public async Task GetInvocationById_ShouldReturnCorrectInvocation()
    {
        var expectedInvocation = new WebHookInvocation { Id = 1, WebHookTypeId = 101 };
        await manager.Upsert(expectedInvocation);

        var result = await manager.GetInvocationById(1);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedInvocation);
    }

    [Fact]
    public async Task GetInvocationById_ShouldReturnNullIfNotFound()
    {
        var result = await manager.GetInvocationById(999);
        result.Should().BeNull();
    }

    [Fact]
    public async Task ListInvocations_ShouldReturnAllOrFiltered()
    {
        var invocation1 = new WebHookInvocation { Id = 1 };
        var invocation2 = new WebHookInvocation { Id = 2, Invocations = [new() { }] };
        await manager.Upsert(invocation1);
        await manager.Upsert(invocation2);

        var allResults = await manager.ListInvocations(null);
        var completedResults = await manager.ListInvocations(true);
        var notCompletedResults = await manager.ListInvocations(false);

        allResults.Should().HaveCount(2);
        completedResults.Should().HaveCount(1).And.ContainSingle(i => i.Id == 1);
        notCompletedResults.Should().HaveCount(1).And.ContainSingle(i => i.Id == 2);
    }

    [Fact]
    public async Task Remove_ShouldRemoveSpecifiedInvocation()
    {
        var invocation = new WebHookInvocation { Id = 1 };
        await manager.Upsert(invocation);
        await manager.Remove(invocation);

        var result = await manager.GetInvocationById(1);
        result.Should().BeNull();
    }

    [Fact]
    public async Task Upsert_ShouldAddOrUpdateInvocation()
    {
        var newInvocation = new WebHookInvocation { Id = 1, WebHookTypeId = 101 };
        var updatedInvocation = new WebHookInvocation { Id = 1, WebHookTypeId = 102 };

        var newId = await manager.Upsert(newInvocation);
        var updateId = await manager.Upsert(updatedInvocation);

        var result = await manager.GetInvocationById(1);

        newId.Should().Be(1);
        updateId.Should().Be(1);
        result.Should().NotBeNull();
        result!.WebHookTypeId.Should().Be(102);
    }
}
