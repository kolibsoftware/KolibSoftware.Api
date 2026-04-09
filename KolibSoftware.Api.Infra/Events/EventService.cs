using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using KolibSoftware.Api.Infra.Models;

namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Implements the IEventService interface, providing functionality to publish events to the event broker. This service is responsible for creating event models based on the provided event data and storing them in the event store for later processing by the event worker. The PublishAsync method takes an event of a specific type, creates an EventModel instance with the appropriate properties, and saves it to the event store. The event type must be registered in the EventRegistry for it to be published successfully.
/// </summary>
/// <param name="eventStore"></param>
public class EventService(
    IEventStore eventStore
) : IEventService
{

    /// <summary>
    /// Publishes an event to the event broker. This method takes an event of a specific type and an optional cancellation token, and it is responsible for adding the event to the event store or queue for later processing by the event worker. The event type must be registered in the event registry for it to be published successfully.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : notnull
    {
        var eventName = EventRegistry.GetEventName(typeof(T)) ?? throw new InvalidOperationException($"Event type {typeof(T).FullName} is not registered in EventRegistry.");
        var _event = new EventModel
        {
            Rid = Guid.CreateVersion7(),
            Name = eventName,
            Data = @event,
            CreatedAt = DateTime.UtcNow,
            Status = EventStatus.Pending,
            HandledAt = null
        };
        await eventStore.PutEventsAsync([_event], cancellationToken);
    }
}