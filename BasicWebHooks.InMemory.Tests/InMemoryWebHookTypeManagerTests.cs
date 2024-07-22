namespace BasicWebHooks.InMemory.Tests;

using BasicWebHooks.InMemory;
using BasicWebHooks.Core;
using Microsoft.Extensions.Time.Testing;
using FluentAssertions;

public class InMemoryWebHookTypeManagerTests
{
    private readonly FakeTimeProvider timeProvider;
    private readonly InMemoryWebHookTypeManager manager;

    public InMemoryWebHookTypeManagerTests()
    {
        timeProvider = new FakeTimeProvider(); // Assuming FakeTimeProvider is correctly implemented to control time
        manager = new InMemoryWebHookTypeManager(timeProvider);
    }

    [Fact]
    public async Task Upsert_AddsNewWebHookType()
    {
        var newType = new WebHookType { Id = 0, Name = "New Type" };

        var id = await manager.Upsert(newType);

        id.Should().NotBe(0);
        var retrievedType = await manager.GetTypeById(id);
        retrievedType.Should().NotBeNull();
        retrievedType!.Name.Should().Be("New Type");
    }

    [Fact]
    public async Task Upsert_UpdatesExistingWebHookType()
    {
        var existingType = new WebHookType { Id = 1, Name = "Existing Type" };
        await manager.Upsert(existingType);

        var updatedType = new WebHookType { Id = 1, Name = "Updated Type" };
        await manager.Upsert(updatedType);

        var retrievedType = await manager.GetTypeById(1);
        retrievedType.Should().NotBeNull();
        retrievedType!.Name.Should().Be("Updated Type");
    }

    [Fact]
    public async Task Delete_RemovesWebHookType()
    {
        var type = new WebHookType { Id = 2, Name = "Type To Delete" };
        await manager.Upsert(type);

        await manager.Delete(type);

        var retrievedType = await manager.GetTypeById(2);
        retrievedType.Should().BeNull();
    }

    [Fact]
    public async Task ListTypes_ReturnsAllTypes()
    {
        var type1 = new WebHookType { Id = 3, Name = "Type 1" };
        var type2 = new WebHookType { Id = 4, Name = "Type 2" };
        await manager.Upsert(type1);
        await manager.Upsert(type2);

        var types = await manager.ListTypes();

        types.Should().HaveCount(2)
            .And.Contain(t => t.Name == "Type 1")
            .And.Contain(t => t.Name == "Type 2");
    }
}