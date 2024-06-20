namespace BasicWebHooks.Core.InvocationFilters;

public class AlwaysInvocationFilter : IInvocationFilter
{
    public ValueTask<bool> ShouldInvoke(WebHookTarget target, WebHookInvocation invocation)
        => ValueTask.FromResult(true);
}