namespace KolibSoftware.Api.Infra.Tasks.Attributes;

/// <summary>
/// Indicates that the decorated class is a task handler.
/// </summary>
public abstract class BaseTaskHandlerAttribute : Attribute
{
    /// <summary>
    /// The name of the task that this handler handles. This is required for the task system to work, as it will use this name to match task handlers with task definitions.
    /// </summary>
    public abstract string TaskName { get; }
}

/// <summary>
/// Indicates that the decorated class is a task handler for the specified task name.
/// </summary>
/// <param name="taskName"></param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class TaskHandlerAttribute(string taskName) : BaseTaskHandlerAttribute
{
    /// <summary>
    /// The name of the task that this handler handles. This is required for the task system to work, as it will use this name to match task handlers with task definitions.
    /// </summary>
    public override string TaskName => taskName;
}


[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class TaskHandlerAttribute<T>() : BaseTaskHandlerAttribute
{
    public override string TaskName => TaskHandlerRegistry.GetTaskName(typeof(T)) ?? typeof(T).Name;
}
