using System.Text.Json;
using KolibSoftware.Api.Infra.Models;

namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Defines the contract for handling events in the application. Implementations of this interface should provide logic to process event data when an event is raised. The HandleEventAsync method is responsible for executing the necessary actions based on the event data, and it can be asynchronous to allow for non-blocking operations. This interface allows for a decoupled architecture where event producers and consumers can operate independently, with the event handler serving as the bridge between them.
/// </summary>
public interface IEventHandler
{
    /// <summary>
    /// Handles the event represented by the EventModel. This method is called when an event is published, and it should contain the logic to process the event data. The implementation can perform various actions based on the event, such as updating state, triggering other actions, or communicating with external systems. The method is asynchronous to support operations that may involve I/O or other long-running tasks without blocking the calling thread. The cancellation token can be used to gracefully handle cancellation requests, allowing for proper cleanup and resource management if the operation needs to be aborted.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleEventAsync(EventModel model, CancellationToken cancellationToken = default);
}
