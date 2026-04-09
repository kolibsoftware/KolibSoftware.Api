namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Settings for the event worker, which is responsible for dispatching events to handlers. The delay is the time to wait before dispatching an event, and the threshold is the maximum time to wait before dispatching an event. If an event is not dispatched within the threshold, it will be discarded.
/// </summary>
public sealed class EventWorkerSettings
{
    /// <summary>
    /// The delay to wait before dispatching an event. This allows for batching of events and reduces the number of dispatches. The default is 5 seconds.
    /// </summary>
    public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// The maximum time to wait before dispatching an event. If an event is not dispatched within this time, it will be discarded. The default is 5 minutes.
    /// </summary>
    public TimeSpan Threshold { get; set; } = TimeSpan.FromMinutes(5);
}