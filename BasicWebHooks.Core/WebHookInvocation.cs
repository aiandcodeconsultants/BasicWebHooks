using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasicWebHooks.Core;

/// <summary>
/// A web-hook invocation instance.
/// </summary>
public class WebHookInvocation : BaseWebHookEntity
{
    /// <summary>
    /// The identifier of the web-hook type.
    /// </summary>
    public long WebHookTypeId { get; set; }

    /// <summary>
    /// The web-hook type.
    /// </summary>
    [ForeignKey(nameof(WebHookTypeId))]
    public WebHookType? Type { get; set; }

    /// <summary>
    /// The data associated with the invocation.
    /// </summary>    
    [MaxLength(1_000)]
    public string? DataJson { get; set; }

    /// <summary>
    /// The list of target-invocations.
    /// </summary>    
    public List<WebHookTargetInvocation> Invocations { get; set; } = [];
}