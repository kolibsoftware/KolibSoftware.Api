using Microsoft.Extensions.Logging;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Static class containing strongly-typed logging methods for the task system, using source generators for performance and maintainability.
/// </summary>
public static partial class TaskLogs
{

    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Error,
        Message = "Error dispatching tasks"
    )]
    public static partial void LogErrorDispatchingTasks(this ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Warning,
        Message = "Unregistered task type: {taskType}"
    )]
    public static partial void LogUnregisteredTaskType(this ILogger logger, string taskType);

    [LoggerMessage(
        EventId = 5,
        Level = LogLevel.Error,
        Message = "Failed to deserialize task data for task type: {taskType}, expected type: {expectedType}"
    )]
    public static partial void LogFailedToDeserializeTaskData(this ILogger logger, string taskType, string expectedType);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Warning,
        Message = "Invalid number of task handlers for task type: {taskType}, must be 1 found: {handlerCount}"
    )]
    public static partial void LogInvalidNumberOfTaskHandlers(this ILogger logger, string taskType, int handlerCount);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Error,
        Message = "Error handling task {taskType} with handler {handlerType}"
    )]
    public static partial void LogErrorHandlingTask(this ILogger logger, string taskType, string handlerType, Exception exception);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Error,
        Message = "Data type mismatch for task {taskType}: expected {expectedType}, but got {actualType}"
    )]
    public static partial void LogDataTypeMismatch(this ILogger logger, string taskType, string expectedType, string actualType);

}