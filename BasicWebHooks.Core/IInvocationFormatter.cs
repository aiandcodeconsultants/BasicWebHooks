namespace BasicWebHooks.Core;

/// <summary>
/// An interface for invocation formatters.
/// </summary>
public interface IInvocationFormatter
{
    /// <summary>
    /// Formats details from the target invocation to produce a string for the invocation.
    /// </summary>
    /// <param name="targetInvocation">The target-invocation.</param>
    /// <returns></returns>
    ValueTask<string> Format(WebHookTargetInvocation targetInvocation);
}