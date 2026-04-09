using System.Text.Json;
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
    /// Publishes a task to the task worker. This method takes a task of a specific type and an optional cancellation token, and it is responsible for adding the task to the task store or queue for later processing by the task worker. The task type must be registered in the task registry for it to be published successfully.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <param name="dependencies">A collection of task models that the current task depends on. These tasks must be completed before the current task can be executed.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The created TaskModel representing the published task.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<TaskModel> PublishAsync<T>(T @task, IEnumerable<TaskModel> dependencies, CancellationToken cancellationToken = default) where T : notnull
    {
        var taskName = TaskRegistry.GetTaskName(typeof(T)) ?? throw new InvalidOperationException($"Task type {typeof(T).FullName} is not registered in TaskRegistry.");
        var _task = new TaskModel
        {
            Rid = Guid.CreateVersion7(),
            Name = taskName,
            Data = JsonSerializer.SerializeToNode(@task)!,
            CreatedAt = DateTime.UtcNow,
            Status = Models.TaskStatus.Pending,
            HandledAt = null,
            Dependencies = [.. dependencies.Select(x => new TaskDependency { DependencyId = x.Id })]
        };
        await repository.InsertAsync(_task, cancellationToken);
        return _task;
    }
}