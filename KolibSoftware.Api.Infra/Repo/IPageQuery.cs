namespace KolibSoftware.Api.Infra.Repo;

public interface IPageQuery<T> : IQuery<T>
{
    public int PageNumber { get; }
    public int PageSize { get; }
}