namespace BasicWebHooks.Core.InvocationFilters;

public class SkipInvocationFiltercs : IInvocationFilter
{
    public ValueTask<bool> ShouldInvoke(WebHookTarget target, WebHookInvocation invocation)
        => ValueTask.FromResult(false);
}