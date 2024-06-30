namespace BasicWebHooks.Core.Invokers;

public class NullInvoker : IInvoker
{
    public ValueTask<bool> TryInvoke(WebHookTargetInvocation targetInvocation, out Exception? exception, CancellationToken cancellationToken = default)
    {
        exception = null;
        return ValueTask.FromResult(true);
    }
}