using System.Text.Json.Nodes;

namespace KolibSoftware.Api.Infra.Models;

/// <summary>
/// Represents a task in the task system, containing information about the task's unique identifier, creation time, name, data payload, and current status. This model is used to store and manage tasks within the task broker service, allowing for tracking and processing of tasks as they are handled by registered handlers.
/// </summary>
public class TaskModel : IResource
{
    /// <summary>
    /// The unique identifier for the task in the task store. This is typically an auto-incrementing integer that serves as the primary key for the task record in the database. It is used internally by the task store to manage and retrieve tasks, but it is not exposed to external consumers of the task model.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The unique identifier for the task. This should be a GUID that is generated when the task is created and should not change throughout the lifetime of the task. The repository or service layer should handle setting this property during a create operation.
    /// </summary>
    public Guid Rid { get; set; }

    /// <summary>
    /// The timestamp indicating when the task was created. This should be set to the current UTC time when the task is created and should not change throughout the lifetime of the task.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The name of the task, which corresponds to the type of task being created. This should be set based on the task type and should be used by handlers to determine which tasks they are interested in processing.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// The data payload of the task, which contains the information relevant to the task being created. This should be stored as a JSON object in the database, and it can contain any structured data that is necessary for handlers to process the task. The repository or service layer should handle serializing the task data to JSON when saving it to the database and deserializing it when retrieving it for processing by handlers.
    /// </summary>
    public JsonNode Data { get; set; } = default!;

    /// <summary>
    /// The timestamp indicating when the event was handled by all registered handlers. This should be set to the current UTC time when all handlers have successfully processed the event, and it can be used for tracking and auditing purposes.
    /// </summary>
    public DateTime? HandledAt { get; set; }

    /// <summary>
    /// The status of the task, indicating whether it is pending, successfully handled, partially handled, or failed. This should be updated based on the outcome of processing by handlers and can be used for monitoring and retrying failed tasks.
    /// </summary>
    public TaskStatus Status { get; set; }

    /// <summary>
    /// The collection of dependencies for the task, indicating other tasks that must be completed before this task can be executed. This allows for defining complex workflows and ensuring that tasks are executed in the correct order based on their dependencies.
    /// </summary>
    public ICollection<TaskDependency> Dependencies { get; set; } = [];
}