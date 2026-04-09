namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Defines the contract for handling tasks in the application. Implementations of this interface should provide logic to process task data when a task is raised. The HandleTaskAsync method is responsible for executing the necessary actions based on the task data, and it can be asynchronous to allow for non-blocking operations. This interface allows for a decoupled architecture where task producers and consumers can operate independently, with the task handler serving as the bridge between them.
/// </summary>
public interface ITaskHandler
{
    /// <summary>
    /// Handles the task. This method is called when a task is published. The implementation should contain the logic to process the task data, which may include updating state, triggering other actions, or communicating with external systems. The method is asynchronous to support operations that may involve I/O or other long-running tasks without blocking the calling thread. The cancellation token can be used to gracefully handle cancellation requests, allowing for proper cleanup and resource management if the operation needs to be aborted.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleTaskAsync(object data, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines the contract for handling tasks of a specific type. Implementations of this interface should provide logic to process the task data when a task of type T is raised. The HandleTaskAsync method is responsible for executing the necessary actions based on the task data, and it can be asynchronous to allow for non-blocking operations. This interface allows for a decoupled architecture where task producers and consumers can operate independently, with the task handler serving as the bridge between them.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITaskHandler<T> : ITaskHandler
{
    /// <summary>
    /// Handles the task of type T. This method is called when a task of the specified type is published. The implementation should contain the logic to process the task data, which may include updating state, triggering other actions, or communicating with external systems. The method is asynchronous to support operations that may involve I/O or other long-running tasks without blocking the calling thread. The cancellation token can be used to gracefully handle cancellation requests, allowing for proper cleanup and resource management if the operation needs to be aborted.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleTaskAsync(T data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles the task of type T by invoking the HandleTaskAsync method with the appropriate type casting. This method is part of the ITaskHandler interface and serves as a bridge to allow handling tasks in a type-safe manner while still conforming to the non-generic interface contract. The implementation simply casts the incoming data to the expected type T and calls the type-specific HandleTaskAsync method, allowing for seamless integration with the task dispatching mechanism that operates on the non-generic ITaskHandler interface.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ITaskHandler.HandleTaskAsync(object data, CancellationToken cancellationToken) => HandleTaskAsync((T)data, cancellationToken);
}