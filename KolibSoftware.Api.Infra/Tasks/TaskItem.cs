using System.Text.Json;
using KolibSoftware.Api.Infra.Models;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Represents a task item that can be published to the task worker. This class encapsulates the task data and its dependencies, allowing for structured representation of tasks and their relationships. The Task property holds the actual task data, while the Dependencies property is a collection of other TaskItem instances that represent tasks that must be completed before this task can be executed. This structure enables the task worker to manage and execute tasks in the correct order based on their dependencies.
/// </summary>
/// <param name="task">The task data associated with this task item.</param>
public sealed class TaskItem
{

    /// <summary>
    /// Gets or sets the task data associated with this task item. This property holds the actual task information that will be processed by the task worker. The type of the task data can be any object, but it is typically expected to be a specific class that represents the task's purpose and parameters. The task type must be registered in the TaskRegistry for it to be published successfully, as the TaskRegistry is used to determine how to handle and execute the task based on its type.
    /// </summary>
    public object Task { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of task items that represent the dependencies for this task. These dependencies are other tasks that must be completed before this task can be executed. The task worker uses this information to determine the execution order of tasks, ensuring that all dependencies are resolved before processing the current task. This allows for complex workflows to be defined where tasks can depend on the completion of multiple other tasks, enabling coordinated execution of related tasks in the system.
    /// </summary>
    public ICollection<TaskItem> Dependencies { get; set; } = [];

    /// <summary>
    /// Converts this TaskItem instance into a TaskModel that can be published to the task worker. This method creates a new TaskModel instance, populating its properties based on the task data and dependencies of this TaskItem. The Name property of the TaskModel is determined by looking up the task type in the TaskRegistry, while the Data property is set to a serialized representation of the task data. The CreatedAt property is set to the current UTC time, and the Status is initialized to Pending. The Dependencies property of the TaskModel is populated by converting each dependent TaskItem into its corresponding TaskModel representation. This method allows for seamless conversion of structured task items into a format that can be stored and processed by the task worker.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public TaskModel ToModel()
    {
        var model = new TaskModel
        {
            Rid = Guid.CreateVersion7(),
            Name = TaskRegistry.GetTaskName(Task.GetType()) ?? throw new InvalidOperationException($"Task type {Task.GetType().FullName} is not registered in TaskRegistry."),
            Data = JsonSerializer.SerializeToNode(Task)!,
            CreatedAt = DateTime.UtcNow,
            Status = Models.TaskStatus.Pending,
            HandledAt = null,
            Dependencies = [.. Dependencies.Select(d => new TaskDependency { Dependency = d.ToModel() })]
        };
        return model;
    }
}