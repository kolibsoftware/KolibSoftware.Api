using System.Text.Json;
using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Repo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Background service that periodically checks for pending tasks in the task store and dispatches them to registered handlers.
/// </summary>
/// <param name="serviceProvider"></param>
/// <param name="options"></param>
/// <param name="logger"></param>
public sealed class TaskWorker(
    IServiceProvider serviceProvider,
    IOptions<TaskWorkerSettings> options,
    ILogger<TaskWorker> logger
) : BackgroundService(), ITaskWorker
{

    /// <summary>
    /// Settings for the task worker, which is responsible for dispatching tasks to handlers. The delay is the time to wait before dispatching a task, and the threshold is the maximum time to wait before dispatching a task. If a task is not dispatched within the threshold, it will be discarded.
    /// </summary>
    private readonly TimeSpan Delay = options.Value.PollingInterval;

    /// <summary>
    /// Executes the background service, which periodically retrieves pending tasks from the task store and dispatches them to the appropriate handlers. The service uses a periodic timer to control the frequency of task retrieval and dispatching, and it handles any exceptions that occur during the process by logging them. The method continues to run until the service is stopped, at which point it will gracefully exit.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(Delay);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IQueryableRepository<TaskModel>>();
            var query = new TaskQuery();
            var tasks = await repository.GetAllAsync(query, stoppingToken);
            if (tasks.Any())
                foreach (var task in tasks)
                {
                    try
                    {
                        var result = await DispatchTask(task, scope.ServiceProvider, stoppingToken);
                        result.Apply(task);
                        await repository.UpdateAsync(task, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogErrorDispatchingTasks(ex);
                    }
                }
        }
    }

    /// <summary>
    /// Dispatches a single task to the appropriate handlers based on the task type. The method retrieves the task type from the task registry, finds all registered handlers for that task type, and invokes each handler's HandleTaskAsync method with the task data. The method returns a TaskStatus indicating whether the dispatch was successful, partially successful, or a failure, based on the number of handlers that successfully processed the task. If the task type is not registered or if no handlers are found, it logs the issue and returns a failure status.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<ITaskResult> DispatchTask(TaskModel task, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var taskType = TaskRegistry.GetTaskType(task.Name);
        if (taskType == null)
        {
            logger.LogUnregisteredTaskType(task.Name);
            return TaskResult.Failed();
        }

        var data = task.Data.Deserialize(taskType);
        if (data == null)
        {
            logger.LogFailedToDeserializeTaskData(task.Name, taskType.FullName!);
            return TaskResult.Failed();
        }

        var dependencies = task.Dependencies.Select(d => d.Dependency.Data.Deserialize(TaskRegistry.GetTaskType(d.Dependency.Name)!)).ToList();
        if(dependencies.Any(d => d == null))
        {
            logger.LogFailedToDeserializeTaskDependencies(task.Name);
            return TaskResult.Failed();
        }

        var handlerType = typeof(ITaskHandler<>).MakeGenericType(taskType);
        var handlers = serviceProvider.GetServices(handlerType).Cast<ITaskHandler>().ToList();
        if (handlers.Count != 1)
        {
            logger.LogInvalidNumberOfTaskHandlers(task.Name, handlers.Count);
            return TaskResult.Failed();
        }

        var handler = handlers.First();
        try
        {
            var result = await handler.HandleTaskAsync(data, dependencies!, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            var handlerTypeName = handler.GetType().FullName ?? "UnknownHandler";
            logger.LogErrorHandlingTask(task.Name, handlerTypeName, ex);
            return TaskResult.Failed();
        }
    }

    /// <summary>
    /// Query that selects tasks that are waiting for their dependencies to be completed, to be dispatched by the task worker service.
    /// </summary>
    public sealed class TaskQuery() : IQuery<TaskModel>
    {
        public IQueryable<TaskModel> Apply(IQueryable<TaskModel> query)
        {
            return query.Include(x => x.Dependencies).ThenInclude(d => d.Dependency)
                .Where(t => t.Status == Models.TaskStatus.Pending && t.Dependencies.All(d => d.Dependency.Status == Models.TaskStatus.Completed))
                .Take(10);
        }
    }
}
