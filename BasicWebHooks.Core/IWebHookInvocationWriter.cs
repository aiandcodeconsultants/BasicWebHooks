namespace BasicWebHooks.Core;

public interface IWebHookInvocationWriter
{
    ValueTask<long> Upsert(WebHookInvocation webhookInvocation, CancellationToken cancellationToken = default);
    ValueTask Remove(WebHookInvocation webhookInvocation, CancellationToken cancellationToken = default);
}