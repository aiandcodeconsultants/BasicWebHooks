namespace BasicWebHooks.Core.Extensions;

/// <summary>
/// A set of extension methods for <see cref="WebHookTargetInvocation"/> instances.
/// </summary>
public static class WebHookTargetInvocationExtensions
{
    /// <summary>
    /// Updates the <paramref name="this"/> <see cref="WebHookTargetInvocation"/> instance with the values from the <paramref name="updateSource"/> instance.
    /// </summary>
    /// <param name="this">The target to update.</param>
    /// <param name="updateSource">The source to udpate from.</param>
    /// <returns>The updated target object.</returns>
    public static WebHookTargetInvocation UpdateFrom(this WebHookTargetInvocation @this, WebHookTargetInvocation updateSource)
    {
        @this.Completed = updateSource.Completed;
        @this.Error = updateSource.Error;
        @this.Id = updateSource.Id;
        @this.Invocation = updateSource.Invocation;
        @this.Log = updateSource.Log;
        @this.Target = updateSource.Target;
        @this.WebHookInvocationId = updateSource.WebHookInvocationId;
        @this.WebHookTargetId = updateSource.WebHookTargetId;

        return @this;
    }
}