namespace KolibSoftware.Api.Infra.Models;

/// <summary>
/// Enumeration representing the status of an event in the event broker system, indicating whether it is pending, successfully handled, partially handled, or failed.
/// </summary>
public enum EventStatus
{
    /// <summary>
    /// The event is pending and has not yet been dispatched to handlers. It is waiting to be processed by the event broker service.
    /// </summary>
    Pending,

    /// <summary>
    /// The event was successfully handled by all registered handlers without any errors. It has been processed and can be considered complete.
    /// </summary>
    Success,

    /// <summary>
    /// The event was partially handled, meaning that some handlers processed it successfully while others encountered errors. It may require further investigation or retries to ensure all handlers can process it successfully.
    /// </summary>
    Partial,

    /// <summary>
    /// The event handling failed, indicating that all handlers encountered errors while processing the event. It may require manual intervention or retries to resolve the issues and successfully handle the event.
    /// </summary>
    Failure
}