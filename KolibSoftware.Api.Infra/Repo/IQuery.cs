namespace KolibSoftware.Api.Infra.Repo;

/// <summary>
/// Represents a query that can be applied to an IQueryable.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IQuery<T>
{

    /// <summary>
    /// Applies the query to the given IQueryable. This method should not execute the query, but rather return a new IQueryable that represents the query. The repository will then execute the query when it is ready to get the results.
    /// </summary>
    /// <param name="queryable"></param>
    /// <returns></returns>
    IQueryable<T> Apply(IQueryable<T> queryable);

}