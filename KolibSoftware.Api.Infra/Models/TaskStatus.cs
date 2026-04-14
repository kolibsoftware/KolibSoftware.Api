namespace KolibSoftware.Api.Infra.Models;

/// <summary>
/// Represents the status of a task in the task processing lifecycle. This enum is used to indicate the current state of a task, such as whether it is ready, pending, successfully completed, cancelled, or if it has failed during processing. Each status provides insight into the progress and outcome of a task within the system.
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// The task is pending on the completion of one or more dependent tasks before it can be processed. This status indicates that the task cannot proceed until the specified dependencies have been successfully completed, allowing for coordination and sequencing of tasks based on their interdependencies.
    /// </summary>
    Pending,

    /// <summary>
    /// The task was successfully handled by the registered handler without any errors. It has been processed and can be considered complete.
    /// </summary>
    Completed,

    /// <summary>
    /// The task was cancelled before it could be handled, indicating that it will not be processed by any handlers and should be removed from the task store. This can occur if the task is no longer relevant or if it was cancelled by the user or system before processing.
    /// </summary>
    Cancelled,

    /// <summary>
    /// The task handling failed, indicating that the handler encountered errors while processing the task. It may require manual intervention or retries to resolve the issues and successfully handle the task.
    /// </summary>
    Failed,
}