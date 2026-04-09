using Microsoft.Extensions.Hosting;

namespace KolibSoftware.Api.Infra.Tasks;

/// <summary>
/// Defines the interface for the task worker service, which is responsible for managing and processing tasks in the application. This service runs in the background and handles the execution of task handlers based on the configured settings and registered handlers.
/// </summary>
public interface ITaskWorker : IHostedService { }