using KolibSoftware.Api.Infra.Models;

namespace KolibSoftware.Api.Infra.Repo;

public static class RepoUtils
{

    public static async Task<T?> GetByRidAsync<T>(this IRepository<T> repository, Guid rid) where T : class, IResource
    {
        var model = await repository.GetAllAsync().ContinueWith(t => t.Result.FirstOrDefault(r => r.Rid == rid));
        return model;
    }

}