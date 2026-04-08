namespace KolibSoftware.Api.Infra.Repo;

/// <summary>
/// Represents a paginated query that can be applied to an IQueryable. This interface extends IQuery with pagination parameters (page number and page size). The repository will use these parameters to return a paginated result when the query is executed.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPageQuery<T> : IQuery<T>
{

    /// <summary>
    /// The page number to return. This should be a positive integer (1-based index). The repository will use this parameter to determine which page of results to return when the query is executed.
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// The page size to return. This should be a positive integer. The repository will use this parameter to determine how many items to return in the page when the query is executed.
    /// </summary>
    public int PageSize { get; }
}