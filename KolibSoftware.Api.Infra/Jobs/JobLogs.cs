using Microsoft.Extensions.Logging;

namespace KolibSoftware.Api.Infra.Jobs;

/// <summary>
/// Defines strongly-typed logging methods for the Job Service, providing structured log messages for various events such as scheduling, executing, completing, and error handling of jobs.
/// </summary>
public static partial class JobLogs
{

    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Error,
        Message = "Error in Job Service"
    )]
    public static partial void LogJobServiceError(this ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Scheduling job {jobType} to run first in {delay} and then every {interval}"
    )]
    public static partial void LogSchedulingJob(this ILogger logger, Type jobType, TimeSpan delay, TimeSpan interval);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "Executing job {jobType}"
    )]
    public static partial void LogExecutingJob(this ILogger logger, Type jobType);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Information,
        Message = "Completed job {jobType}"
    )]
    public static partial void LogCompletedJob(this ILogger logger, Type jobType);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Error,
        Message = "Error executing job {jobType}"
    )]
    public static partial void LogErrorExecutingJob(this ILogger logger, Type jobType, Exception exception);
}