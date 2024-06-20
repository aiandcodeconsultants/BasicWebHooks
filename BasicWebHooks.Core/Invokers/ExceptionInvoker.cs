namespace BasicWebHooks.Core.Invokers;

public class ExceptionInvoker() : IInvoker
{
    private readonly Exception? exceptionToReturn = null;

    public ExceptionInvoker(Exception exceptionToReturn)
        : this()
        => this.exceptionToReturn = exceptionToReturn;

    public ValueTask<bool> TryInvoke(WebHookTargetInvocation targetInvocation, out Exception? exception, CancellationToken cancellationToken = default)
    {
        exception = exceptionToReturn ?? new Exception(nameof(ExceptionInvoker));
        return ValueTask.FromResult(false);
    }
}