using BasicWebHooks.Core;
using BasicWebHooks.Core.Invokers;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace BasicWebHooks.EF.Tests;

public class EFWebHookTargetManagerTests : IAsyncLifetime
{
    private const string InMemoryConnectionString = "DataSource=:memory:";
    private IServiceProvider? services;
    private BasicWebHooksDbContext Db => services!.GetRequiredService<BasicWebHooksDbContext>();
    private EFWebHookTargetManager<BasicWebHooksDbContext> Manager => services!.GetRequiredService<EFWebHookTargetManager<BasicWebHooksDbContext>>();
    private readonly TimeProvider timeProvider = Substitute.For<TimeProvider>();

    protected virtual string ConnectionString => InMemoryConnectionString;

    protected virtual IServiceCollection ConfigureServices(ServiceCollection services)
        => services
            .AddSingleton(_ =>
            {
                var connection = new SqliteConnection(ConnectionString);
                connection.Open();
                return connection;
            })
            .AddDbContext<BasicWebHooksDbContext>((services, options) =>
                options.UseSqlite(
                    services.GetRequiredService<SqliteConnection>(),
                    false,
                    ConfigureSqliteOptions))
            .AddScoped<IBasicWebHooksDbContext>(provider => provider.GetRequiredService<BasicWebHooksDbContext>())
            .AddScoped(provider => new EFWebHookTargetManager<BasicWebHooksDbContext>(
                provider.GetRequiredService<BasicWebHooksDbContext>(),
                timeProvider));

    protected virtual void ConfigureSqliteOptions(SqliteDbContextOptionsBuilder cfg)
        => cfg.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

    [Fact]
    public async Task Upsert_WhenWebHookTargetIsNew_AddsWebHookTarget()
    {
        // Arrange
        var webhookTarget = new WebHookTarget { Id = 0, WebHookTypeId = 1, InvokerType = nameof(NullInvoker) };

        // Act
        var result = await Manager.Upsert(webhookTarget);
        var target = await Db.WebHookTargets.FirstOrDefaultAsync();

        // Assert
        _ = result.Should().NotBe(0);
        _ = webhookTarget.Id.Should().NotBe(0);
        _ = target.Should().NotBeNull();
        _ = target!.InvokerType.Should().Be(webhookTarget.InvokerType);
    }

    [Fact]
    public async Task Upsert_WhenWebHookTargetExists_UpdatesWebHookTarget()
    {
        // Arrange
        var webhookTarget = new WebHookTarget { Id = 1, WebHookTypeId = 1, InvokerType = nameof(ConsoleInvoker) };
        var newInvokerType = nameof(NullInvoker);
        _ = await Db.WebHookTargets!.AddAsync(webhookTarget);
        _ = await Db.SaveChangesAsync();
        webhookTarget.InvokerType = newInvokerType;

        // Act
        var result = await Manager.Upsert(webhookTarget);
        var target = await Db.WebHookTargets.FirstOrDefaultAsync();
        var count = await Db.WebHookTargets.CountAsync();

        // Assert
        _ = result.Should().Be(webhookTarget.Id);
        _ = target.Should().NotBeNull();
        _ = count.Should().Be(1);
        _ = target!.InvokerType.Should().Be(newInvokerType);
    }

    [Fact]
    public async Task Delete_SetsDeletedTimestamp_WhenTargetExistsAndNotDeleted()
    {
        // Arrange
        var webhookTarget = new WebHookTarget { Id = 1, WebHookTypeId = 1, InvokerType = nameof(NullInvoker) };
        _ = await Db.WebHookTargets!.AddAsync(webhookTarget);
        _ = await Db.SaveChangesAsync();

        var now = DateTime.UtcNow;
        _ = timeProvider.GetUtcNow().Returns(new DateTimeOffset(now));

        // Act
        await Manager.Delete(webhookTarget);
        var target = await Db.WebHookTargets.IgnoreQueryFilters().FirstOrDefaultAsync();
        var count = await Db.WebHookTargets.CountAsync();

        // Assert
        _ = target.Should().NotBeNull();
        _ = target!.Deleted.Should().Be(now);
        _ = count.Should().Be(0);
    }

    [Fact]
    public async Task Delete_DoesNotSetDeletedTimestamp_WhenTargetDoesNotExist()
    {
        // Arrange
        var webhookTarget = new WebHookTarget { Id = 1, WebHookTypeId = 1, InvokerType = nameof(NullInvoker) };

        // Act
        await Manager.Delete(webhookTarget);
        var target = await Db.WebHookTargets.FirstOrDefaultAsync();

        // Assert
        _ = target.Should().BeNull();
    }

    [Fact]
    public async Task GetTargetById_ReturnsWebHookTarget_WhenFound()
    {
        // Arrange
        var webhookTarget = new WebHookTarget { Id = 1, WebHookTypeId = 1, InvokerType = nameof(NullInvoker) };
        _ = await Db.WebHookTargets!.AddAsync(webhookTarget);
        _ = await Db.SaveChangesAsync();

        // Act
        var result = await Manager.GetTargetById(webhookTarget.Id);

        // Assert
        _ = result.Should().NotBeNull();
        _ = result!.InvokerType.Should().Be(webhookTarget.InvokerType);
    }

    [Fact]
    public async Task GetTargetById_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var webhookTarget = new WebHookTarget { Id = 1, WebHookTypeId = 1, InvokerType = nameof(NullInvoker) };
        _ = await Db.WebHookTargets!.AddAsync(webhookTarget);
        _ = await Db.SaveChangesAsync();

        // Act
        var result = await Manager.GetTargetById(webhookTarget.Id + 1);

        // Assert
        _ = result.Should().BeNull();
    }

    [Fact]
    public async Task ListTargets_ReturnsAllWebHookTargets()
    {
        // Arrange
        var webhookTargets = new List<WebHookTarget>
        {
            new() { Id = 1, WebHookTypeId = 1, InvokerType = nameof(NullInvoker) },
            new() { Id = 2, WebHookTypeId = 1, InvokerType = nameof(ConsoleInvoker) }
        };
        await Db.WebHookTargets!.AddRangeAsync(webhookTargets);
        _ = await Db.SaveChangesAsync();

        // Act
        var result = await Manager.ListTargets();

        // Assert
        _ = result.Should().HaveCount(2);
        _ = result.Select(t => t.InvokerType).Should().Contain([nameof(NullInvoker), nameof(ConsoleInvoker)]);
    }

    [Fact]
    public async Task ListTargetsByTypeId_ReturnsWebHookTargetsOfType()
    {
        // Arrange
        var webhookTargets = new List<WebHookTarget>
        {
            new() { Id = 1, WebHookTypeId = 1, InvokerType = nameof(NullInvoker) },
            new() { Id = 2, WebHookTypeId = 2, InvokerType = nameof(ConsoleInvoker) }
        };
        await Db.WebHookTargets!.AddRangeAsync(webhookTargets);
        _ = await Db.SaveChangesAsync();

        // Act
        var result = await Manager.ListTargetsByTypeId(1);

        // Assert
        _ = result.Should().HaveCount(1);
        _ = result[0].InvokerType.Should().Be(nameof(NullInvoker));
        _ = result[0].Type!.Name.Should().Be("Test");
    }

    public async Task InitializeAsync()
    {
        services = ConfigureServices(new ServiceCollection())
                            .BuildServiceProvider();
        _ = await Db.Database.EnsureCreatedAsync();
        _ = await Db.WebHookTypes.AddAsync(new WebHookType { Name = "Test" });
        _ = await Db.WebHookTypes.AddAsync(new WebHookType { Name = "Another" });
        _ = await Db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _ = await Db.Database.EnsureDeletedAsync();
        await services!.GetRequiredService<SqliteConnection>().DisposeAsync();
    }
}
