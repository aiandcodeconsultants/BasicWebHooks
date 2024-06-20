namespace BasicWebHooks.Core;

public interface IWebHookTypeWriter
{
    ValueTask<long> Upsert(WebHookType webhookType, CancellationToken cancellationToken = default);
    ValueTask Delete(WebHookType webhookType, CancellationToken cancellationToken = default);
}