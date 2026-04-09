using KolibSoftware.Api.Infra.Models;

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
    /// <param name="dependencies">A collection of task models that the current task depends on. These tasks must be completed before the current task can be executed.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The created TaskModel representing the published task.</returns>
    Task<TaskModel> PublishAsync<T>(T task, IEnumerable<TaskModel> dependencies, CancellationToken cancellationToken = default) where T : notnull;
}