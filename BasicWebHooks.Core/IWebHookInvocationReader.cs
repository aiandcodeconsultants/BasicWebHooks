namespace BasicWebHooks.Core;

public interface IWebHookInvocationReader
{
    ValueTask<WebHookInvocation> GetInvocationById(long id, CancellationToken cancellationToken = default);
    ValueTask<List<WebHookInvocation>> ListInvocations(bool? includeCompleted = false, CancellationToken cancellationToken = default);
}