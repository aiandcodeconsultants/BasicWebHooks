using System.Collections.Concurrent;
using BasicWebHooks.Core;

namespace BasicWebHooks.InMemory;

/// <summary>
/// An in-memory implementation of <see cref="IWebHookInvocationReader"/> and <see cref="IWebHookInvocationWriter"/>.
/// </summary>
public class InMemoryWebHookInvocationManager
    : IWebHookInvocationReader,
    IWebHookInvocationWriter
{
    private readonly ConcurrentDictionary<long, WebHookInvocation> invocations = new();

    /// <inheritdoc/>
    public ValueTask<WebHookInvocation?> GetInvocationById(long id, CancellationToken cancellationToken = default)
    {
        invocations.TryGetValue(id, out var invocation);
        return ValueTask.FromResult(invocation);
    }

    /// <inheritdoc/>
    public ValueTask<List<WebHookInvocation>> ListInvocations(bool? includeCompleted = false, CancellationToken cancellationToken = default)
    {
        var results = includeCompleted.HasValue
            ? invocations.Values.Where(x => x.Invocations == null || includeCompleted.Value == x.Invocations!.TrueForAll(i => i.Completed != null)).ToList()
            : invocations.Values.ToList();
        return ValueTask.FromResult(results);
    }

    /// <inheritdoc/>
    public ValueTask Remove(WebHookInvocation webhookInvocation, CancellationToken cancellationToken = default)
    {
        invocations.TryRemove(webhookInvocation.Id, out _);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask<long> Upsert(WebHookInvocation webhookInvocation, CancellationToken cancellationToken = default)
    {
        if (webhookInvocation.Id == 0)
        {
            var nextId = invocations.Keys.DefaultIfEmpty(0).Max() + 1;
            webhookInvocation.Id = nextId;
        }
        invocations[webhookInvocation.Id] = webhookInvocation;
        return ValueTask.FromResult(webhookInvocation.Id);
    }
}