using System.Collections.Concurrent;
using BasicWebHooks.Core;

namespace BasicWebHooks.InMemory;

/// <summary>
/// An in-memory web-hook type manager to be registered as a singleton (type-safe, thread-safe).
/// </summary>
/// <param name="timeProvider"></param>
public class InMemoryWebHookTypeManager(TimeProvider timeProvider)
    : IWebHookTypeReader,
    IWebHookTypeWriter
{
    private readonly ConcurrentDictionary<long, WebHookType> WebHookTypes = new();

    public ValueTask Delete(WebHookType webhookType, CancellationToken cancellationToken = default)
    {
        _ = WebHookTypes.Remove(webhookType.Id, out _);

        return ValueTask.CompletedTask;
    }

    public ValueTask<WebHookType?> GetTypeById(long id, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(WebHookTypes.TryGetValue(id, out var type) ? type : null);

    public ValueTask<List<WebHookType>> ListTypes(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(WebHookTypes.Values.ToList());

    public ValueTask<long> Upsert(WebHookType webhookType, CancellationToken cancellationToken = default)
    {
        if (webhookType.Id == 0)
        {
            webhookType.Id = WebHookTypes.Count == 0 ? 1 : WebHookTypes.Keys.Max() + 1;
        }
        webhookType.Updated = timeProvider.GetUtcNow().UtcDateTime;
        WebHookTypes[webhookType.Id] = webhookType;

        return ValueTask.FromResult(webhookType.Id);
    }
}