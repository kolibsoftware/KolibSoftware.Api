using KolibSoftware.Api.Infra.Models;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Represents the result of handling a task, including any status updates or data changes that should be applied to the task model.
/// </summary>
public interface ITaskResult
{
    /// <summary>
    /// Applies the result to the given task model, updating its status, data, dependencies, or other relevant properties as needed.
    /// </summary>
    /// <param name="model"></param>
    void Apply(TaskModel model);
}
