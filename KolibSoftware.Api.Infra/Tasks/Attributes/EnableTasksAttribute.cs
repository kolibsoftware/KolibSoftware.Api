namespace KolibSoftware.Api.Infra.Tasks.Attributes;

/// <summary>
/// Marks the assembly as containing tasks. This is required for the task system to work, as it will only scan assemblies with this attribute for task handlers and task definitions.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
public sealed class EnableTasksAttribute : Attribute;