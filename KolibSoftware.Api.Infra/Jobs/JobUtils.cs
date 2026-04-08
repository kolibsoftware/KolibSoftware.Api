using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KolibSoftware.Api.Infra.Jobs;

/// <summary>
/// Provides extension methods for configuring and adding the job service to the application's dependency injection container. This includes registering the job service as a hosted service and discovering job types to be executed by the service based on their defined attributes and configurations.
/// </summary>
public static class JobUtils
{

    /// <summary>
    /// Adds the job service to the application's dependency injection container, allowing it to discover and execute registered jobs based on their defined intervals and schedules. This method also registers all job types found in the application for dependency injection, enabling them to be resolved and executed by the job service.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddJobs(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHostedService<JobService>();
        var jobTypes = JobRegistry.GetJobTypes();
        foreach (var jobType in jobTypes)
            builder.Services.AddKeyedTransient(typeof(IJob), jobType, jobType);
        return builder;
    }

}