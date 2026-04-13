using System.Reflection;
using KolibSoftware.Api.Infra.Tasks.Attributes;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Static registry that discovers and caches all task handler types in assemblies marked with the EnableTasksAttribute, allowing efficient lookup of handlers for a given task type or vice versa.
/// </summary>
public static class TaskHandlerRegistry
{
    private static readonly Dictionary<string, Type> _handlerTypes;

    static TaskHandlerRegistry()
    {
        var pairs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetCustomAttribute<EnableTasksAttribute>() != null)
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface && t.GetCustomAttribute<TaskHandlerAttribute>() != null)
            .Select(t => new
            {
                Attribute = t.GetCustomAttribute<TaskHandlerAttribute>()!,
                Type = t
            });

        _handlerTypes = pairs.ToDictionary(p => p.Attribute.TaskName, p => p.Type);
    }

    /// <summary>
    /// Gets the string name of a task handler type based on the [TaskHandler] attribute, or returns null if the type is not registered as a task handler. This is used for storing and querying task handlers in the task broker.
    /// Note that task handler only can be registered for a single task name, so this method returns a single task name corresponding to the given handler type, or null if no match is found. For lookup of handler types corresponding to a given task name, use GetHandlerTypes(string taskName) instead.
    /// </summary>
    /// <param name="handlerType"></param>
    /// <returns></returns>
    public static string? GetTaskName(Type handlerType) => _handlerTypes.FirstOrDefault(kv => kv.Value == handlerType).Key;

    /// <summary>
    /// Gets the handler type that is registered to handle a specific task name, which can be used by the task broker service to dispatch tasks to the correct handlers.
    /// Note that task handler only can be registered for a single task name, so this method returns a single handler type corresponding to the given task name, or null if no match is found. For lookup of the task name corresponding to a given handler type, use GetTaskName(Type handlerType) instead.
    /// </summary>
    /// <param name="taskName"></param>
    /// <returns></returns>
    public static Type? GetHandlerType(string taskName) => _handlerTypes.GetValueOrDefault(taskName);

    /// <summary>
    /// Gets all registered task handler names in the system, which can be used for diagnostics, monitoring, or dynamic task handling scenarios.
    /// Note that task handler only can be registered for a single task name, so this method returns all task names corresponding to the registered handler types.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> GetTaskNames() => _handlerTypes.Keys;

    /// <summary>
    /// Gets all registered task handler types in the system, which can be used for diagnostics, monitoring, or dynamic task handling scenarios.
    /// Note that task handler only can be registered for a single task name, so this method returns a single task name corresponding to the given handler type, or null if no match is found. For lookup of handler types corresponding to a given task name, use GetHandlerType(string taskName) instead.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetHandlerTypes() => _handlerTypes.Values;

}
