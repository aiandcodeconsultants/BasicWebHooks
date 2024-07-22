namespace BasicWebHooks.Core;

public interface IInvoker
{
    ValueTask<Exception?> TryInvoke(WebHookTargetInvocation targetInvocation, CancellationToken cancellationToken = default);
}