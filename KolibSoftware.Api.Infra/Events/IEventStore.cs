using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Repo;

namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Defines the contract for an event store, which is responsible for persisting and retrieving events. The event store allows for storing events asynchronously and querying them based on specific criteria defined by the IEventQuery interface.
/// </summary>
public interface IEventStore
{

    /// <summary>
    /// Asynchronously retrieves events from the event store based on a specified query. The method takes an IEventQuery object, which defines the criteria for filtering events, and a CancellationToken for handling cancellation requests. It returns an IEnumerable of Event objects that match the query criteria, allowing for efficient retrieval of relevant events from the store.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<EventModel>> GetEventsAsync(IQuery<EventModel> query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously stores multiple events in the event store. The method takes an IEnumerable of Event objects and a CancellationToken for handling cancellation requests. This allows for efficient batch storage of events, which can be useful in scenarios where multiple events need to be stored at once, reducing the overhead of individual storage operations.
    /// </summary>
    /// <param name="events"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PutEventsAsync(IEnumerable<EventModel> events, CancellationToken cancellationToken = default);
}