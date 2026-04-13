using KolibSoftware.Api.Infra.Models;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Defines the contract for a task service, which allows publishing tasks to the task worker.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Publishes a task model to the task store. This method takes a TaskModel instance, which contains the task name, serialized task data, timestamps, status, and dependencies, and saves it to the task store using the repository. The method is asynchronous and can be cancelled using a CancellationToken. Once the task is published, it will be available for processing by the task worker, which will execute it based on its dependencies and status.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PublishAsync(TaskModel model, CancellationToken cancellationToken = default);
}