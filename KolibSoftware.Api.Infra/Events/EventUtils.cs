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
        builder.Services.Configure<EventWorkerSettings>(builder.Configuration.GetSection("EventWorker"));
        builder.Services.AddTransient<IEventService, EventService>();
        builder.Services.AddHostedService<EventWorker>();
        var types = EventHandlerRegistry.GetHandlerTypes();
        foreach (var type in types)
        {
            var handlers = EventHandlerRegistry.GetTypeHandlers(type);
            foreach (var handler in handlers)
                builder.Services.AddTransient(handler, type);
        }
        return builder;
    }
}