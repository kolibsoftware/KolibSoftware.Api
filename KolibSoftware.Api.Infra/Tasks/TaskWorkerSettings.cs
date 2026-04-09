namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Settings for the task worker, which is responsible for dispatching tasks to handlers. The polling interval is the time to wait before dispatching a task. If a task is not dispatched within the polling interval, it will be discarded.
/// </summary>
public sealed class TaskWorkerSettings
{
    /// <summary>
    /// The interval to wait before dispatching a task. This allows for batching of tasks and reduces the number of dispatches. The default is 5 seconds.
    /// </summary>
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(5);
}