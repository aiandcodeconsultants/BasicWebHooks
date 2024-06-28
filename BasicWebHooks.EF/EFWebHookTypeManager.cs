using BasicWebHooks.Core;
using Microsoft.EntityFrameworkCore;

namespace BasicWebHooks.EF;

public class EFWebhookTypeManager<TDbContext>(TDbContext db)
    : IWebHookTypeReader, IWebHookTypeWriter
    where TDbContext : DbContext, IBasicWebHooksDbContext
{
    /// <inheritdoc/>
    public async ValueTask<long> /*IBasicWebHooksDbContext.*/Upsert(WebHookType webhookType, CancellationToken cancellationToken = default)
    {
        var entry = webhookType.Id == 0 ? null : await db.WebHookTypes.FindAsync([webhookType.Id], cancellationToken: cancellationToken);
        if (entry is null)
        {
            _ = await db.WebHookTypes.AddAsync(webhookType, cancellationToken);
            entry = webhookType;
        }
        else
            db.Entry(entry).CurrentValues.SetValues(webhookType);
        _ = db.SaveChangesAsync(cancellationToken);

        return entry.Id;
    }

    /// <inheritdoc/>
    public ValueTask /*IBasicWebHooksDbContext.*/Delete(WebHookType webhookType, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public async ValueTask<WebHookType?> /*IWebHookTypeReader.*/GetTypeById(long id, CancellationToken cancellationToken = default)
        => await db.WebHookTypes.FindAsync([id], cancellationToken: cancellationToken);

    /// <inheritdoc/>
    public async ValueTask<List<WebHookType>> /*IWebHookTypeReader.*/ListTypes(CancellationToken cancellationToken = default)
        => await db.WebHookTypes.ToListAsync(cancellationToken);
}