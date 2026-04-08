namespace KolibSoftware.Api.Infra.Jobs.Attributes;

/// <summary>
/// Indicates that the decorated class or struct is a job to be registered and executed by the job service. The attribute allows specifying an optional name, execution interval, and schedule for the job.
/// </summary>
/// <param name="name"></param>
/// <param name="interval"></param>
/// <param name="schedule"></param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class JobAttribute(
    string? name = null,
    string? interval = null,
    string? schedule = null
) : Attribute
{
    /// <summary>
    /// Gets the name of the job, which can be used for configuration and logging purposes. If not specified, the job's type name will be used as the default name.
    /// </summary>
    public string? Name { get; } = name;

    /// <summary>
    /// Gets the execution interval for the job, which determines how often the job should be executed. The interval can be specified as a string that can be parsed into a TimeSpan (e.g., "00:30:00" for 30 minutes). If not specified, the interval can be configured through application settings or will default to 24 hours.
    /// </summary>
    public TimeSpan? Interval { get; } = interval is not null ? TimeSpan.Parse(interval) : null;

    /// <summary>
    /// Gets the daily schedule time for the job, which determines the specific time of day when the job should be executed. The schedule can be specified as a string that can be parsed into a TimeOnly (e.g., "14:00:00" for 2 PM). If not specified, the job will be executed based on its defined interval without a specific daily schedule.
    /// </summary>
    public TimeOnly? Schedule { get; } = schedule is not null ? TimeOnly.Parse(schedule) : null;
}