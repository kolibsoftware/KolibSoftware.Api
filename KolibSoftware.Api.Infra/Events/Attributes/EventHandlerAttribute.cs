namespace KolibSoftware.Api.Infra.Events.Attributes;

/// <summary>
/// Indicates that the decorated class or struct is an event handler.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class EventHandlerAttribute : Attribute;