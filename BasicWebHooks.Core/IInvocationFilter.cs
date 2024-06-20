namespace BasicWebHooks.Core;

public interface IInvocationFilter
{
    ValueTask<bool> ShouldInvoke(WebHookTarget target, WebHookInvocation invocation);
}