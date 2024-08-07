﻿using BasicWebHooks.Core.Extensions;

namespace BasicWebHooks.Core.Invokers;

public class ConsoleInvoker() : IInvoker
{
    private readonly TimeProvider TimeProvider = TimeProvider.System;

    public ConsoleInvoker(TimeProvider timeProvider) : this()
        => TimeProvider = timeProvider;

    public ValueTask<Exception?> TryInvoke(WebHookTargetInvocation targetInvocation, CancellationToken cancellationToken = default)
    {
        var now = TimeProvider.GetUtcNow().UtcDateTime;

        Console.WriteLine($"{now:O} - Invoking {targetInvocation.Id} with {targetInvocation.Invocation!.Type!.Name} ({targetInvocation.Target!.ParametersJson ?? "<null>"})");

        targetInvocation!.Log = (targetInvocation.Log == null ? "" : targetInvocation + "\r\n") + $"Logged to console @ {DateTime.UtcNow:O}";
        targetInvocation.SetUpdated(now).Completed = now;

        return ValueTask.FromResult<Exception?>(null);
    }
}