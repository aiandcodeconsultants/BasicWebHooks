namespace BasicWebHooks.Core;

public interface IWebHookTargetWriter
{
    ValueTask<long> Upsert(WebHookTarget webhookTarget, CancellationToken cancellationToken = default);
    ValueTask Delete(WebHookTarget webhookTarget, CancellationToken cancellationToken = default);
}