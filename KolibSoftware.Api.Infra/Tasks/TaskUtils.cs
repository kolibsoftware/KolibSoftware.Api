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
    /// Extension method to publish a task without dependencies to the task worker. This method simplifies the process of publishing a task by allowing the caller to omit the dependencies parameter when there are no dependencies for the task. It internally calls the PublishAsync method of the ITaskService with an empty list of dependencies.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="taskService"></param>
    /// <param name="task"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<TaskModel> PublishAsync<T>(this ITaskService taskService, T task, CancellationToken cancellationToken = default)
        where T : notnull
        => taskService.PublishAsync(task, [], cancellationToken);

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
            var handlers = TaskHandlerRegistry.GetTypeHandlers(type);
            foreach (var handler in handlers)
                builder.Services.AddTransient(handler, type);
        }
        return builder;
    }

}