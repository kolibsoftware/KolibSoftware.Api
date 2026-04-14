using System.Text.Json;
using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Tasks;
using KolibSoftware.Api.Infra.Tasks.Attributes;

namespace KolibSoftware.Api.Example.Documents;

[Task]
public class DocumentTask
{
    public string Stage { get; set; } = "Extract";
}

[TaskHandler<DocumentTask>]
public class DocumentTaskHandler() : ITaskHandler
{
    public Task<bool> HandleTaskAsync(TaskModel model, CancellationToken cancellationToken = default)
    {
        var extractTask =  model.Dependencies.FirstOrDefault(x => x.Dependency.Name == TaskRegistry.GetTaskName(typeof(ExtractTask)))?.Dependency?.Data.Deserialize<ExtractTask>();
        if (extractTask?.OutputDocuments == null) return Task.FromResult(false);
        
        return Task.FromResult(true);
    }
}