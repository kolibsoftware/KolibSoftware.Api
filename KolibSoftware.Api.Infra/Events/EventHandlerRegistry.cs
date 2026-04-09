using System.Reflection;
using KolibSoftware.Api.Infra.Events.Attributes;

namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Static registry that discovers and caches all event handler types in assemblies marked with the EnableEventsAttribute, allowing efficient lookup of handlers for a given event type or vice versa.
/// </summary>
public static class EventHandlerRegistry
{
    private static readonly Dictionary<Type, IEnumerable<Type>> _handlerTypes;
    private static readonly Dictionary<Type, IEnumerable<Type>> _typeHandlers;

    static EventHandlerRegistry()
    {
        var pairs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetCustomAttribute<EnableEventsAttribute>() != null)
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface && t.GetCustomAttribute<EventHandlerAttribute>() != null)
            .SelectMany(t => t.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .Select(i => new
                {
                    Handler = i,
                    Type = t
                })
            );

        _handlerTypes = pairs
            .GroupBy(p => p.Handler)
            .ToDictionary(g => g.Key, g => g.Select(p => p.Type));

        _typeHandlers = pairs
            .GroupBy(p => p.Type)
            .ToDictionary(g => g.Key, g => g.Select(p => p.Handler));
    }

    /// <summary>
    /// Gets all registered event handler types in the application, which can be used for diagnostics or dynamic registration in the dependency injection container.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetHandlerTypes() => _typeHandlers.Keys;

    /// <summary>
    /// Gets the event types that a given handler type is registered to handle, which can be used for diagnostics or dynamic registration in the dependency injection container.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetTypeHandlers() => _handlerTypes.Keys;

    /// <summary>
    /// Gets the handler types that are registered to handle a specific event type, which can be used by the event broker service to dispatch events to the correct handlers.
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetHandlerTypes(Type handler) => _handlerTypes.GetValueOrDefault(handler) ?? [];

    /// <summary>
    /// Gets the event types that a given handler type is registered to handle, which can be used by the event broker service to dispatch events to the correct handlers.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetTypeHandlers(Type type) => _typeHandlers.GetValueOrDefault(type) ?? [];

}
