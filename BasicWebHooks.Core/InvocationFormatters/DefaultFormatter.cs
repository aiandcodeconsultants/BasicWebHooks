namespace BasicWebHooks.Core.InvocationFormatters;

/// <summary>
/// A default invocation formatter.
/// </summary>
public class DefaultFormatter : IInvocationFormatter
{
    /// <inheritdoc />
    public ValueTask<string> Format(WebHookTargetInvocation targetInvocation)
        => ValueTask.FromResult((targetInvocation.Invocation ?? throw new InvalidOperationException("No invocation found."))
            .DataJson ?? $"{{ \"type\": \"{targetInvocation.Invocation.Type!.Name}\", \"invocationId\": {targetInvocation.Invocation.Id} }}");
}