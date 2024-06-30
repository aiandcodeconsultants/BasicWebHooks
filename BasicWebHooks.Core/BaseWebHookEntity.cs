using System.ComponentModel.DataAnnotations;

namespace BasicWebHooks.Core;

/// <summary>
/// The common base entity for all WebHook entities.
/// </summary>
public abstract class BaseWebHookEntity
{
    /// <summary>
    /// Constructs a new <see cref="BaseWebHookEntity"/> instance.
    /// </summary>
    protected BaseWebHookEntity()
    {
        Created = DateTime.UtcNow;
        Updated = DateTime.UtcNow;
    }

    /// <summary>
    /// The primary key identifier for the entity.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// When the entity was created.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// When the entity was last updated.
    /// </summary>
    public DateTime Updated { get; set; }

    /// <summary>
    /// When the entity was deleted.
    /// </summary>
    public DateTime? Deleted { get; set; }
}