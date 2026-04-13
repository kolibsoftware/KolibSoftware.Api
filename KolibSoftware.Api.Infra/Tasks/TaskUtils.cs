using System.Text.Json;
using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Repo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Utility class that provides extension methods for configuring and using the task worker services and handlers in the application host builder. This class includes methods to add task worker services, discover and register task handler types, and define queries for selecting pending tasks to be dispatched by the task worker service.
/// </summary>
public static class TaskUtils
{

    /// <summary>
    /// Extension method to add task worker services and handlers to the application host builder. This method configures the task worker settings, registers the task service, and discovers and registers all task handler types in the application assembly.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddTasks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IRepository<TaskModel>, Repository<TaskModel>>();
        builder.Services.AddScoped<IQueryableRepository<TaskModel>, QueryableRepository<TaskModel>>();
        builder.Services.AddScoped<ITaskService, TaskService>();
        builder.Services.Configure<TaskWorkerSettings>(builder.Configuration.GetSection("TaskWorker"));
        builder.Services.AddHostedService<TaskWorker>();
        var types = TaskHandlerRegistry.GetHandlerTypes();
        foreach (var type in types)
        {
            var taskName = TaskHandlerRegistry.GetTaskName(type) ?? throw new InvalidOperationException($"Task handler type {type.FullName} does not have a registered task name");
            var handler = TaskHandlerRegistry.GetHandlerType(taskName) ?? throw new InvalidOperationException($"Task name {taskName} does not have a registered handler type");
            builder.Services.AddKeyedScoped(typeof(ITaskHandler), taskName, handler);
        }
        return builder;
    }

    /// <summary>
    /// Publishes a task of a specific type to the task worker. This method creates a TaskItem instance from the provided task data, converts it into a TaskModel, and then calls the PublishAsync method of the ITaskService to store it in the task store. The task type must be registered in the TaskRegistry for it to be published successfully. This method provides a convenient way to publish tasks directly from their data representation without needing to manually create TaskItem instances, allowing for streamlined task publishing in the application.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="taskService"></param>
    /// <param name="task"></param>
    /// <param name="dependencies"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<TaskModel> PublishAsync<T>(this ITaskService taskService, T task, IEnumerable<TaskModel> dependencies, CancellationToken cancellationToken = default)
        where T : notnull
    {
        var model = new TaskModel
        {
            Rid = Guid.CreateVersion7(),
            Name = TaskRegistry.GetTaskName(typeof(T)) ?? throw new InvalidOperationException($"Task type {typeof(T).FullName} is not registered in TaskRegistry."),
            Data = JsonSerializer.SerializeToNode(task)!,
            CreatedAt = DateTime.UtcNow,
            Status = Models.TaskStatus.Pending,
            HandledAt = null,
            Dependencies = [.. dependencies.Select(d => new TaskDependency { Dependency = d })]
        };
        await taskService.PublishAsync(model, cancellationToken);
        return model;
    }

    /// <summary>
    /// Publishes a TaskItem to the task worker. This method converts the TaskItem into a TaskModel using the ToModel method and then calls the PublishAsync method of the ITaskService to store the task in the task store. The TaskItem encapsulates the task data and its dependencies, allowing for structured representation of tasks and their relationships. This method provides a convenient way to publish tasks that are defined as TaskItem instances, which can include complex workflows with multiple dependencies. The task type of the TaskItem must be registered in the TaskRegistry for it to be published successfully.
    /// </summary>
    /// <param name="taskService"></param>
    /// <param name="taskItem"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<TaskModel> PublishAsync(this ITaskService taskService, TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        var model = taskItem.ToModel();
        await taskService.PublishAsync(model, cancellationToken);
        return model;
    }

    /// <summary>
    /// Publishes a task of a specific type to the task worker. This method creates a TaskItem instance from the provided task data, converts it into a TaskModel, and then calls the PublishAsync method of the ITaskService to store it in the task store. The task type must be registered in the TaskRegistry for it to be published successfully. This method provides a convenient way to publish tasks directly from their data representation without needing to manually create TaskItem instances, allowing for streamlined task publishing in the application.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="taskService"></param>
    /// <param name="task"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<TaskModel> PublishAsync<T>(this ITaskService taskService, T task, CancellationToken cancellationToken = default)
        where T : notnull
        => PublishAsync(taskService, new TaskItem { Task = task }, cancellationToken);

}