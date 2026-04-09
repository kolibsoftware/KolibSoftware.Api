namespace KolibSoftware.Api.Infra.Models;

/// <summary>
/// Enumeration representing the status of a task in the task system, indicating whether it is pending, successfully handled, partially handled, or failed.
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// The task is pending and has not yet been dispatched to the handler. It is waiting to be processed by the task broker service.
    /// </summary>
    Pending,

    /// <summary>
    /// The task was successfully handled by the registered handler without any errors. It has been processed and can be considered complete.
    /// </summary>
    Success,

    /// <summary>
    /// Thet task was cancelled before it could be handled, indicating that it will not be processed by any handlers and should be removed from the task store. This can occur if the task is no longer relevant or if it was cancelled by the user or system before processing.
    /// </summary>
    Cancelled,

    /// <summary>
    /// The task handling failed, indicating that the handler encountered errors while processing the task. It may require manual intervention or retries to resolve the issues and successfully handle the task.
    /// </summary>
    Failure,
}