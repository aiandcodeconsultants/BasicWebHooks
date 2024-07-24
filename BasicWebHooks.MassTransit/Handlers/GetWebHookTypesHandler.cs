namespace BasicWebHooks.MassTransit.Handlers;

using BasicWebHooks.Core;
using BasicWebHooks.MassTransit.Requests;
using global::MassTransit;
using OneOf.Monads;

public class GetWebHookTypesHandler
    : IConsumer<GetWebHookTypes>
{
    private readonly IWebHookTypeReader reader;

    public GetWebHookTypesHandler(IWebHookTypeReader reader)
        => this.reader = reader;

    /// <inheritdoc/>
    public async Task Consume(ConsumeContext<GetWebHookTypes> context)
    {
        try
        {
            var types = await reader.ListTypes(context.CancellationToken).ConfigureAwait(false);

            await context.RespondAsync(Result<Exception, List<WebHookType>>.Success(types)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await context.RespondAsync(Result<Exception, List<WebHookType>>.Error(ex)).ConfigureAwait(false);
        }
    }
}