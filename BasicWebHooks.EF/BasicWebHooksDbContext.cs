using BasicWebHooks.Core;
using Microsoft.EntityFrameworkCore;

namespace BasicWebHooks.EF;
public class BasicWebHooksDbContext(DbContextOptions<BasicWebHooksDbContext> options)
    : DbContext(options), IBasicWebHooksDbContext
{   
    public DbSet<WebHookType> WebHookTypes { get; init; }
    public DbSet<WebHookTarget> WebHookTargets { get; init; }
    public DbSet<WebHookInvocation> WebHookInvocations { get; init; }
    public DbSet<WebHookTargetInvocation> WebHookTargetInvocations { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<WebHookType>().HasQueryFilter(x => x.Deleted == null);
        _ = modelBuilder.Entity<WebHookTarget>().HasQueryFilter(x => x.Deleted == null);
        _ = modelBuilder.Entity<WebHookInvocation>().HasQueryFilter(x => x.Deleted == null);
        _ = modelBuilder.Entity<WebHookTargetInvocation>().HasQueryFilter(x => x.Deleted == null);
    }
}
