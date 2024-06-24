using BasicWebHooks.Core;
using Microsoft.EntityFrameworkCore;

namespace BasicWebHooks.EF;

public interface IBasicWebHooksDbContext
{
    DbSet<WebHookType> WebHookTypes { get; }
    DbSet<WebHookTarget> WebHookTargets { get; }
    DbSet<WebHookInvocation> WebHookInvocations { get; }
    DbSet<WebHookTargetInvocation> WebHookTargetInvocations { get; }
}