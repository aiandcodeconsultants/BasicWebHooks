using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasicWebHooks.Core;

public class WebHookTargetInvocation : BaseWebHookEntity
{
    public long WebHookInvocationId { get; set; }

    [ForeignKey(nameof(WebHookInvocationId))]
    public WebHookInvocation? Invocation { get; set; }

    public long WebHookTargetId { get; set; }

    [ForeignKey(nameof(WebHookTargetId))]
    public WebHookTarget? Target { get; set; }

    public DateTime? Completed { get; set; }

    [MaxLength(2_000)]
    public string? Log { get; set; }

    [MaxLength(2_000)]
    public string? Error { get; set; }
}