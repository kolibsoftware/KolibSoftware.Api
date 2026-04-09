namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Defines the contract for an event broker context, which allows publishing events to the event broker.
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Publishes an event to the event broker. This method takes an event of a specific type and an optional cancellation token, and it is responsible for adding the event to the event store or queue for later processing by the event worker. The event type must be registered in the event registry for it to be published successfully.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : notnull;
}