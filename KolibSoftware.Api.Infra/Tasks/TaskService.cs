using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Repo;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Implements the ITaskService interface, providing functionality to publish tasks to the task worker. This service is responsible for creating task models based on the provided task data and storing them in the task store for later processing by the task worker. The PublishAsync method takes a task of a specific type, creates a TaskModel instance with the appropriate properties, and saves it to the task store. The task type must be registered in the TaskRegistry for it to be published successfully.
/// </summary>
/// <param name="repository"></param>
public class TaskService(
    IRepository<TaskModel> repository
) : ITaskService
{

    /// <summary>
    /// Publishes a task model to the task store. This method takes a TaskModel instance, which contains the task name, serialized task data, timestamps, status, and dependencies, and saves it to the task store using the repository. The method is asynchronous and can be cancelled using a CancellationToken. Once the task is published, it will be available for processing by the task worker, which will execute it based on its dependencies and status.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task PublishAsync(TaskModel model, CancellationToken cancellationToken = default)
    {
        await repository.InsertAsync(model, cancellationToken);
    }
}