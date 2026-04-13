using System.Text.Json;
using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Repo;

namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Implements the IEventService interface, providing functionality to publish events to the event broker. This service is responsible for creating event models based on the provided event data and storing them in the event store for later processing by the event worker. The PublishAsync method takes an event of a specific type, creates an EventModel instance with the appropriate properties, and saves it to the event store. The event type must be registered in the EventRegistry for it to be published successfully.
/// </summary>
/// <param name="repository"></param>
public class EventService(
    IRepository<EventModel> repository
) : IEventService
{

    /// <summary>
    /// Publishes an event model to the event store. This method takes an EventModel instance, which contains the event name, serialized event data, timestamps, and status, and saves it to the event store using the repository. The method is asynchronous and can be cancelled using a CancellationToken. Once the event is published, it will be available for processing by the event worker, which will dispatch it to the appropriate handlers based on the event name.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task PublishAsync(EventModel model, CancellationToken cancellationToken = default)
    {
        await repository.InsertAsync(model, cancellationToken);
    }
}