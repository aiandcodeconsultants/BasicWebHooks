
using BasicWebHooks.Core;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using NSubstitute;

namespace BasicWebHooks.EF.Tests;
public class EFWebHookTargetInvocationManagerTests : IAsyncLifetime
{
    private readonly BasicWebHooksDbContext db;
    private readonly EFWebHookTargetInvocationManager<BasicWebHooksDbContext> manager;
    private readonly TimeProvider timeProvider;

    public EFWebHookTargetInvocationManagerTests()
    {
        var options = new DbContextOptionsBuilder<BasicWebHooksDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        db = new BasicWebHooksDbContext(options);
        timeProvider = Substitute.For<TimeProvider>();
        manager = new EFWebHookTargetInvocationManager<BasicWebHooksDbContext>(db, timeProvider);
    }

    public async Task DisposeAsync()
    {
        await db.Database.EnsureDeletedAsync();
        await db.DisposeAsync();
    }

    [Fact]
    public async Task GetTargetInvocationById_ReturnsCorrectInvocation()
    {
        var expectedInvocation = new WebHookTargetInvocation { Id = 1, WebHookInvocationId = 1 };
        await db.WebHookTargetInvocations.AddAsync(expectedInvocation);
        await db.SaveChangesAsync();

        var result = await manager.GetTargetInvocationById(1);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedInvocation);
    }

    public Task InitializeAsync() => throw new NotImplementedException();

    [Fact]
    public async Task ListTargetInvocationByInvocationId_ReturnsAllRelatedInvocations()
    {
        await db.WebHookTargetInvocations.AddRangeAsync(
            new WebHookTargetInvocation { Id = 1, WebHookInvocationId = 1 },
            new WebHookTargetInvocation { Id = 2, WebHookInvocationId = 1 },
            new WebHookTargetInvocation { Id = 3, WebHookInvocationId = 2 }
        );
        await db.SaveChangesAsync();

        var results = await manager.ListTargetInvocationByInvocationId(1);

        results.Should().HaveCount(2);
        results.Select(i => i.Id).Should().Contain(new[] { 1L, 2L });
    }

    [Fact]
    public async Task Remove_MarksInvocationAsDeleted()
    {
        var webhookTargetInvocation = new WebHookTargetInvocation { Id = 1, WebHookInvocationId = 1 };
        await db.WebHookTargetInvocations.AddAsync(webhookTargetInvocation);
        await db.SaveChangesAsync();

        timeProvider.GetUtcNow().Returns(DateTime.UtcNow);

        await manager.Remove(webhookTargetInvocation);

        var result = await db.WebHookTargetInvocations.FindAsync(1);
        result.Should().NotBeNull();
        result!.Deleted.Should().BeCloseTo(timeProvider.GetUtcNow().UtcDateTime, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task Upsert_AddsNewInvocation_WhenNotExists()
    {
        var newInvocation = new WebHookTargetInvocation { Id = 2, WebHookInvocationId = 1 };

        var resultId = await manager.Upsert(newInvocation);

        var result = await db.WebHookTargetInvocations.FindAsync(2);
        result.Should().NotBeNull();
        resultId.Should().Be(2);
    }

    [Fact]
    public async Task Upsert_UpdatesExistingInvocation_WhenExists()
    {
        var existingInvocation = new WebHookTargetInvocation { Id = 1, WebHookInvocationId = 1 };
        await db.WebHookTargetInvocations.AddAsync(existingInvocation);
        await db.SaveChangesAsync();

        var updatedInvocation = new WebHookTargetInvocation { Id = 1, WebHookInvocationId = 2 };
        await manager.Upsert(updatedInvocation);

        var result = await db.WebHookTargetInvocations.FindAsync(1);
        result.Should().NotBeNull();
        result!.WebHookInvocationId.Should().Be(2);
    }
}
