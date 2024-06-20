using BasicWebHooks.Configuration.Tests.Extensions;
using BasicWebHooks.Core.Invokers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace BasicWebHooks.Configuration.Tests;

public class WebHookTypesAndTargetsFromConfigurationTests
{
    private IConfiguration TestConfiguration1 => new ConfigurationBuilder()
            .AddFromObject(new
            {
                WebHooks = new
                {
                    type1 = new[]
                    {
                        new
                        {
                            InvokerType = nameof(ConsoleInvoker),
                            ParametersJson = (string?)null,
                        },
                        new
                        {
                            InvokerType = nameof(ConsoleInvoker),
                            ParametersJson = (string?)"{ \"clientId\": \"test12345\" }",
                        },
                    },
                    type2 = new[]
                    {
                        new
                        {
                            InvokerType = nameof(ConsoleInvoker),
                            ParametersJson = "{ \"url\": \"https://example.com\", \"headers\": { \"x-api-key\": \"12345\" } }",
                        },
                    },
                },
            })
            .Build();

    [Fact]
    public void CanGetTypesAndTargetsFromConfiguration()
    {
        // Arrange
        var configuration = TestConfiguration1;

        // Act
        var typesAndTargets = new WebHookTypesAndTargetsFromConfiguration(configuration);

        // Assert
        _ = typesAndTargets.Should().NotBeNull();
    }

    [Fact]
    public async Task GetsCorrectTypeCountFromConfiguration()
    {
        // Arrange
        var configuration = TestConfiguration1;
        var expectedTypeCount = 2;

        // Act
        var typesAndTargets = new WebHookTypesAndTargetsFromConfiguration(configuration);
        var types = await typesAndTargets.ListTypes();

        // Assert
        _ = types.Should().HaveCount(expectedTypeCount);
    }

    [Fact]
    public async Task GetsCorrectTargetCountFromConfiguration()
    {
        // Arrange
        var configuration = TestConfiguration1;
        var expectedTargetCount = 3;

        // Act
        var typesAndTargets = new WebHookTypesAndTargetsFromConfiguration(configuration);
        var types = await typesAndTargets.ListTargets();

        // Assert
        _ = types.Should().HaveCount(expectedTargetCount);
    }

    [Fact]
    public async Task GetsCorrectTargetsByTypeIdFromConfiguration()
    {
        // Arrange
        var configuration = TestConfiguration1;
        var expectedTargetCount = 2;
        var typeId = 1;

        // Act
        var typesAndTargets = new WebHookTypesAndTargetsFromConfiguration(configuration);
        var types = await typesAndTargets.ListTargetsByTypeId(typeId);

        // Assert
        _ = types.Should().HaveCount(expectedTargetCount);
    }

    [Fact]
    public async Task GetsCorrectTypeByIdFromConfiguration()
    {
        // Arrange
        var configuration = TestConfiguration1;
        var expectedType = "type1";
        var typeId = 1;

        // Act
        var typesAndTargets = new WebHookTypesAndTargetsFromConfiguration(configuration);
        var type = await typesAndTargets.GetTypeById(typeId);

        // Assert
        _ = type.Should().NotBeNull();
        _ = type!.Name.Should().Be(expectedType);
    }

    [Fact]
    public async Task GetsCorrectTargetByIdFromConfiguration()
    {
        // Arrange
        var configuration = TestConfiguration1;
        var expectedTargetId = 2;
        var targetId = 2;

        // Act
        var typesAndTargets = new WebHookTypesAndTargetsFromConfiguration(configuration);
        var target = await typesAndTargets.GetTargetById(targetId);

        // Assert
        _ = target.Should().NotBeNull();
        _ = target!.Id.Should().Be(expectedTargetId);
    }
}