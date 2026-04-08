namespace KolibSoftware.Api.Infra.Jobs.Attributes;

/// <summary>
/// Indicates that the assembly contains jobs to be registered and executed by the job service.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class EnableJobsAttribute : Attribute;