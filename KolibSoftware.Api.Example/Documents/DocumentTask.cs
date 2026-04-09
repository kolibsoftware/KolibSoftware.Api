using KolibSoftware.Api.Infra.Tasks;
using KolibSoftware.Api.Infra.Tasks.Attributes;

namespace KolibSoftware.Api.Example.Documents;

[Task]
public class DocumentTask
{
    public Guid DocumentRid { get; set; }
}

[TaskHandler]
public class DocumentTaskHandler() : ITaskHandler<DocumentTask>
{
    public Task HandleTaskAsync(DocumentTask task, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Document Task: {task.DocumentRid}");
        return Task.CompletedTask;
    }
}