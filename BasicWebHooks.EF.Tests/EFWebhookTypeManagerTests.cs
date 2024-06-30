
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
    private BasicWebHooksDbContext Db => services!.GetRequiredService<BasicWebHooksDbContext>();
    private EFWebhookTypeManager<BasicWebHooksDbContext> Manager => services!.GetRequiredService<EFWebhookTypeManager<BasicWebHooksDbContext>>();
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
        var result = await Manager.Upsert(webhookType);
        var webhook = await Db.WebHookTypes.FirstOrDefaultAsync();

        // Assert
        _ = result.Should().NotBe(0);
        _ = webhookType.Id.Should().NotBe(0);
        _ = webhook.Should().NotBeNull();
        _ = webhook!.Name.Should().Be(webhookType.Name);
    }

    [Fact]
    public async Task Upsert_WhenWebhookTypeExists_UpdatesWebhookType()
    {
        // Arrange
        var webhookType = new WebHookType { Name = "Test" };
        var newName = "Updated";
        _ = await Db.WebHookTypes!.AddAsync(webhookType);
        _ = await Db.SaveChangesAsync();
        webhookType.Name = newName;

        // Act
        var result = await Manager.Upsert(webhookType);
        var webhook = await Db.WebHookTypes.FirstOrDefaultAsync();
        var count = await Db.WebHookTypes.CountAsync();

        // Assert
        _ = result.Should().Be(webhookType.Id);
        _ = webhook.Should().NotBeNull();
        _ = count.Should().Be(1);
        _ = webhook!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task GetTypeById_ReturnsWebhookType_WhenFound()
    {
        // Arrange
        var webhookType = new WebHookType { Name = "Test" };
        _ = await Db.WebHookTypes!.AddAsync(webhookType);
        _ = await Db.SaveChangesAsync();

        // Act
        var result = await Manager.GetTypeById(webhookType.Id);

        // Assert
        _ = result.Should().NotBeNull();
        _ = result!.Name.Should().Be(webhookType.Name);
    }

    [Fact]
    public async Task GetTypeById_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var webhookType = new WebHookType { Name = "Test" };
        _ = await Db.WebHookTypes!.AddAsync(webhookType);
        _ = await Db.SaveChangesAsync();

        // Act
        var result = await Manager.GetTypeById(webhookType.Id + 1);

        // Assert
        _ = result.Should().BeNull();
    }

    public async Task InitializeAsync()
    {
        services = ConfigureServices(new ServiceCollection())
                            .BuildServiceProvider();
        _ = await Db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        _ = await Db.Database.EnsureDeletedAsync();

        await services!.GetRequiredService<SqliteConnection>().DisposeAsync();
    }
}