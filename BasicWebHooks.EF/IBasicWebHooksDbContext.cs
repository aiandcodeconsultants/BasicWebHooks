using System.Data.Entity;
using BasicWebHooks.Core;

namespace BasicWebHooks.EF;

public interface IBasicWebHooksDbContext
{
    DbSet<WebHookType> WebHookTypes { get; set; }
    DbSet<WebHookTarget> WebHookTargets { get; set; }
    DbSet<WebHookInvocation> WebHookInvocations { get; set; }
    DbSet<WebHookTargetInvocation> WebHookTargetInvocations { get; set; }
}