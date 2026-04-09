namespace KolibSoftware.Api.Infra.Models;

/// <summary>
/// Represents an event that can be published to the event broker. This model contains properties that describe the event, such as its unique identifier (Rid), name, creation timestamp, data payload, handling status, and the timestamp when it was handled. The EventModel is used by the event service to store and manage events in the event store, and it is processed by the event worker to execute registered handlers based on the event type and status.
/// </summary>
public class EventModel
{

    /// <summary>
    /// The unique identifier for the event in the event store. This is typically an auto-incrementing integer that serves as the primary key for the event record in the database. It is used internally by the event store to manage and retrieve events, but it is not exposed to external consumers of the event model.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// The unique identifier for the event. This should be a GUID that is generated when the event is created and should not change throughout the lifetime of the event. The repository or service layer should handle setting this property during a create operation.
    /// </summary>
    public Guid Rid { get; init; }

    /// <summary>
    /// The timestamp indicating when the event was created. This should be set to the current UTC time when the event is created and should not change throughout the lifetime of the event.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// The name of the event, which corresponds to the type of event being published. This should be set based on the event type and should be used by handlers to determine which events they are interested in processing.
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// The data associated with the event, which can be any object that contains relevant information for handlers to process the event. This should be set based on the specific event being published and can contain any necessary details for handlers to perform their work.
    /// </summary>
    public object Data { get; init; } = null!;

    /// <summary>
    /// The timestamp indicating when the event was handled by all registered handlers. This should be set to the current UTC time when all handlers have successfully processed the event, and it can be used for tracking and auditing purposes.
    /// </summary>
    public DateTime? HandledAt { get; set; }

    /// <summary>
    /// The status of the event, indicating whether it is pending, successfully handled, partially handled, or failed. This should be updated based on the outcome of processing by handlers and can be used for monitoring and retrying failed events.
    /// </summary>
    public EventStatus Status { get; set; }
}