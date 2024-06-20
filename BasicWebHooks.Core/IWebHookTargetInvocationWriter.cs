namespace BasicWebHooks.Core;

public interface IWebHookTargetInvocationWriter
{
    ValueTask<long> Upsert(WebHookTargetInvocation webhookTargetInvocation, CancellationToken cancellationToken = default);
    ValueTask Remove(WebHookTargetInvocation webhookTargetInvocation, CancellationToken cancellationToken = default);
}