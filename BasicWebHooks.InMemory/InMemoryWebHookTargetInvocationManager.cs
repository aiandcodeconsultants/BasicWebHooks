namespace BasicWebHooks.InMemory;

using System.Collections.Concurrent;
using BasicWebHooks.Core;

public class InMemoryWebHookTargetInvocationManager
    : IWebHookTargetInvocationReader,
    IWebHookTargetInvocationWriter
{
    private readonly ConcurrentDictionary<long, WebHookTargetInvocation> TargetInvocations = new();

    /// <inheritdoc/>
    public ValueTask<WebHookTargetInvocation?> GetTargetInvocationById(long id, CancellationToken cancellationToken = default)
    {
        TargetInvocations.TryGetValue(id, out var invocation);
        return ValueTask.FromResult(invocation);
    }

    /// <inheritdoc/>
    public ValueTask<List<WebHookTargetInvocation>> ListTargetInvocationByInvocationId(long id, CancellationToken cancellationToken = default)
    {
        var invocations = TargetInvocations.Values
            .Where(x => x.WebHookInvocationId == id)
            .ToList();
        return ValueTask.FromResult(invocations);
    }

    /// <inheritdoc/>
    public ValueTask Remove(WebHookTargetInvocation webhookTargetInvocation, CancellationToken cancellationToken = default)
    {
        TargetInvocations.TryRemove(webhookTargetInvocation.Id, out _);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask<long> Upsert(WebHookTargetInvocation webhookTargetInvocation, CancellationToken cancellationToken = default)
    {
        if (webhookTargetInvocation.Id == 0)
        {
            var nextId = TargetInvocations.Keys.DefaultIfEmpty(0).Max() + 1;
            webhookTargetInvocation.Id = nextId;
        }
        TargetInvocations[webhookTargetInvocation.Id] = webhookTargetInvocation;
        return ValueTask.FromResult(webhookTargetInvocation.Id);
    }
}