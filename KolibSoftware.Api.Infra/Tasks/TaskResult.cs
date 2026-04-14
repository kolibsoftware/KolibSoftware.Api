using System.Text.Json;
using System.Text.Json.Nodes;
using KolibSoftware.Api.Infra.Models;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Represents the result of handling a task, which can include a new status, updated data, or dependencies on other tasks. The TaskResult class provides static factory methods for creating different types of results, such as status updates, state changes with associated data, and dependency tracking. When applied to a TaskModel, the TaskResult will update the model's properties accordingly, allowing the task worker to manage the execution flow based on the results returned by task handlers.
/// </summary>
public abstract class TaskResult : ITaskResult
{

    /// <summary>
    /// Applies the task result to the given TaskModel, updating its properties based on the type of result. This method is called by the task worker after a task handler returns a TaskResult, allowing it to update the task's status, data, and dependencies as needed. The specific implementation of this method will depend on the type of TaskResult being applied, such as a status update, state change, or dependency tracking.
    /// </summary>
    /// <param name="model"></param>
    public abstract void Apply(TaskModel model);

    /// <summary>
    /// Creates a task result that indicates a change in the task's status. The method accepts a TaskStatus enum value representing the new status of the task, and returns an ITaskResult that, when applied to a TaskModel, will update its Status property to the specified value and set the HandledAt timestamp to the current UTC time. This allows task handlers to easily indicate status changes for tasks they process, enabling the task worker to manage the execution flow based on these status updates.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public static ITaskResult Status(Models.TaskStatus status) => new StatusResult(status);

    /// <summary>
    /// Creates a task result that indicates a change in the task's status along with associated data. The method accepts a TaskStatus enum value representing the new status of the task and a data object of type T, which will be serialized to JSON and stored in the TaskModel's Data property. The returned ITaskResult, when applied to a TaskModel, will update its Status property to the specified value, set its Data property to the serialized data, and set the HandledAt timestamp to the current UTC time. This allows task handlers to provide additional context or information along with status updates for tasks they process, enabling more complex workflows and state management within the task worker.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="status"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static ITaskResult State<T>(Models.TaskStatus status, T data)
    {
        if (TaskRegistry.GetTaskName(typeof(T)) is null) throw new Exception($"Type {typeof(T).FullName} is not registered as a task");
        var jsonData = JsonSerializer.SerializeToNode(data) ?? throw new Exception("Failed to serialize task data");
        return new StateResult(status, jsonData);
    }

    /// <summary>
    /// Creates a task result that indicates the current task is waiting for the completion of one or more dependent tasks. The method accepts a collection of TaskModel instances representing the dependencies, and returns an ITaskResult that, when applied to a TaskModel, will set its status to Waiting and associate the specified dependencies with it. This allows the task worker to track the dependencies and only dispatch the task for execution once all its dependencies have been completed successfully.
    /// </summary>
    /// <param name="dependencies"></param>
    /// <returns></returns>
    public static ITaskResult Dependency(IEnumerable<TaskModel> dependencies) => new DependencyResult(null, dependencies);

    /// <summary>
    /// Creates a task result that indicates the current task is waiting for the completion of one or more dependent tasks, along with associated data. The method accepts a data object of type T, which will be serialized to JSON and stored in the TaskModel's Data property, and a collection of TaskModel instances representing the dependencies. The returned ITaskResult, when applied to a TaskModel, will set its status to Waiting, associate the specified dependencies with it, and store the provided data in its Data property. This allows task handlers to provide additional context or information along with dependency tracking for tasks they process, enabling more complex workflows and state management within the task worker.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="dependencies"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static ITaskResult Dependency<T>(T data, IEnumerable<TaskModel> dependencies)
    {
        if (TaskRegistry.GetTaskName(typeof(T)) is null) throw new Exception($"Type {typeof(T).FullName} is not registered as a task");
        var jsonData = JsonSerializer.SerializeToNode(data) ?? throw new Exception("Failed to serialize task data");
        return new DependencyResult(jsonData, dependencies);
    }

    /// <summary>
    /// Creates a task result that indicates the current task is suspended and should be retried later with the provided data. The method accepts a data object of type T, which will be serialized to JSON and stored in the TaskModel's Data property. The returned ITaskResult, when applied to a TaskModel, will update its Status property to Waiting, set its Data property to the serialized data, and set the HandledAt timestamp to the current UTC time. This allows task handlers to indicate that a task cannot be completed at the moment but should be retried later with the provided data, enabling more flexible handling of tasks that may require multiple attempts or additional information before they can be successfully processed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ITaskResult Suspended<T>(T data) => State(Models.TaskStatus.Pending, data);

    /// <summary>
    /// Creates a task result that indicates the current task has been completed successfully. The returned ITaskResult, when applied to a TaskModel, will update its Status property to Completed and set the HandledAt timestamp to the current UTC time. This allows task handlers to easily indicate that a task has been processed successfully, enabling the task worker to manage the execution flow based on these status updates.
    /// </summary>
    /// <returns></returns>
    public static ITaskResult Completed() => Status(Models.TaskStatus.Completed);

    /// <summary>
    /// Creates a task result that indicates the current task has been completed successfully with associated data. The method accepts a data object of type T, which will be serialized to JSON and stored in the TaskModel's Data property. The returned ITaskResult, when applied to a TaskModel, will update its Status property to Completed, set its Data property to the serialized data, and set the HandledAt timestamp to the current UTC time. This allows task handlers to provide additional context or information along with the completion status for tasks they process, enabling more complex workflows and state management within the task worker.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ITaskResult Completed<T>(T data) => State(Models.TaskStatus.Completed, data);

    /// <summary>
    /// Creates a task result that indicates the current task has been cancelled and should not be processed by any handlers. The returned ITaskResult, when applied to a TaskModel, will update its Status property to Cancelled and set the HandledAt timestamp to the current UTC time. This allows task handlers to indicate that a task should be cancelled, enabling the task worker to remove it from the task store and prevent any further processing.
    /// </summary>
    /// <returns></returns>
    public static ITaskResult Cancelled() => Status(Models.TaskStatus.Cancelled);

    /// <summary>
    /// Creates a task result that indicates the current task has been cancelled with associated data. The method accepts a data object of type T, which will be serialized to JSON and stored in the TaskModel's Data property. The returned ITaskResult, when applied to a TaskModel, will update its Status property to Cancelled, set its Data property to the serialized data, and set the HandledAt timestamp to the current UTC time. This allows task handlers to provide additional context or information along with the cancellation status for tasks they process, enabling more detailed logging and management of cancelled tasks within the task worker.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ITaskResult Cancelled<T>(T data) => State(Models.TaskStatus.Cancelled, data);

    /// <summary>
    /// Creates a task result that indicates the current task has failed during processing. The returned ITaskResult, when applied to a TaskModel, will update its Status property to Failed and set the HandledAt timestamp to the current UTC time. This allows task handlers to indicate that an error occurred while processing the task, enabling the task worker to manage retries, logging, or manual intervention as needed based on these failure status updates.
    /// </summary>
    /// <returns></returns>
    public static ITaskResult Failed() => Status(Models.TaskStatus.Failed);

    /// <summary>
    /// Creates a task result that indicates the current task has failed during processing with associated data. The method accepts a data object of type T, which will be serialized to JSON and stored in the TaskModel's Data property. The returned ITaskResult, when applied to a TaskModel, will update its Status property to Failed, set its Data property to the serialized data, and set the HandledAt timestamp to the current UTC time. This allows task handlers to provide additional context or information along with the failure status for tasks they process, enabling more detailed logging, error handling, and potential retries within the task worker.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ITaskResult Failed<T>(T data) => State(Models.TaskStatus.Failed, data);

    /// <summary>
    /// Creates a task result that indicates the current task is waiting for the completion of one or more dependent tasks, with the dependencies specified as a collection of objects. The method accepts a data object of type T, which will be serialized to JSON and stored in the TaskModel's Data property, and a collection of dependency objects. Each dependency object is expected to be of a type that is registered as a task in the TaskRegistry. The method will serialize each dependency object into a TaskModel and associate it with the current task as a dependency. The returned ITaskResult, when applied to a TaskModel, will set its status to Waiting, associate the specified dependencies with it, and store the provided data in its Data property. This allows task handlers to provide additional context or information along with dependency tracking for tasks they process, enabling more complex workflows and state management within the task worker.
    /// </summary>
    /// <param name="dependencies"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static ITaskResult Waiting(IEnumerable<object> dependencies)
    {
        var taskDependencies = dependencies.Select(d =>
        {
            var taskType = d.GetType();
            if (TaskRegistry.GetTaskName(taskType) is null) throw new Exception($"Type {taskType.FullName} is not registered as a task");
            var jsonData = JsonSerializer.SerializeToNode(d) ?? throw new Exception("Failed to serialize task dependency data");
            return new TaskModel
            {
                Rid = Guid.NewGuid(),
                Name = TaskRegistry.GetTaskName(taskType)!,
                Data = jsonData,
                Status = Models.TaskStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            };
        });
        return Dependency(taskDependencies);
    }

    /// <summary>
    /// Creates a task result that indicates the current task is waiting for the completion of one or more dependent tasks, with the dependencies specified as a collection of objects, along with associated data. The method accepts a data object of type T, which will be serialized to JSON and stored in the TaskModel's Data property, and a collection of dependency objects. Each dependency object is expected to be of a type that is registered as a task in the TaskRegistry. The method will serialize each dependency object into a TaskModel and associate it with the current task as a dependency. The returned ITaskResult, when applied to a TaskModel, will set its status to Waiting, associate the specified dependencies with it, and store the provided data in its Data property. This allows task handlers to provide additional context or information along with dependency tracking for tasks they process, enabling more complex workflows and state management within the task worker.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="dependencies"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static ITaskResult Waiting<T>(T data, IEnumerable<object> dependencies)
    {
        var taskDependencies = dependencies.Select(d =>
        {
            var taskType = d.GetType();
            if (TaskRegistry.GetTaskName(taskType) is null) throw new Exception($"Type {taskType.FullName} is not registered as a task");
            var jsonData = JsonSerializer.SerializeToNode(d) ?? throw new Exception("Failed to serialize task dependency data");
            return new TaskModel
            {
                Rid = Guid.NewGuid(),
                Name = TaskRegistry.GetTaskName(taskType)!,
                Data = jsonData,
                Status = Models.TaskStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            };
        });
        return Dependency(data, taskDependencies);
    }

    /// <summary>
    /// Private implementation of a TaskResult that represents a simple status update. When applied to a TaskModel, it updates the model's Status property to the specified value and sets the HandledAt timestamp to the current UTC time. This is used internally by the static factory methods to create status update results for tasks.
    /// </summary>
    /// <param name="status"></param>
    private sealed class StatusResult(Models.TaskStatus status) : TaskResult
    {
        public override void Apply(TaskModel model)
        {
            model.Status = status;
            model.HandledAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Private implementation of a TaskResult that represents a state change with associated data. When applied to a TaskModel, it updates the model's Status property to the specified value, sets its Data property to the provided JSON data, and sets the HandledAt timestamp to the current UTC time. This is used internally by the static factory methods to create state change results for tasks that include additional context or information in their Data property.
    /// </summary>
    /// <param name="status"></param>
    /// <param name="data"></param>
    private sealed class StateResult(Models.TaskStatus status, JsonNode data) : TaskResult
    {
        public override void Apply(TaskModel model)
        {
            model.Data = data;
            model.Status = status;
            model.HandledAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Private implementation of a TaskResult that represents a task waiting for dependencies. When applied to a TaskModel, it adds the specified dependencies to the model's Dependencies collection, updates its status to Waiting, and sets the HandledAt timestamp to the current UTC time. This is used internally by the static factory methods to create dependency tracking results for tasks that need to wait for other tasks to be completed before they can be processed.
    /// </summary>
    /// <param name="dependencies"></param>
    private sealed class DependencyResult(JsonNode? data, IEnumerable<TaskModel> dependencies) : TaskResult
    {
        public override void Apply(TaskModel model)
        {
            if (data != null) model.Data = data;
            model.Status = Models.TaskStatus.Pending;
            model.HandledAt = DateTime.UtcNow;
            foreach (var dep in dependencies)
                model.Dependencies.Add(new TaskDependency { Dependency = dep });
        }
    }
}