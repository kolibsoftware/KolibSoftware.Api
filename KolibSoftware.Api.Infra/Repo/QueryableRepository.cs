using Microsoft.EntityFrameworkCore;

namespace KolibSoftware.Api.Infra.Repo;

/// <summary>
/// A repository that supports querying with IQuery and IPageQuery.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="dbContext"></param>
public class QueryableRepository<T>(DbContext dbContext) : Repository<T>(dbContext), IQueryableRepository<T> where T : class
{

    /// <summary>
    /// Gets all items that match the given query.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<T>> GetAllAsync(IQuery<T> query, CancellationToken cancellationToken = default)
    {
        var items = await query.Apply(DbSet).ToListAsync(cancellationToken);
        return items;
    }

    /// <summary>
    /// Gets a page of items that match the given query.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IPageResult<T>> GetPageAsync(IPageQuery<T> query, CancellationToken cancellationToken = default)
    {
        var result = await query.Apply(DbSet).PageAsync(query.PageNumber, query.PageSize, cancellationToken);
        return result;
    }
}