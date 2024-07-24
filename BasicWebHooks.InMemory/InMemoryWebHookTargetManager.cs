namespace BasicWebHooks.InMemory;

using System.Collections.Concurrent;
using BasicWebHooks.Core;

public class InMemoryWebHookTargetManager
    : IWebHookTargetInvocationReader,
    IWebHookTargetInvocationWriter
{
    private ConcurrentDictionary<long, WebHookTargetInvocation> TargetInvocations { get; } = new();

    public ValueTask<WebHookTargetInvocation?> GetTargetInvocationById(long id, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(TargetInvocations.TryGetValue(id, out var invocation) ? invocation : null);

    public ValueTask<List<WebHookTargetInvocation>> ListTargetInvocationByInvocationId(long id, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(TargetInvocations.Values.Where(x => x.WebHookInvocationId == id).ToList());

    public ValueTask Remove(WebHookTargetInvocation webhookTargetInvocation, CancellationToken cancellationToken = default)
    {
        if (TargetInvocations.TryRemove(webhookTargetInvocation.Id, out _))
        {
            return ValueTask.CompletedTask;
        }

        return ValueTask.FromException(new InvalidOperationException("WebHookTargetInvocation not found"));
    }

    public ValueTask<long> Upsert(WebHookTargetInvocation webhookTargetInvocation, CancellationToken cancellationToken = default)
    {
        if (webhookTargetInvocation.Id == 0)
        {
            webhookTargetInvocation.Id = TargetInvocations.Count == 0 ? 1 : TargetInvocations.Keys.Max() + 1;
        }

        TargetInvocations[webhookTargetInvocation.Id] = webhookTargetInvocation;

        return ValueTask.FromResult(webhookTargetInvocation.Id);
    }
}