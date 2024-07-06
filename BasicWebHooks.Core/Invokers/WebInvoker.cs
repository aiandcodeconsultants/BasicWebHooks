
using System.Text.Json;
using BasicWebHooks.Core.InvocationFormatters;

namespace BasicWebHooks.Core.Invokers;

/// <summary>
/// A full web-calling invoker implementation.
/// </summary>
public class WebInvoker : IInvoker
{
    public class WebInvokerParameters
    {
        public string? Method { get; set; } = "POST";
        public string? Url { get; set; }
        public List<(string, string)> Headers { get; set; } = [];
        public string Formatter { get; set; } = nameof(DefaultFormatter);
    }

    private readonly List<IInvocationFormatter> formatters;

    public WebInvoker(IEnumerable<IInvocationFormatter> formatters)
    {
        this.formatters = formatters.ToList();
    }

    /// <inheritdoc />
    public ValueTask<bool> TryInvoke(WebHookTargetInvocation targetInvocation, out Exception? exception, CancellationToken cancellationToken = default)
    {
        if (targetInvocation.Target == null)
            throw new InvalidOperationException(nameof(targetInvocation.Target) + " is null");

        if (targetInvocation.Invocation == null)
            throw new InvalidOperationException(nameof(targetInvocation.Invocation) + " is null");

        if(targetInvocation.Target.ParametersJson == null)
            throw new InvalidOperationException(nameof(@targetInvocation.Target.ParametersJson) + " is null");

        var parameters = JsonSerializer.Deserialize<WebInvokerParameters>(targetInvocation.Target.ParametersJson);

        var formatter = 
    }
}