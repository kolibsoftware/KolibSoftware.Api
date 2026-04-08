namespace KolibSoftware.Api.Infra.Models;

/// <summary>
/// Represents an entity that can be audited for updates. This interface should be implemented by any entity that needs to track when it was last updated and who updated it. The repository or service layer should be responsible for setting these properties when an update occurs.
/// </summary>
public interface IUpdateAuditable
{
    /// <summary>
    /// The date and time when the entity was last updated. This should be set to the current date and time whenever the entity is updated. The repository or service layer should handle setting this property during an update operation.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// The identifier of the user who last updated the entity. This should be set to the ID of the user performing the update whenever the entity is updated. The repository or service layer should handle setting this property during an update operation.
    /// </summary>
    public Guid UpdatedBy { get; set; }

    /// <summary>
    /// Marks the entity as updated by setting the UpdatedAt and UpdatedBy properties. This method should be called by the repository or service layer when an entity is updated, passing in the ID of the user performing the update operation.
    /// </summary>
    /// <param name="userId"></param>
    public void MarkAsUpdated(Guid userId)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userId;
    }
}