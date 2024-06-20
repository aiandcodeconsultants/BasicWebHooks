namespace BasicWebHooks.Core;

public interface IInvoker
{
    ValueTask<bool> TryInvoke(WebHookTargetInvocation targetInvocation, out Exception? exception, CancellationToken cancellationToken = default);
}