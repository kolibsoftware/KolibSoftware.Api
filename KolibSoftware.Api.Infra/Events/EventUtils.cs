using System.Text.Json;
using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Repo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Utility class that provides extension methods for configuring and using the event broker services and handlers in the application host builder. This class includes methods to add event broker services, discover and register event handler types, and define queries for selecting pending events to be dispatched by the event broker service.
/// </summary>
public static class EventUtils
{

    /// <summary>
    /// Extension method to add event broker services and handlers to the application host builder. This method configures the event broker settings, registers the event broker context and service, and discovers and registers all event handler types in the application assembly.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddEvents(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IRepository<EventModel>, Repository<EventModel>>();
        builder.Services.AddScoped<IQueryableRepository<EventModel>, QueryableRepository<EventModel>>();
        builder.Services.AddScoped<IEventService, EventService>();
        builder.Services.Configure<EventWorkerSettings>(builder.Configuration.GetSection("EventWorker"));
        builder.Services.AddHostedService<EventWorker>();
        var types = EventRegistry.GetEventTypes();
        foreach (var type in types)
        {
            var eventName = EventRegistry.GetEventName(type) ?? throw new InvalidOperationException($"Event type {type.FullName} does not have a registered event name");
            var handlers = EventHandlerRegistry.GetHandlerTypes(eventName);
            foreach (var handler in handlers)
                builder.Services.AddKeyedScoped(typeof(IEventHandler), eventName, handler);
        }
        return builder;
    }

    /// <summary>
    /// Publishes an event of a specific type to the event broker. This method creates an EventModel instance with the appropriate properties, including the event name, serialized event data, and timestamps, and then calls the PublishAsync method of the IEventService to store the event in the event store. The event type must be registered in the EventRegistry for it to be published successfully.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventService"></param>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<EventModel> PublishAsync<T>(this IEventService eventService, T @event, CancellationToken cancellationToken = default) where T : notnull
    {
        var model = new EventModel
        {
            Rid = Guid.CreateVersion7(),
            Name = EventRegistry.GetEventName(typeof(T)) ?? throw new InvalidOperationException($"Event type {typeof(T).FullName} is not registered in EventRegistry."),
            Data = JsonSerializer.SerializeToNode(@event)!,
            CreatedAt = DateTime.UtcNow,
            Status = EventStatus.Pending,
            HandledAt = null
        };
        await eventService.PublishAsync(model, cancellationToken);
        return model;
    }
}