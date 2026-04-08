using Microsoft.Extensions.Hosting;

namespace KolibSoftware.Api.Infra.Jobs;

/// <summary>
/// Defines a contract for a job service that can be hosted by the application to manage and execute registered jobs based on their defined intervals and schedules. The job service is responsible for discovering job types, scheduling their execution, and handling their lifecycle within the application. It implements the IHostedService interface to integrate with the application's hosting environment and ensure proper startup and shutdown behavior.
/// </summary>
public interface IJobService : IHostedService { }