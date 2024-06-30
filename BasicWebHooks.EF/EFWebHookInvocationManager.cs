using BasicWebHooks.Core;
using Microsoft.EntityFrameworkCore;

namespace BasicWebHooks.EF;
public class EFWebHookInvocationManager<TWebHookDbContext>(TWebHookDbContext db)
    : IWebHookInvocationReader, IWebHookInvocationWriter
    where TWebHookDbContext : DbContext, IBasicWebHooksDbContext
{
    private readonly TimeProvider? timeProvider;
    private TimeProvider TimeProvider => timeProvider ?? TimeProvider.System;
    public EFWebHookInvocationManager(TWebHookDbContext db, TimeProvider timeProvider)
        : this(db)
        => this.timeProvider = timeProvider;

    /// <inheritdoc/>
    public async ValueTask<WebHookInvocation?> GetInvocationById(long id, CancellationToken cancellationToken = default)
        => await db.WebHookInvocations.FindAsync([id], cancellationToken);

    /// <inheritdoc/>
    public async ValueTask<List<WebHookInvocation>> ListInvocations(bool? includeCompleted = false, CancellationToken cancellationToken = default)
        => await db.WebHookInvocations.ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async ValueTask Remove(WebHookInvocation webhookInvocation, CancellationToken cancellationToken = default)
    {
        var invocation = await db.WebHookInvocations.FindAsync([webhookInvocation.Id], cancellationToken);
        if (invocation != null && invocation!.Deleted == null)
        {
            invocation.Deleted = TimeProvider.GetUtcNow().UtcDateTime;
            _ = await db.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public ValueTask<long> Upsert(WebHookInvocation webhookInvocation, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}