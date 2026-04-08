namespace KolibSoftware.Api.Infra.Repo;

/// <summary>
/// Defines a repository for performing CRUD operations on a data source.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRepository<T>
{

    /// <summary>
    /// Gets all entities of type T from the data source.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity of type T by its ID from the data source. The ID is assumed to be an integer and the primary key of the entity.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity of type T by its RID (resource identifier) from the data source. The RID is a GUID that uniquely identifies the entity across different systems and contexts, and is used for external references and integration. This method allows for retrieving entities based on their RID instead of their internal ID, which can be useful for scenarios where the ID is not known or not relevant.
    /// </summary>
    /// <param name="rid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T?> GetByRidAsync(Guid rid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a new entity of type T into the data source.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task InsertAsync(T model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity of type T in the data source. The entity must already exist in the data source and have a valid ID.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateAsync(T model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing entity of type T from the data source. The entity must already exist in the data source and have a valid ID.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(T model, CancellationToken cancellationToken = default);
}