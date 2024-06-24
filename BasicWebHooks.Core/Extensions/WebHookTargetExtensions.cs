namespace BasicWebHooks.Core.Extensions;

/// <summary>
/// A set of extension methods for <see cref="WebHookTarget"/> instances.
/// </summary>
public static class WebHookTargetExtensions
{
    /// <summary>
    /// Updates the <paramref name="this"/> <see cref="WebHookTarget"/> instance with the values from the <paramref name="updateSource"/> instance.
    /// </summary>
    /// <param name="this">The target to update.</param>
    /// <param name="updateSource">The source to udpate from.</param>
    /// <returns>The updated target object.</returns>
    public static WebHookTarget UpdateFrom(this WebHookTarget @this, WebHookTarget updateSource)
    {
        // NB: Created is deliberately not copied.
        @this.Updated = updateSource.Updated;
        @this.Deleted = updateSource.Deleted;
        @this.FilterType = updateSource.FilterType;
        @this.Id = updateSource.Id;
        @this.Invocations = updateSource.Invocations;
        @this.InvokerType = updateSource.InvokerType;
        @this.Type = updateSource.Type;
        @this.WebHookTypeId = updateSource.WebHookTypeId;

        return @this;
    }
}