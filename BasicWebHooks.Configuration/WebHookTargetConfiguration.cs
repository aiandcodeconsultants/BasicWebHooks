namespace BasicWebHooks.Configuration;

public class WebHookTargetConfiguration
{
    public required string InvokerType { get; set; }
    public string? ParametersJson { get; set; }
    public string? FilterType { get; set; }
}
