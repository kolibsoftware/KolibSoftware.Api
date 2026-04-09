namespace KolibSoftware.Api.Infra.Tasks.Attributes;

/// <summary>
/// Indicates that the decorated class or struct is a task handler.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class TaskHandlerAttribute : Attribute;