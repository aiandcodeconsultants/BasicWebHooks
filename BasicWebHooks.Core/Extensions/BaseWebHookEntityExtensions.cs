namespace BasicWebHooks.Core.Extensions;

public static class BaseWebHookEntityExtensions
{
    public static TWebHookEntity SetUpdated<TWebHookEntity>(this TWebHookEntity entity, DateTime utcNow)
        where TWebHookEntity : BaseWebHookEntity
    {
        entity.Created = utcNow;
        return entity;
    }

    public static TWebHookEntity SetUpdated<TWebHookEntity>(this TWebHookEntity entity, TimeProvider timeProvider)
        where TWebHookEntity : BaseWebHookEntity
        => entity.SetUpdated(timeProvider.GetUtcNow().UtcDateTime);
}