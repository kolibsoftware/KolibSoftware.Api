using System.Reflection;
using KolibSoftware.Api.Infra.Tasks.Attributes;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Static registry that discovers and caches all task handler types in assemblies marked with the EnableTasksAttribute, allowing efficient lookup of handlers for a given task type or vice versa.
/// </summary>
public static class TaskHandlerRegistry
{
    private static readonly Dictionary<Type, IEnumerable<Type>> _handlerTypes;
    private static readonly Dictionary<Type, IEnumerable<Type>> _typeHandlers;

    static TaskHandlerRegistry()
    {
        var pairs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetCustomAttribute<EnableTasksAttribute>() != null)
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface && t.GetCustomAttribute<TaskHandlerAttribute>() != null)
            .SelectMany(t => t.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITaskHandler<>))
                .Select(i => new
                {
                    Handler = i,
                    Type = t
                })
            );

        _handlerTypes = pairs
            .GroupBy(p => p.Handler)
            .ToDictionary(g => g.Key, g => g.Select(p => p.Type));

        _typeHandlers = pairs
            .GroupBy(p => p.Type)
            .ToDictionary(g => g.Key, g => g.Select(p => p.Handler));
    }

    /// <summary>
    /// Gets all registered task handler types in the application, which can be used for diagnostics or dynamic registration in the dependency injection container.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetHandlerTypes() => _typeHandlers.Keys;

    /// <summary>
    /// Gets the task types that a given handler type is registered to handle, which can be used for diagnostics or dynamic registration in the dependency injection container.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetTypeHandlers() => _handlerTypes.Keys;

    /// <summary>
    /// Gets the handler types that are registered to handle a specific task type, which can be used by the task broker service to dispatch tasks to the correct handlers.
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetHandlerTypes(Type handler) => _handlerTypes.GetValueOrDefault(handler) ?? [];

    /// <summary>
    /// Gets the task types that a given handler type is registered to handle, which can be used by the task broker service to dispatch tasks to the correct handlers.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetTypeHandlers(Type type) => _typeHandlers.GetValueOrDefault(type) ?? [];

}
