namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Defines the contract for a task service, which allows publishing tasks to the task worker.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Publishes a task to the task worker. This method takes a task of a specific type and an optional cancellation token, and it is responsible for adding the task to the task store or queue for later processing by the task worker. The task type must be registered in the task registry for it to be published successfully.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PublishAsync<T>(T task, CancellationToken cancellationToken = default) where T : notnull;
}