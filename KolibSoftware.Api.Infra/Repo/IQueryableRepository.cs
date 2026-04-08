namespace KolibSoftware.Api.Infra.Repo;

/// <summary>
/// A repository that supports querying with IQuery and IPageQuery.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IQueryableRepository<T> : IRepository<T>
{

    /// <summary>
    /// Gets all items that match the given query.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<T>> GetAllAsync(IQuery<T> query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a page of items that match the given query.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IPageResult<T>> GetPageAsync(IPageQuery<T> query, CancellationToken cancellationToken = default);
}