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

// /// <summary>
// /// Defines the contract for handling events of a specific type. Implementations of this interface should provide logic to process the event data when an event of type T is raised. The HandleEventAsync method is responsible for executing the necessary actions based on the event data, and it can be asynchronous to allow for non-blocking operations. This interface allows for a decoupled architecture where event producers and consumers can operate independently, with the event handler serving as the bridge between them.
// /// </summary>
// /// <typeparam name="T"></typeparam>
// public interface IEventHandler<T> : IEventHandler
// {
//     /// <summary>
//     /// Handles the event of type T. This method is called when an event of the specified type is published. The implementation should contain the logic to process the event data, which may include updating state, triggering other actions, or communicating with external systems. The method is asynchronous to support operations that may involve I/O or other long-running tasks without blocking the calling thread. The cancellation token can be used to gracefully handle cancellation requests, allowing for proper cleanup and resource management if the operation needs to be aborted.
//     /// </summary>
//     /// <param name="data"></param>
//     /// <param name="cancellationToken"></param>
//     /// <returns></returns>
//     Task HandleEventAsync(T data, CancellationToken cancellationToken = default);

//     /// <summary>
//     /// Handles the event represented by the EventModel. This method is called when an event is published, and it should contain the logic to process the event data. The implementation can perform various actions based on the event, such as updating state, triggering other actions, or communicating with external systems. The method is asynchronous to support operations that may involve I/O or other long-running tasks without blocking the calling thread. The cancellation token can be used to gracefully handle cancellation requests, allowing for proper cleanup and resource management if the operation needs to be aborted.
//     /// </summary>
//     /// <param name="model"></param>
//     /// <param name="cancellationToken"></param>
//     /// <returns></returns>
//     Task IEventHandler.HandleEventAsync(EventModel model, CancellationToken cancellationToken)
//     {
//         var data = model.Data.Deserialize<T>() ?? throw new JsonException($"Failed to deserialize event data to type {typeof(T).FullName}");
//         return HandleEventAsync(data, cancellationToken);
//     }
// }