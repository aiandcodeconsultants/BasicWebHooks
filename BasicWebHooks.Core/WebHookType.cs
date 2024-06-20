using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasicWebHooks.Core;

public class WebHookType : BaseWebHookEntity
{
    [MaxLength(100)]
    public required string Name { get; set; }

    [InverseProperty(nameof(WebHookTarget.Type))]
    public List<WebHookTarget> Targets { get; set; } = [];

    [InverseProperty(nameof(WebHookInvocation.Type))]
    public List<WebHookInvocation> Invocations { get; set; } = [];
}