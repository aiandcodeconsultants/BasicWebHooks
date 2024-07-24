
using BasicWebHooks.Core;
using BasicWebHooks.InMemory;
using BasicWebHooks.MassTransit.Handlers;
using BasicWebHooks.MassTransit.Requests;
using MassTransit;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using OneOf.Monads;

namespace BasicWebHooks.MassTransit.Tests.Handlers;
public class GetWebHookTypesHandlerTests
{
    private readonly TimeProvider TimeProvider = new FakeTimeProvider(new DateTimeOffset(2024, 7, 24, 0, 0, 0, 0, 0, TimeSpan.Zero));
    private readonly GetWebHookTypesHandler handler;
    private readonly IWebHookTypeReader reader;
    private readonly IWebHookTypeWriter writer;
    private readonly ConsumeContext<GetWebHookTypes> context;

    public GetWebHookTypesHandlerTests()
    {
        var manager = new InMemoryWebHookTypeManager(TimeProvider);
        reader = manager;
        writer = manager;
        handler = new GetWebHookTypesHandler(reader);
        context = Substitute.For<ConsumeContext<GetWebHookTypes>>();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WithWebHookTypes()
    {
        // Arrange
        var expectedTypes = new List<WebHookType>
        {
            new WebHookType { Id = 1, Name = "Type1" },
            new WebHookType { Id = 2, Name = "Type2" }
        };

        foreach (var type in expectedTypes)
        {
            await writer.Upsert(type);
        }

        // Act
        await handler.Consume(context);

        // Assert
        await context.Received(1).RespondAsync(
            Arg.Is<Result<Exception, List<WebHookType>>>(x => x.IsSuccess() && x.SuccessValue().Count == 2));
    }
}