using Microsoft.Extensions.Hosting;

namespace KolibSoftware.Api.Infra.Events;

/// <summary>
/// Defines the interface for the event worker service, which is responsible for managing and processing events in the application. This service runs in the background and handles the execution of event handlers based on the configured settings and registered handlers.
/// </summary>
public interface IEventWorker : IHostedService { }