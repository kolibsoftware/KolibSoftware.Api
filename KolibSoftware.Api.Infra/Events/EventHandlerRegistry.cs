using System.Reflection;
using KolibSoftware.Api.Infra.Events.Attributes;

namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Static registry that discovers and caches all event handler types in assemblies marked with the EnableEventsAttribute, allowing efficient lookup of handlers for a given event type or vice versa.
/// </summary>
public static class EventHandlerRegistry
{
    private static readonly Dictionary<string, IEnumerable<Type>> _handlerTypes;

    static EventHandlerRegistry()
    {
        var pairs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetCustomAttribute<EnableEventsAttribute>() != null)
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface && t.GetCustomAttribute<BaseEventHandlerAttribute>() != null && t.IsAssignableTo(typeof(IEventHandler)))
            .Select(t => new
            {
                Attribute = t.GetCustomAttribute<BaseEventHandlerAttribute>()!,
                Type = t
            });

        _handlerTypes = pairs.GroupBy(p => p.Attribute.EventName)
            .ToDictionary(g => g.Key, g => g.Select(p => p.Type));

    }

    /// <summary>
    /// Gets the string name of an event handler type based on the [EventHandler] attribute, or returns null if the type is not registered as an event handler. This is used for storing and querying event handlers in the event broker.
    /// Note that event handler only can be registered for a single event name, so this method returns a single event name corresponding to the given handler type, or null if no match is found. For lookup of handler types corresponding to a given event name, use GetHandlerTypes(string eventName) instead.
    /// </summary>
    /// <param name="handlerType"></param>
    /// <returns></returns>
    public static string? GetEventName(Type handlerType) => _handlerTypes.FirstOrDefault(kv => kv.Value.Contains(handlerType)).Key;


    /// <summary>
    /// Gets the event handler types corresponding to a given string event name based on the [EventHandler] attribute, or returns an empty enumerable if the name is not registered as an event handler. This is used for looking up and invoking event handlers for a given event name in the event broker.
    /// Note that multiple event handler types may be registered for the same event name, so this method returns all matching handler types, or an empty enumerable if no match is found. For lookup of the event name corresponding to a given handler type, use GetEventName(Type handlerType) instead.
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetHandlerTypes(string eventName) => _handlerTypes.GetValueOrDefault(eventName) ?? [];

    /// <summary>
    /// Gets all registered event handler names in the system, which can be used for diagnostics, monitoring, or dynamic event handling scenarios.
    /// Note that multiple handler types may be registered for the same event name, so this method returns distinct event names corresponding to registered handlers.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> GetEventNames() => _handlerTypes.Keys;

    /// <summary>
    /// Gets all registered event handler types in the system, which can be used for diagnostics, monitoring, or dynamic event handling scenarios.
    /// Note that event handler only can be registered for a single event name, so this method returns a single event name corresponding to the given handler type, or null if no match is found. For lookup of handler types corresponding to a given event name, use GetHandlerTypes(string eventName) instead.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetHandlerTypes() => _handlerTypes.Values.SelectMany(v => v);
}
