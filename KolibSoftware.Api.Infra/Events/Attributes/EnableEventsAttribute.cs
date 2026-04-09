namespace KolibSoftware.Api.Infra.Events.Attributes;

/// <summary>
/// Marks the assembly as containing events. This is required for the event system to work, as it will only scan assemblies with this attribute for event handlers and event definitions.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
public sealed class EnableEventsAttribute : Attribute;