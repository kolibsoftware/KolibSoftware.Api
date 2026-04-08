namespace KolibSoftware.Api.Infra.Models;

/// <summary>
/// Represents an entity that can be audited for creation. This interface should be implemented by any entity that needs to track when it was created and who created it. The repository or service layer should be responsible for setting these properties when a new entity is created.
/// </summary>
public interface ICreateAuditable
{
    /// <summary>
    /// The date and time when the entity was created. This should be set to the current date and time whenever a new entity is created. The repository or service layer should handle setting this property during a create operation.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The identifier of the user who created the entity. This should be set to the ID of the user performing the create operation whenever a new entity is created. The repository or service layer should handle setting this property during a create operation.
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Marks the entity as created by setting the CreatedAt and CreatedBy properties. This method should be called by the repository or service layer when a new entity is created, passing in the ID of the user performing the create operation.
    /// </summary>
    /// <param name="userId"></param>
    public void MarkAsCreated(Guid userId)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = userId;
    }
}