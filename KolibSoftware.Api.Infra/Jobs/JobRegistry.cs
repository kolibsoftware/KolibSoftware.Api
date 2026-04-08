using System.Reflection;
using KolibSoftware.Api.Infra.Jobs.Attributes;

namespace KolibSoftware.Api.Infra.Jobs;

/// <summary>
/// A static registry that discovers and stores information about job types in the application. It scans assemblies marked with the EnableJobsAttribute for types decorated with the JobAttribute, and provides methods to retrieve job types by name or get all registered job types.
/// </summary>
public static class JobRegistry
{
    private static readonly Dictionary<string, Type> _jobTypes;
    
    static JobRegistry()
    {
        _jobTypes = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetCustomAttribute<EnableJobsAttribute>() != null)
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetCustomAttribute<JobAttribute>() != null)
            .ToDictionary(
                t => t.GetCustomAttribute<JobAttribute>()?.Name ?? t.Name,
                t => t
            );
    }

    /// <summary>
    /// Gets the name of a job type, which can be used for configuration and logging purposes. If the job type is not registered, it returns null.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string? GetJobName(Type type) => _jobTypes.FirstOrDefault(kv => kv.Value == type).Key;

    /// <summary>
    /// Gets the job type associated with the specified name. If the name is not registered, it returns null.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Type? GetJobType(string name) => _jobTypes.GetValueOrDefault(name);

    /// <summary>
    /// Gets all registered job types in the application, allowing the job service to discover and execute them based on their defined intervals and schedules.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetJobTypes() => _jobTypes.Values;
}
