
using System;
using System.Text;
using System.Text.Json;
using BasicWebHooks.Core.InvocationFormatters;
using Microsoft.Extensions.Logging;

namespace BasicWebHooks.Core.Invokers;

/// <summary>
/// A full web-calling invoker implementation.
/// </summary>
/// <param name="formatters">The invocation formatters.</param>
/// <param name="httpClient">The HTTP client.</param>
/// <param name="logger">The logger.</param>
public class WebInvoker(
    IEnumerable<IInvocationFormatter> formatters,
    HttpClient httpClient,
    ILogger<WebInvoker> logger)
    : IInvoker
{
    /// <summary>
    /// A class defining the parameters available to use in the web invoker.
    /// </summary>
    public class WebInvokerParameters
    {
        /// <summary>
        /// The HTTP method to use.
        /// </summary>
        public string? Method { get; set; } = "POST";

        /// <summary>
        /// The URL to call.
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// The headers to include in the request.
        /// </summary>
        public List<(string, string)> Headers { get; set; } = [];

        /// <summary>
        /// The formatter to use.
        /// </summary>
        public string Formatter { get; set; } = nameof(DefaultFormatter);
    }

    private readonly List<IInvocationFormatter> formatters = formatters.ToList();

    /// <inheritdoc />
    public async ValueTask<Exception?> TryInvoke(WebHookTargetInvocation targetInvocation, CancellationToken cancellationToken = default)
    {
        if (targetInvocation.Target == null)
        {
            var exception = new InvalidOperationException($"{nameof(targetInvocation.Target)} is null");
            logger.LogError(exception, "No target found.");
            return exception;
        }

        if (targetInvocation.Invocation == null)
        {
            var exception = new InvalidOperationException($"{nameof(targetInvocation.Invocation)} is null");
            logger.LogError(exception, "No invocation found.");
            return exception;
        }

        if (targetInvocation.Target.ParametersJson == null)
        {
            var exception = new InvalidOperationException($"{nameof(@targetInvocation.Target.ParametersJson)} is null");
            logger.LogError(exception, "No parameters found.");
            return exception;
        }

        var parameters = JsonSerializer.Deserialize<WebInvokerParameters>(targetInvocation.Target.ParametersJson);

        if (parameters == null)
        {
            var exception = new InvalidOperationException($"{nameof(parameters)} is null");
            logger.LogError(exception, "No parameters found.");
            return exception;
        }

        if (parameters.Url == null)
        {
            var exception = new InvalidOperationException($"{nameof(parameters.Url)} is null");
            logger.LogError(exception, "No URL found.");
            return exception;
        }

        var formatter = formatters.Find(f => f.GetType().Name == parameters.Formatter);

        if (formatter == null)
        {
            var exception = new InvalidOperationException($"{nameof(formatter)} is null");
            logger.LogError(exception, "No formatter found.");
            return exception;
        }

        var formatted = await formatter.Format(targetInvocation);

        try
        {
            logger.LogDebug("Invoking {Method} {Url} with {Formatted}", parameters.Method, parameters.Url, formatted);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "BasicWebHooks.Core.WebInvoker");
            
            if (parameters.Headers != null)
            {
                foreach (var (key, value) in parameters.Headers)
                {
                    httpClient.DefaultRequestHeaders.Add(key, value);
                }
            }

            HttpResponseMessage response;
            Exception? exception = null;

            switch (parameters.Method)
            {
                case "GET":
                    response = await httpClient.GetAsync(parameters.Url, cancellationToken);
                    if (!response.IsSuccessStatusCode)
                    {
                        exception = new InvalidOperationException($"Failed to invoke {parameters.Method} {parameters.Url}");
                        logger.LogError(exception, "Failed to invoke {Method} {Url} with {Formatted}", parameters.Method, parameters.Url, formatted);
                        return exception;
                    }

                    break;
                case "POST":
                    response = await httpClient.PostAsync(
                        parameters.Url,
                        new StringContent(formatted, Encoding.UTF8, "application/json"),
                        cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        exception = new InvalidOperationException($"Failed to invoke {parameters.Method} {parameters.Url}");
                        logger.LogError(exception, "Failed to invoke {Method} {Url} with {Formatted}", parameters.Method, parameters.Url, formatted);
                        return exception;
                    }

                    break;
                default:
                    exception = new InvalidOperationException($"Unsupported method {parameters.Method}");
                    logger.LogError(exception, "Unsupported method {Method}", parameters.Method);
                    return exception;
            }

            if (!response.IsSuccessStatusCode)
            {
                exception = new InvalidOperationException($"Failed to invoke {parameters.Method} {parameters.Url}: Status={response.StatusCode}, Response={await response.Content.ReadAsStringAsync(cancellationToken)}");
                logger.LogError(exception, "Failed to invoke {Method} {Url} with {Formatted}", parameters.Method, parameters.Url, formatted);
                return exception;
            }

            logger.LogInformation("Invoked {Method} {Url} with {Formatted}", parameters.Method, parameters.Url, formatted);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to invoke {Method} {Url} with {Formatted}", parameters.Method, parameters.Url, formatted);
            return ex;
        }
    }
}