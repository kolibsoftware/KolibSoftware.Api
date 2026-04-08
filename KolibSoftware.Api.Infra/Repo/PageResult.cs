using Microsoft.EntityFrameworkCore;

namespace KolibSoftware.Api.Infra.Repo;

/// <summary>
/// A helper class to create paginated results from an IQueryable.
/// </summary>
public static class PageResult
{

    private class Result<T>(
        IEnumerable<T> pageItems,
        int itemCount,
        int pageNumber,
        int pageCount
    ) : IPageResult<T>
    {
        public IEnumerable<T> Items => pageItems;
        public int ItemCount => itemCount;
        public int PageNumber => pageNumber;
        public int PageCount => pageCount;

        public static readonly Result<T> Empty = new([], 0, 1, 0);
    }

    /// <summary>
    /// Creates an empty paginated result. This can be used when there are no items to return, but you still want to return pagination metadata (e.g. item count = 0, page number = 1, page count = 0).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IPageResult<T> Empty<T>() => Result<T>.Empty;

    /// <summary>
    /// Creates a paginated result from an IQueryable. This method will execute the query to get the total item count and the items for the requested page.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static IPageResult<T> Page<T>(this IQueryable<T> items, int pageNumber, int pageSize)
    {
        var itemCount = items.Count();
        var pageItems = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        var pageCount = (int)Math.Ceiling((double)itemCount / pageSize);
        return new Result<T>(pageItems, itemCount, pageNumber, pageCount);
    }

    /// <summary>
    /// Creates a paginated result from an IQueryable asynchronously. This method will execute the query to get the total item count and the items for the requested page.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IPageResult<T>> PageAsync<T>(this IQueryable<T> items, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var itemCount = await items.CountAsync(cancellationToken);
        var pageItems = await items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        var pageCount = (int)Math.Ceiling((double)itemCount / pageSize);
        return new Result<T>(pageItems, itemCount, pageNumber, pageCount);
    }

    /// <summary>
    /// Maps the items in the paginated result using the provided mapping function. This method will create a new paginated result with the mapped items, but will keep the same pagination metadata (item count, page number, page count).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pageResult"></param>
    /// <param name="mapFunc"></param>
    /// <returns></returns>
    public static IPageResult<U> Map<T, U>(this IPageResult<T> pageResult, Func<T, U> mapFunc)
    {
        var mappedItems = pageResult.Items.Select(mapFunc);
        return new Result<U>(mappedItems, pageResult.ItemCount, pageResult.PageNumber, pageResult.PageCount);
    }

}