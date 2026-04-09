namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Defines the contract for handling events in the application. Implementations of this interface should provide logic to process event data when an event is raised. The HandleEventAsync method is responsible for executing the necessary actions based on the event data, and it can be asynchronous to allow for non-blocking operations. This interface allows for a decoupled architecture where event producers and consumers can operate independently, with the event handler serving as the bridge between them.
/// </summary>
public interface IEventHandler
{
    /// <summary>
    /// Handles the event. This method is called when an event is published. The implementation should contain the logic to process the event data, which may include updating state, triggering other actions, or communicating with external systems. The method is asynchronous to support operations that may involve I/O or other long-running tasks without blocking the calling thread. The cancellation token can be used to gracefully handle cancellation requests, allowing for proper cleanup and resource management if the operation needs to be aborted.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleEventAsync(object data, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines the contract for handling events of a specific type. Implementations of this interface should provide logic to process the event data when an event of type T is raised. The HandleEventAsync method is responsible for executing the necessary actions based on the event data, and it can be asynchronous to allow for non-blocking operations. This interface allows for a decoupled architecture where event producers and consumers can operate independently, with the event handler serving as the bridge between them.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IEventHandler<T> : IEventHandler
{
    /// <summary>
    /// Handles the event of type T. This method is called when an event of the specified type is published. The implementation should contain the logic to process the event data, which may include updating state, triggering other actions, or communicating with external systems. The method is asynchronous to support operations that may involve I/O or other long-running tasks without blocking the calling thread. The cancellation token can be used to gracefully handle cancellation requests, allowing for proper cleanup and resource management if the operation needs to be aborted.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleEventAsync(T data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles the event of type T by invoking the HandleEventAsync method with the appropriate type casting. This method is part of the IEventHandler interface and serves as a bridge to allow handling events in a type-safe manner while still conforming to the non-generic interface contract. The implementation simply casts the incoming data to the expected type T and calls the type-specific HandleEventAsync method, allowing for seamless integration with the event dispatching mechanism that operates on the non-generic IEventHandler interface.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task IEventHandler.HandleEventAsync(object data, CancellationToken cancellationToken) => HandleEventAsync((T)data, cancellationToken);
}