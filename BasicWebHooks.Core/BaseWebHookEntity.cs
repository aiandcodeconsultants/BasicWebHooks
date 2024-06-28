namespace BasicWebHooks.Core;

using System.ComponentModel.DataAnnotations;

public abstract class BaseWebHookEntity
{
    protected BaseWebHookEntity()
    {
        Created = DateTime.UtcNow;
        Updated = DateTime.UtcNow;
    }

    [Key]
    public long Id { get; set; }

    public DateTime Created { get; set; }

    public DateTime Updated { get; set; }

    public DateTime? Deleted { get; set; }
}