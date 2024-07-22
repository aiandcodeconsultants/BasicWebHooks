namespace BasicWebHooks.Core.Invokers;

public class ExceptionInvoker() : IInvoker
{
    private readonly Exception? exceptionToReturn = null;

    public ExceptionInvoker(Exception exceptionToReturn)
        : this()
        => this.exceptionToReturn = exceptionToReturn;

    public ValueTask<Exception?> TryInvoke(WebHookTargetInvocation targetInvocation, CancellationToken cancellationToken = default)
        => ValueTask.FromResult<Exception?>(exceptionToReturn ?? new Exception(nameof(ExceptionInvoker)));
}