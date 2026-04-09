using Microsoft.Extensions.Logging;

namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Static class containing strongly-typed logging methods for the event broker service, using source generators for performance and maintainability.
/// </summary>
public static partial class EventLogs
{

    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Error,
        Message = "Error dispatching events"
    )]
    public static partial void LogErrorDispatchingEvents(this ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Warning,
        Message = "Unregistered event type: {eventType}"
    )]
    public static partial void LogUnregisteredEventType(this ILogger logger, string eventType);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Warning,
        Message = "No handlers found for event type: {eventType}"
    )]
    public static partial void LogNoHandlersFound(this ILogger logger, string eventType);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Error,
        Message = "Error handling event {eventType} with handler {handlerType}"
    )]
    public static partial void LogErrorHandlingEvent(this ILogger logger, string eventType, string handlerType, Exception exception);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Error,
        Message = "Data type mismatch for event {eventType}: expected {expectedType}, but got {actualType}"
    )]
    public static partial void LogDataTypeMismatch(this ILogger logger, string eventType, string expectedType, string actualType);

}