using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasicWebHooks.Core;
public class WebHookInvocation : BaseWebHookEntity
{
    public long WebHookTypeId { get; set; }

    [ForeignKey(nameof(WebHookTypeId))]
    public required WebHookType Type { get; set; }
    
    [MaxLength(1_000)]
    public string? DataJson { get; set; }
    
    public List<WebHookTargetInvocation> Invocations { get; set; } = [];
}