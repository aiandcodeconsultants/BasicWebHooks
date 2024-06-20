namespace BasicWebHooks.Core;
public abstract class BaseWebHookEntity
{
    protected BaseWebHookEntity()
    {
        Created = DateTime.UtcNow;
        Updated = DateTime.UtcNow;
    }

    public int Id { get; set; }

    public DateTime Created { get; set; }

    public DateTime Updated { get; set; }

    public DateTime? Deleted { get; set; }
}