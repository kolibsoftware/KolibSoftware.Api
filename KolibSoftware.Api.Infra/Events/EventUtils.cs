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
}