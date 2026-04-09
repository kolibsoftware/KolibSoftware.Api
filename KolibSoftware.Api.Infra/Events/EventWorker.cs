using System.Text.Json;
using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Repo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Background service that periodically checks for pending events in the event store and dispatches them to registered handlers.
/// </summary>
/// <param name="serviceProvider"></param>
/// <param name="options"></param>
/// <param name="logger"></param>
public sealed class EventWorker(
    IServiceProvider serviceProvider,
    IOptions<EventWorkerSettings> options,
    ILogger<EventWorker> logger
) : BackgroundService(), IEventWorker
{

    /// <summary>
    /// Settings for the event worker, which is responsible for dispatching events to handlers. The delay is the time to wait before dispatching an event, and the threshold is the maximum time to wait before dispatching an event. If an event is not dispatched within the threshold, it will be discarded.
    /// </summary>
    private readonly TimeSpan Delay = options.Value.Delay;

    /// <summary>
    /// Settings for the event worker, which is responsible for dispatching events to handlers. The delay is the time to wait before dispatching an event, and the threshold is the maximum time to wait before dispatching an event. If an event is not dispatched within the threshold, it will be discarded.
    /// </summary>
    private readonly TimeSpan Threshold = options.Value.Threshold;

    /// <summary>
    /// Executes the background service, which periodically retrieves pending events from the event store and dispatches them to the appropriate handlers. The service uses a periodic timer to control the frequency of event retrieval and dispatching, and it handles any exceptions that occur during the process by logging them. The method continues to run until the service is stopped, at which point it will gracefully exit.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(Delay);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IQueryableRepository<EventModel>>();
            var timepoint = DateTime.UtcNow.Subtract(Threshold);
            var query = new EventQuery(Threshold);
            var events = await repository.GetAllAsync(query, stoppingToken);
            if (events.Any())
                foreach (var @event in events)
                {
                    try
                    {
                        @event.Status = await DispatchEvent(@event, scope.ServiceProvider, stoppingToken);
                        @event.HandledAt = DateTime.UtcNow;
                        await repository.UpdateAsync(@event, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogErrorDispatchingEvents(ex);
                    }
                }
        }
    }

    /// <summary>
    /// Dispatches a single event to the appropriate handlers based on the event type. The method retrieves the event type from the event registry, finds all registered handlers for that event type, and invokes each handler's HandleEventAsync method with the event data. The method returns an EventStatus indicating whether the dispatch was successful, partially successful, or a failure, based on the number of handlers that successfully processed the event. If the event type is not registered or if no handlers are found, it logs the issue and returns a failure status.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<EventStatus> DispatchEvent(EventModel @event, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var eventType = EventRegistry.GetEventType(@event.Name);
        if (eventType == null)
        {
            logger.LogUnregisteredEventType(@event.Name);
            return EventStatus.Failure;
        }

        var data = @event.Data.Deserialize(eventType);
        if (data == null)
        {
            logger.LogFailedToDeserializeEventData(@event.Name, eventType.FullName!);
            return EventStatus.Failure;
        }

        var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
        var handlers = serviceProvider.GetServices(handlerType).Cast<IEventHandler>().ToList();
        if (handlers.Count == 0)
        {
            logger.LogNoHandlersFound(@event.Name);
            return EventStatus.Failure;
        }

        var successCount = 0;
        foreach (IEventHandler handler in handlers!)
        {
            try
            {
                await handler.HandleEventAsync(data, cancellationToken);
                successCount++;
            }
            catch (Exception ex)
            {
                var handlerTypeName = (handler as object)?.GetType().FullName ?? "UnknownHandler";
                logger.LogErrorHandlingEvent(@event.Name, handlerTypeName, ex);
            }
        }

        return successCount == handlers.Count
            ? EventStatus.Success
            : successCount > 0
                ? EventStatus.Partial
                : EventStatus.Failure;

    }

    /// <summary>
    /// Query that selects pending events that are older than a specified age threshold, to be dispatched by the event broker service.
    /// </summary>
    /// <param name="age"></param>
    public sealed class EventQuery(TimeSpan age) : IQuery<EventModel>
    {
        public IQueryable<EventModel> Apply(IQueryable<EventModel> query)
        {
            var timepoint = DateTime.UtcNow.Subtract(age);
            return query.Where(e => e.Status == EventStatus.Pending);
        }
    }
}