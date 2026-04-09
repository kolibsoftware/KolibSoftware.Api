namespace KolibSoftware.Api.Infra.Models;

/// <summary>
/// Represents a dependency between two tasks in the task system, where one task (the parent) depends on the completion of another task (the child) before it can be executed. This allows for defining complex workflows and ensuring that tasks are executed in the correct order based on their dependencies.
/// </summary>
public class TaskDependency
{
    /// <summary>
    /// The unique identifier for the task dependency in the task store. This is typically an auto-incrementing integer that serves as the primary key for the task dependency record in the database. It is used internally by the task store to manage and retrieve task dependencies, but it is not exposed to external consumers of the task dependency model.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The unique identifier of the child task that the parent task depends on. This indicates that the child task must be completed successfully before the parent task can be executed.
    /// </summary>
    public int DependentId { get; set; }

    /// <summary>
    /// The unique identifier of the parent task that has a dependency on the child task. This indicates that the parent task cannot be executed until the child task has been completed successfully.
    /// </summary>
    public int DependencyId { get; set; }
}