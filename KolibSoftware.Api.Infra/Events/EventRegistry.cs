using System.Reflection;
using KolibSoftware.Api.Infra.Events.Attributes;

namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Static registry that discovers and maps event types to their string names based on the presence of the [Event] attribute in assemblies marked with [EnableEvents].
/// It provides methods to retrieve event names and types, facilitating event handling and dispatching in the event broker system.
/// </summary>
public static class EventRegistry
{
    private static readonly Dictionary<string, Type> _eventTypes;

    static EventRegistry()
    {
        var pairs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetCustomAttribute<EnableEventsAttribute>() != null)
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface && t.GetCustomAttribute<EventAttribute>() != null)
            .Select(t => new
            {
                Attribute = t.GetCustomAttribute<EventAttribute>()!,
                Type = t
            });
        _eventTypes = pairs.ToDictionary(p => p.Attribute.EventName ?? p.Type.Name, p => p.Type);
    }

    /// <summary>
    /// Gets the string name of an event type based on the [Event] attribute, or returns null if the type is not registered as an event. This is used for storing and querying events in the event store.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string? GetEventName(Type type) => _eventTypes.FirstOrDefault(kv => kv.Value == type).Key;

    /// <summary>
    /// Gets the event type corresponding to a given string name based on the [Event] attribute, or returns null if the name is not registered as an event. This is used for deserializing and dispatching events from the event store.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Type? GetEventType(string name) => _eventTypes.GetValueOrDefault(name);

    /// <summary>
    /// Gets all registered event names and types in the system, which can be used for diagnostics, monitoring, or dynamic event handling scenarios.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> GetEventNames() => _eventTypes.Keys;

    /// <summary>
    /// Gets all registered event types and their corresponding string names in the system, which can be used for diagnostics, monitoring, or dynamic event handling scenarios.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetEventTypes() => _eventTypes.Values;
}