using System.Text.Json;
using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Repo;
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
                        task.Status = await DispatchTask(task, scope.ServiceProvider, stoppingToken);
                        task.HandledAt = DateTime.UtcNow;
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
    private async Task<Models.TaskStatus> DispatchTask(TaskModel task, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var taskType = TaskRegistry.GetTaskType(task.Name);
        if (taskType == null)
        {
            logger.LogUnregisteredTaskType(task.Name);
            return Models.TaskStatus.Failure;
        }

        var data = task.Data.Deserialize(taskType);
        if (data == null)
        {
            logger.LogFailedToDeserializeTaskData(task.Name, taskType.FullName!);
            return Models.TaskStatus.Failure;
        }

        var handlerType = typeof(ITaskHandler<>).MakeGenericType(taskType);
        var handlers = serviceProvider.GetServices(handlerType).Cast<ITaskHandler>().ToList();
        if (handlers.Count != 1)
        {
            logger.LogInvalidNumberOfTaskHandlers(task.Name, handlers.Count);
            return Models.TaskStatus.Failure;
        }

        var handler = handlers.First();
        try
        {
            await handler.HandleTaskAsync(data, cancellationToken);
            return Models.TaskStatus.Success;
        }
        catch (Exception ex)
        {
            var handlerTypeName = handler.GetType().FullName ?? "UnknownHandler";
            logger.LogErrorHandlingTask(task.Name, handlerTypeName, ex);
            return Models.TaskStatus.Failure;
        }
    }

    /// <summary>
    /// Query that selects pending tasks that are older than a specified age threshold, to be dispatched by the task worker service.
    /// </summary>
    public sealed class TaskQuery() : IQuery<TaskModel>
    {
        public IQueryable<TaskModel> Apply(IQueryable<TaskModel> query)
        {
            return query.Where(t => t.Status == Models.TaskStatus.Pending);
        }
    }
}
