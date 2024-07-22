using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BasicWebHooks.Core.Invokers;

public class LoggerInvoker(ILogger<LoggerInvoker> logger, TimeProvider? timeProvider = null) : IInvoker
{
    private const string Message = "LoggerInvoker @ {Now}\r\nInvocation: {InvocationJson}";

    public ValueTask<Exception?> TryInvoke(WebHookTargetInvocation targetInvocation, CancellationToken cancellationToken = default)
    {
        try
        {
            var now = (timeProvider ?? TimeProvider.System).GetUtcNow().UtcDateTime;
            logger.LogInformation(Message, now, JsonSerializer.Serialize(targetInvocation));
            return ValueTask.FromResult<Exception?>(null);
        } catch (Exception ex)
        {
            return ValueTask.FromResult<Exception?>(ex);
        }
    }
}