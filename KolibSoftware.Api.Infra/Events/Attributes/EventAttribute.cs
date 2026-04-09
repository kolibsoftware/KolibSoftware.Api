namespace KolibSoftware.Api.Infra.Events.Attributes;

/// <summary>
/// Marks a class or struct as an event. The name of the event can be specified using the constructor parameter, otherwise the name of the class or struct will be used as the event name.
/// </summary>
/// <param name="name"></param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class EventAttribute(string? name = null) : Attribute
{
    /// <summary>
    /// The name of the event. If not specified, the name of the class or struct will be used as the event name.
    /// </summary>
    public string? Name { get; } = name;
}
