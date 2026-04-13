namespace KolibSoftware.Api.Infra.Tasks.Attributes;

/// <summary>
/// Marks a class or struct as a task. The name of the task can be specified using the constructor parameter, otherwise the name of the class or struct will be used as the task name.
/// </summary>
/// <param name="taskName"></param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class TaskAttribute(string? taskName = null) : Attribute
{
    /// <summary>
    /// The name of the task. If not specified, the name of the class or struct will be used as the task name.
    /// </summary>
    public string? TaskName => taskName;
}
