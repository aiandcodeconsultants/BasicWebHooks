using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasicWebHooks.Core;

public class WebHookTarget : BaseWebHookEntity
{
    public long WebHookTypeId { get; set; }

    [ForeignKey(nameof(WebHookTypeId))]
    public WebHookType? Type { get; set; }

    [InverseProperty(nameof(WebHookTargetInvocation.Target))]
    public List<WebHookTargetInvocation> Invocations { get; set; } = [];

    [MaxLength(255)]
    public required string InvokerType { get; set; }

    [MaxLength(1_000)]
    public string? ParametersJson { get; set; }

    [MaxLength(100)]
    public string? FilterType { get; set; }
}