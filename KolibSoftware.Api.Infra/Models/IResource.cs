namespace KolibSoftware.Api.Infra.Models;

/// <summary>
/// Represents a resource with a unique identifier (Rid). This interface can be implemented by any entity that needs to have a unique identifier for API purposes. The Rid should be a GUID that is generated when the entity is created and should not change throughout the lifetime of the entity. This allows for consistent referencing of the entity in API endpoints and other operations.
/// </summary>
public interface IResource
{
    /// <summary>
    /// The unique identifier for the resource. This should be a GUID that is generated when the entity is created and should not change throughout the lifetime of the entity. The repository or service layer should handle setting this property during a create operation.
    /// </summary>
    public Guid Rid { get; set; }
}