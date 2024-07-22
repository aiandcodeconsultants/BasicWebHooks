namespace BasicWebHooks.Core.Invokers;

public class NullInvoker : IInvoker
{
    public ValueTask<Exception?> TryInvoke(WebHookTargetInvocation targetInvocation, CancellationToken cancellationToken = default)
        => ValueTask.FromResult<Exception?>(null);
}