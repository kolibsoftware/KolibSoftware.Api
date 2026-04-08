namespace KolibSoftware.Api.Infra.Models;

/// <summary>
/// Represents an entity that can be marked as deleted with a timestamp and user ID. This is useful for soft deletion, where instead of actually removing the record from the database, we mark it as deleted and keep track of when and by whom it was deleted. This allows for easier recovery of deleted records and auditing of deletion actions.
/// </summary>
public interface IDeleteAuditable
{
    /// <summary>
    /// The date and time when the entity was marked as deleted. This should be set to the current date and time whenever the entity is marked as deleted. The repository or service layer should handle setting this property during a delete operation.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// The identifier of the user who marked the entity as deleted. This should be set to the ID of the user performing the delete operation whenever the entity is marked as deleted. The repository or service layer should handle setting this property during a delete operation.
    /// </summary>
    public Guid? DeletedBy { get; set; }

    /// <summary>
    /// Marks the entity as deleted by setting the DeletedAt and DeletedBy properties. This method should be called by the repository or service layer when an entity is marked as deleted, passing in the ID of the user performing the delete operation.
    /// </summary>
    /// <param name="userId"></param>
    public void MarkAsDeleted(Guid userId)
    {
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }
}