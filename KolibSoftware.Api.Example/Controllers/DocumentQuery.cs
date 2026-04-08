using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Infra.Repo;

namespace KolibSoftware.Api.Example.Controllers;

public class DocumentQuery : IPageQuery<DocumentModel>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? Hint { get; set; }

    public IQueryable<DocumentModel> Apply(IQueryable<DocumentModel> queryable)
    {
        if (!string.IsNullOrEmpty(Hint))
            queryable = queryable.Where(d => d.Title.Contains(Hint) || d.Content.Contains(Hint));
        return queryable;
    }
}