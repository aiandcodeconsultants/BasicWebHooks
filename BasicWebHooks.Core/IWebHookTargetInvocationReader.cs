namespace BasicWebHooks.Core;

public interface IWebHookTargetInvocationReader
{
    ValueTask<WebHookTargetInvocation> GetTargetInvocationById(long id, CancellationToken cancellationToken = default);
    ValueTask<List<WebHookTargetInvocation>> ListTargetInvocationByInvocationId(long id, CancellationToken cancellationToken = default);
}