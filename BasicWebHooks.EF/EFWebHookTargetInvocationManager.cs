using BasicWebHooks.Core;
using BasicWebHooks.Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BasicWebHooks.EF;

public class EFWebHookTargetInvocationManager<TDbContext>(TDbContext db)
    : IWebHookTargetInvocationReader, IWebHookTargetInvocationWriter
    where TDbContext : DbContext, IBasicWebHooksDbContext
{
    private TimeProvider TimeProvider { get; } = TimeProvider.System;
    public EFWebHookTargetInvocationManager(TDbContext db, TimeProvider timeProvider)
        : this(db)
        => TimeProvider = timeProvider;

    public async ValueTask<WebHookTargetInvocation?> GetTargetInvocationById(long id, CancellationToken cancellationToken = default)
        => await db.WebHookTargetInvocations.FindAsync([id], cancellationToken);

    public async ValueTask<List<WebHookTargetInvocation>> ListTargetInvocationByInvocationId(long id, CancellationToken cancellationToken = default)
        => await db.WebHookTargetInvocations
                    .Where(x => x.WebHookInvocationId == id)
                    .ToListAsync(cancellationToken);

    public async ValueTask Remove(WebHookTargetInvocation webhookTargetInvocation, CancellationToken cancellationToken = default)
    {
        var invocation = await db.WebHookTargetInvocations.FindAsync([webhookTargetInvocation.Id], cancellationToken);
        if (invocation != null && invocation.Deleted == null)
        {
            invocation!.Deleted = TimeProvider.GetUtcNow().UtcDateTime;
            _ = await db.SaveChangesAsync(cancellationToken);
        }
    }

    public async ValueTask<long> Upsert(WebHookTargetInvocation webhookTargetInvocation, CancellationToken cancellationToken = default)
    {
        var saved = await db.WebHookTargetInvocations.FindAsync([webhookTargetInvocation.Id], cancellationToken: cancellationToken);
        if (saved != null)
        {
            _ = saved.UpdateFrom(webhookTargetInvocation);
        }
        else
        {
            saved = webhookTargetInvocation;
            _ = await db.WebHookTargetInvocations.AddAsync(saved, cancellationToken);
        }

        _ = await db.SaveChangesAsync(cancellationToken);

        return saved.Id;
    }
}