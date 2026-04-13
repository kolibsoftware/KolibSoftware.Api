using System.Reflection;
using KolibSoftware.Api.Infra.Tasks.Attributes;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Static registry that discovers and maps task types to their string names based on the presence of the [Task] attribute in assemblies marked with [EnableTasks].
/// It provides methods to retrieve task names and types, facilitating task handling and dispatching in the task broker system.
/// </summary>
public static class TaskRegistry
{
    private static readonly Dictionary<string, Type> _taskTypes;

    static TaskRegistry()
    {
        var pairs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetCustomAttribute<EnableTasksAttribute>() != null)
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface && t.GetCustomAttribute<TaskAttribute>() != null)
            .Select(t => new
            {
                Attribute = t.GetCustomAttribute<TaskAttribute>()!,
                Type = t
            });
        _taskTypes = pairs.ToDictionary(p => p.Attribute.TaskName ?? p.Type.Name, p => p.Type);
    }

    /// <summary>
    /// Gets the string name of an task type based on the [Task] attribute, or returns null if the type is not registered as an task. This is used for storing and querying tasks in the task store.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string? GetTaskName(Type type) => _taskTypes.FirstOrDefault(kv => kv.Value == type).Key;

    /// <summary>
    /// Gets the task type corresponding to a given string name based on the [Task] attribute, or returns null if the name is not registered as an task. This is used for deserializing and dispatching tasks from the task store.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Type? GetTaskType(string name) => _taskTypes.GetValueOrDefault(name);

    /// <summary>
    /// Gets all registered task names and types in the system, which can be used for diagnostics, monitoring, or dynamic task handling scenarios.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> GetTaskNames() => _taskTypes.Keys;

    /// <summary>
    /// Gets all registered task types and their corresponding string names in the system, which can be used for diagnostics, monitoring, or dynamic task handling scenarios.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetTaskTypes() => _taskTypes.Values;
}