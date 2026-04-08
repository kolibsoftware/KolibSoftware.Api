namespace KolibSoftware.Api.Infra.Repo;

public interface IQuery<T>
{
    IQueryable<T> Apply(IQueryable<T> queryable);
}