using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BasicWebHooks.Core.Invokers;

public class LoggerInvoker(ILogger<LoggerInvoker> logger, TimeProvider? timeProvider = null) : IInvoker
{
    private const string Message = "LoggerInvoker @ {Now}\r\nInvocation: {InvocationJson}";

    public ValueTask<bool> TryInvoke(WebHookTargetInvocation targetInvocation, out Exception? exception, CancellationToken cancellationToken = default)
    {
        try
        {
            var now = (timeProvider ?? TimeProvider.System).GetUtcNow().UtcDateTime;
            logger.LogInformation(Message, now, JsonSerializer.Serialize(targetInvocation));
            exception = null;
            return ValueTask.FromResult(true);
        } catch (Exception ex)
        {
            exception = ex;
            return ValueTask.FromResult(false);
        }
    }
}