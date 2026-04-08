namespace KolibSoftware.Api.Infra.Models;

/// <summary>
/// Utility class for marking entities as created, updated, or deleted. This class provides extension methods for the ICreateAuditable, IUpdateAuditable, and IDeleteAuditable interfaces to set the appropriate properties when an entity is created, updated, or marked as deleted. The repository or service layer should call these methods when performing create, update, or delete operations to ensure that the audit properties are correctly set.
/// </summary>
public static class ModelUtils
{

    /// <summary>
    /// Marks the entity as created by setting the CreatedAt and CreatedBy properties. This method should be called by the repository or service layer when a new entity is created, passing in the ID of the user performing the create operation.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="userId"></param>
    public static void MarkAsCreated(this ICreateAuditable entity, Guid userId)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.CreatedBy = userId;
    }

    /// <summary>
    /// Marks the entity as updated by setting the UpdatedAt and UpdatedBy properties. This method should be called by the repository or service layer when an entity is updated, passing in the ID of the user performing the update operation.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="userId"></param>
    public static void MarkAsUpdated(this IUpdateAuditable entity, Guid userId)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = userId;
    }

    /// <summary>
    /// Marks the entity as deleted by setting the DeletedAt and DeletedBy properties. This method should be called by the repository or service layer when an entity is marked as deleted, passing in the ID of the user performing the delete operation.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="userId"></param>
    public static void MarkAsDeleted(this IDeleteAuditable entity, Guid userId)
    {
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = userId;
    }
}