using Microsoft.EntityFrameworkCore;

namespace KolibSoftware.Api.Infra.Repo;

/// <summary>
/// A generic repository implementation using Entity Framework Core. This repository provides basic CRUD operations and pagination support. It can be used as a base class for more specific repositories that require additional methods or custom queries.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="dbContext"></param>
public class Repository<T>(DbContext dbContext) : IRepository<T> where T : class
{

    /// <summary>
    /// The DbContext instance used by this repository to access the data source. This property is protected, so it can be accessed by derived classes that need to perform custom queries or operations on the data source. The DbContext is injected into the repository through the constructor, which allows for better testability and separation of concerns.
    /// </summary>
    public DbContext DbContext => dbContext;

    /// <summary>
    /// The DbSet instance for the entity type T. This property is protected, so it can be accessed by derived classes that need to perform custom queries or operations on the data source. The DbSet is obtained from the DbContext using the Set<T>() method, which returns a DbSet instance that can be used to query and manipulate entities of type T in the data source.
    /// </summary>
    public DbSet<T> DbSet { get; } = dbContext.Set<T>();

    /// <summary>
    /// Gets all entities of type T from the data source. This method uses the DbSet to query the data source and return all entities of type T as a list. The method is asynchronous and supports cancellation through the CancellationToken parameter. If there are no entities of type T in the data source, this method will return an empty list.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await DbSet.ToListAsync(cancellationToken);
        return items;
    }

    /// <summary>
    /// Gets an entity of type T by its ID from the data source. This method uses the DbSet's FindAsync method to retrieve an entity by its primary key. The ID is assumed to be an integer and the primary key of the entity. The method is asynchronous and supports cancellation through the CancellationToken parameter. If there is no entity with the specified ID in the data source, this method will return null.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await DbSet.FindAsync([id], cancellationToken);
        return item;
    }

    /// <summary>
    /// Inserts a new entity of type T into the data source. This method uses the DbSet's AddAsync method to add a new entity to the data source, and then calls SaveChangesAsync on the DbContext to persist the changes. The method is asynchronous and supports cancellation through the CancellationToken parameter. After this method is called, the new entity will be saved in the data source and will have its primary key (ID) generated if it is an identity column.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task InsertAsync(T model, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(model, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an existing entity of type T in the data source. This method uses the DbSet's Update method to mark an existing entity as modified, and then calls SaveChangesAsync on the DbContext to persist the changes. The method is asynchronous and supports cancellation through the CancellationToken parameter. The entity must already exist in the data source and have a valid ID for this method to work correctly. After this method is called, the changes to the entity will be saved in the data source.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task UpdateAsync(T model, CancellationToken cancellationToken = default)
    {
        DbSet.Update(model);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes an existing entity of type T from the data source. This method uses the DbSet's Remove method to mark an existing entity for deletion, and then calls SaveChangesAsync on the DbContext to persist the changes. The method is asynchronous and supports cancellation through the CancellationToken parameter. The entity must already exist in the data source and have a valid ID for this method to work correctly. After this method is called, the entity will be deleted from the data source.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task DeleteAsync(T model, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(model);
        await DbContext.SaveChangesAsync(cancellationToken);
    }
}