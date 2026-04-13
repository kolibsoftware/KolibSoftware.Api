namespace KolibSoftware.Api.Infra.Events.Attributes;

/// <summary>
/// Base attribute for event handlers, allowing for different ways to specify the event name or type that the handler is associated with. This can be used for more flexible event handler registration and discovery in the event broker system.
/// </summary>
public abstract class BaseEventHandlerAttribute : Attribute
{
    /// <summary>
    /// The name of the event handler.
    /// </summary>
    public abstract string EventName { get; }
}

/// <summary>
/// Marks a class as an event handler for a specific event name. The name of the event can be specified using the constructor parameter, and must match the name of an event registered in the system for the handler to be invoked when that event is dispatched. This is used for storing and querying event handlers in the event broker.
/// </summary>
/// <param name="name"></param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class EventHandlerAttribute(string name) : BaseEventHandlerAttribute
{
    /// <summary>
    /// The name of the event that this handler is associated with. This must match the name of an event registered in the system for the handler to be invoked when that event is dispatched. If not specified, the name of the handler class or struct will be used as the event name, which may not be desirable in most cases.
    /// </summary>
    public override string EventName => name;
}

/// <summary>
/// Marks a class as an event handler for a specific event type. The event type is determined by the generic type parameter T, and the name of the event is derived from the event type using the EventRegistry. This allows for more type-safe and convention-based event handler registration and discovery in the event broker system, as the handler will be automatically associated with the correct event name based on the event type it handles.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class EventHandlerAttribute<T>() : BaseEventHandlerAttribute
{
    /// <summary>
    /// The name of the event that this handler is associated with, derived from the event type T using the EventRegistry. This allows for more type-safe and convention-based event handler registration and discovery in the event broker system, as the handler will be automatically associated with the correct event name based on the event type it handles. If the event type T is not registered in the EventRegistry, the name of the event will default to the name of the event type T, which may not be desirable in most cases.
    /// </summary>
    public override string EventName => EventRegistry.GetEventName(typeof(T)) ?? typeof(T).Name;
}