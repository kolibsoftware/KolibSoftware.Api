using System.Text.Json;
using System.Text.Json.Nodes;

namespace KolibSoftware.Api.Infra.Models;

/// <summary>
/// Represents an event that can be published to the event broker. This model contains properties that describe the event, such as its unique identifier (Rid), name, creation timestamp, data payload, handling status, and the timestamp when it was handled. The EventModel is used by the event service to store and manage events in the event store, and it is processed by the event worker to execute registered handlers based on the event type and status.
/// </summary>
public class EventModel : IResource
{

    /// <summary>
    /// The unique identifier for the event in the event store. This is typically an auto-incrementing integer that serves as the primary key for the event record in the database. It is used internally by the event store to manage and retrieve events, but it is not exposed to external consumers of the event model.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The unique identifier for the event. This should be a GUID that is generated when the event is created and should not change throughout the lifetime of the event. The repository or service layer should handle setting this property during a create operation.
    /// </summary>
    public Guid Rid { get; set; }

    /// <summary>
    /// The timestamp indicating when the event was created. This should be set to the current UTC time when the event is created and should not change throughout the lifetime of the event.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The name of the event, which corresponds to the type of event being published. This should be set based on the event type and should be used by handlers to determine which events they are interested in processing.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// The data payload of the event, which contains the information relevant to the event being published. This should be stored as a JSON object in the database, and it can contain any structured data that is necessary for handlers to process the event. The repository or service layer should handle serializing the event data to JSON when saving it to the database and deserializing it when retrieving it for processing by handlers.
    /// </summary>
    public JsonNode Data { get; set; } = default!;

    /// <summary>
    /// The timestamp indicating when the event was handled by all registered handlers. This should be set to the current UTC time when all handlers have successfully processed the event, and it can be used for tracking and auditing purposes.
    /// </summary>
    public DateTime? HandledAt { get; set; }

    /// <summary>
    /// The status of the event, indicating whether it is pending, successfully handled, partially handled, or failed. This should be updated based on the outcome of processing by handlers and can be used for monitoring and retrying failed events.
    /// </summary>
    public EventStatus Status { get; set; }
}