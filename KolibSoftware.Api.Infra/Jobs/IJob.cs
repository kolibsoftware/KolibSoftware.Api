namespace KolibSoftware.Api.Infra.Jobs;

/// <summary>
/// Defines a contract for a job that can be executed by the job service.
/// </summary>
public interface IJob
{
    /// <summary>
    /// Executes the job asynchronously.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}