using BasicWebHooks.Core;
using BasicWebHooks.Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BasicWebHooks.EF;

public class EFWebHookTargetManager<TDbContext>(TDbContext db)
    : IWebHookTargetReader, IWebHookTargetWriter
    where TDbContext : DbContext, IBasicWebHooksDbContext
{
    private readonly TimeProvider? timeProvider;
    private TimeProvider TimeProvider => timeProvider ?? TimeProvider.System;
    public EFWebHookTargetManager(TDbContext db, TimeProvider timeProvider)
        : this(db)
        => this.timeProvider = timeProvider;

    public async ValueTask Delete(WebHookTarget webhookTarget, CancellationToken cancellationToken = default)
    {
        var target = await db.WebHookTargets.FindAsync([webhookTarget.Id], cancellationToken: cancellationToken);
        if (target != null && target.Deleted == null)
        {
            target.Deleted = TimeProvider.GetUtcNow().UtcDateTime;
            _ = await db.SaveChangesAsync(cancellationToken);
        }
    }

    public ValueTask<WebHookTarget?> GetTargetById(long id, CancellationToken cancellationToken = default)
        => db.WebHookTargets.FindAsync([id], cancellationToken: cancellationToken);

    public async ValueTask<List<WebHookTarget>> ListTargets(CancellationToken cancellationToken = default)
        => await db.WebHookTargets.ToListAsync(cancellationToken);

    public async ValueTask<List<WebHookTarget>> ListTargetsByTypeId(long id, CancellationToken cancellationToken = default)
        => await db.WebHookTargets.Where(x => x.WebHookTypeId == id)
                            .ToListAsync(cancellationToken);

    public async ValueTask<long> Upsert(WebHookTarget webhookTarget, CancellationToken cancellationToken = default)
    {
        var saved = await db.WebHookTargets.FindAsync([webhookTarget.Id], cancellationToken: cancellationToken);
        if (saved != null)
        {
            _ = saved.UpdateFrom(webhookTarget);
        }
        else
        {
            saved = webhookTarget;
            _ = await db.WebHookTargets.AddAsync(saved, cancellationToken);
        }

        _ = await db.SaveChangesAsync(cancellationToken);

        return saved.Id;
    }
}
