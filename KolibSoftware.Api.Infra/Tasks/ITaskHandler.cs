using KolibSoftware.Api.Infra.Models;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Defines the contract for handling tasks in the task broker system. Implementations of this interface will be responsible for processing tasks of specific types, as determined by the [TaskHandler] attribute and the TaskHandlerRegistry. The HandleTaskAsync method should return true if the task was handled successfully, or false if it failed and should be retried according to the task broker's retry policy.
/// </summary>
public interface ITaskHandler
{
    /// <summary>
    /// Handles a task represented by the TaskModel. The implementation should process the task according to its type and payload, and return true if the task was completed successfully, or false if it failed and should be retried according to the task broker's retry policy.
    /// The cancellation token can be used to gracefully handle shutdown scenarios or timeouts, allowing the handler to stop processing and clean up resources if needed.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> HandleTaskAsync(TaskModel model, CancellationToken cancellationToken = default);
}
