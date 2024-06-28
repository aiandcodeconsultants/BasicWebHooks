
using BasicWebHooks.Core;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace BasicWebHooks.EF.Tests;
public class EFWebhookTypeManagerTests : IAsyncLifetime
{
    private const string InMemoryConnectionString = "DataSource=:memory:";
    private IServiceProvider? services;
    private BasicWebHooksDbContext db => services!.GetRequiredService<BasicWebHooksDbContext>();
    private EFWebhookTypeManager<BasicWebHooksDbContext> manager => services!.GetRequiredService<EFWebhookTypeManager<BasicWebHooksDbContext>>();
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
            .AddScoped<EFWebhookTypeManager<BasicWebHooksDbContext>>();

    protected virtual void ConfigureSqliteOptions(SqliteDbContextOptionsBuilder cfg)
        => cfg.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

    [Fact]
    public async Task Upsert_WhenWebhookTypeIsNew_AddsWebhookType()
    {
        // Arrange
        var webhookType = new WebHookType { Name = "Test" };

        // Act
        var result = await manager.Upsert(webhookType);
        var webhook = await db.WebHookTypes.FirstOrDefaultAsync();

        // Assert
        result.Should().NotBe(0);
        webhookType.Id.Should().NotBe(0);
        webhook.Should().NotBeNull();
        webhook!.Name.Should().Be(webhookType.Name);
    }

    [Fact]
    public async Task Upsert_WhenWebhookTypeExists_UpdatesWebhookType()
    {
        // Arrange
        var webhookType = new WebHookType { Name = "Test" };
        var newName = "Updated";
        _ = await db.WebHookTypes!.AddAsync(webhookType);
        _ = await db.SaveChangesAsync();
        webhookType.Name = newName;

        // Act
        var result = await manager.Upsert(webhookType);
        var webhook = await db.WebHookTypes.FirstOrDefaultAsync();
        var count = await db.WebHookTypes.CountAsync();

        // Assert
        result.Should().Be(webhookType.Id);
        webhook.Should().NotBeNull();
        count.Should().Be(1);
        webhook!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task GetTypeById_ReturnsWebhookType_WhenFound()
    {
        // Arrange
        var webhookType = new WebHookType { Name = "Test" };
        _ = await db.WebHookTypes!.AddAsync(webhookType);
        _ = await db.SaveChangesAsync();

        // Act
        var result = await manager.GetTypeById(webhookType.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(webhookType.Name);
    }

    [Fact]
    public async Task GetTypeById_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var webhookType = new WebHookType { Name = "Test" };
        _ = await db.WebHookTypes!.AddAsync(webhookType);
        _ = await db.SaveChangesAsync();

        // Act
        var result = await manager.GetTypeById(webhookType.Id + 1);

        // Assert
        result.Should().BeNull();
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously:- clearer without applying
    public async Task InitializeAsync()
    {
        services = ConfigureServices(new ServiceCollection())
                            .BuildServiceProvider();
        _ = await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        _ = await db.Database.EnsureDeletedAsync();

        await services!.GetRequiredService<SqliteConnection>().DisposeAsync();
    }
}