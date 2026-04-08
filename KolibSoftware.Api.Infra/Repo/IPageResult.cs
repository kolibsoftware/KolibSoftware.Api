namespace KolibSoftware.Api.Infra.Repo;

/// <summary>
/// Defines a paginated result of items of type T. This is used to return paginated results from a repository method, along with metadata about the total item count, page number, and page count.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPageResult<T>
{

    /// <summary>
    /// Gets the items for the current page. This is a subset of the total items that match the query, based on the requested page number and page size.
    /// </summary>
    IEnumerable<T> Items { get; }

    /// <summary>
    /// Gets the total item count for the query. This is the total number of items that match the query, regardless of pagination. This value is used to calculate the total page count and to provide metadata about the result set.
    /// </summary>
    int ItemCount { get; }

    /// <summary>
    /// Gets the current page number. This is the page number that was requested in the query. The first page is typically page number 1. This value is used to provide metadata about the result set and to indicate which page of results is being returned.
    /// </summary>
    int PageNumber { get; }

    /// <summary>
    /// Gets the total page count for the query. This is calculated based on the total item count and the page size. This value is used to provide metadata about the result set and to indicate how many pages of results are available for the query.
    /// </summary>
    int PageCount { get; }
}