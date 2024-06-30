using BasicWebHooks.Core;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;

namespace BasicWebHooks.EF.Tests;

public class EFWebHookInvocationManagerTests : IAsyncLifetime
{
    private const string InMemoryConnectionString = "DataSource=:memory:";
    private IServiceProvider? services;
    private BasicWebHooksDbContext Db => services!.GetRequiredService<BasicWebHooksDbContext>();
    private EFWebHookInvocationManager<BasicWebHooksDbContext> Manager => services!.GetRequiredService<EFWebHookInvocationManager<BasicWebHooksDbContext>>();
    private readonly static DateTime Now = DateTime.UtcNow;
    private readonly TimeProvider timeProvider = new FakeTimeProvider(Now.ToDateTimeOffset());

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
            .AddSingleton(timeProvider)
            .AddScoped(provider => new EFWebHookInvocationManager<BasicWebHooksDbContext>(
                provider.GetRequiredService<BasicWebHooksDbContext>(),
                timeProvider));

    protected virtual void ConfigureSqliteOptions(SqliteDbContextOptionsBuilder cfg)
        => cfg.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

    [Fact]
    public async Task GetInvocationById_ReturnsWebHookInvocation_WhenFound()
    {
        // Arrange
        var webhookInvocation = new WebHookInvocation { Id = 1, WebHookTypeId = 1 };
        _ = await Db.WebHookInvocations!.AddAsync(webhookInvocation);
        _ = await Db.SaveChangesAsync();

        // Act
        var result = await Manager.GetInvocationById(webhookInvocation.Id);

        // Assert
        _ = result.Should().NotBeNull();
        _ = result!.WebHookTypeId.Should().Be(webhookInvocation.WebHookTypeId);
    }

    [Fact]
    public async Task GetInvocationById_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var webhookInvocation = new WebHookInvocation { Id = 1, WebHookTypeId = 1 };
        _ = await Db.WebHookInvocations!.AddAsync(webhookInvocation);
        _ = await Db.SaveChangesAsync();

        // Act
        var result = await Manager.GetInvocationById(webhookInvocation.Id + 1);

        // Assert
        _ = result.Should().BeNull();
    }

    [Fact]
    public async Task ListInvocations_ReturnsAllWebHookInvocations()
    {
        // Arrange
        var webhookInvocations = new List<WebHookInvocation>
        {
            new() { Id = 1, WebHookTypeId = 1 },
            new() { Id = 2, WebHookTypeId = 2 },
        };
        await Db.WebHookInvocations!.AddRangeAsync(webhookInvocations);
        _ = await Db.SaveChangesAsync();

        // Act
        var result = await Manager.ListInvocations();

        // Assert
        _ = result.Should().HaveCount(2);
        _ = result.Select(t => t.WebHookTypeId).Should().Contain([1, 2]);
    }

    [Fact]
    public async Task Remove_SetsDeletedTimestamp_WhenInvocationExistsAndNotDeleted()
    {
        // Arrange
        var webhookInvocation = new WebHookInvocation { Id = 1, WebHookTypeId = 1 };
        _ = await Db.WebHookInvocations!.AddAsync(webhookInvocation);
        _ = await Db.SaveChangesAsync();

        // Act
        await Manager.Remove(webhookInvocation);
        var invocation = await Db.WebHookInvocations.IgnoreQueryFilters().FirstOrDefaultAsync();

        // Assert
        _ = invocation.Should().NotBeNull();
        _ = invocation!.Deleted.Should().Be(Now);
    }

    [Fact]
    public async Task Remove_DoesNotSetDeletedTimestamp_WhenInvocationDoesNotExist()
    {
        // Arrange
        var webhookInvocation = new WebHookInvocation { Id = 1, WebHookTypeId = 1 };

        // Act
        await Manager.Remove(webhookInvocation);
        var invocation = await Db.WebHookInvocations.FirstOrDefaultAsync();

        // Assert
        _ = invocation.Should().BeNull();
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