using KolibSoftware.Api.Infra.Tasks;
using KolibSoftware.Api.Infra.Tasks.Attributes;

namespace KolibSoftware.Api.Example.Documents;

[Task]
public class DocumentTask {}

[TaskHandler]
public class DocumentTaskHandler() : ITaskHandler<DocumentTask>
{
    public Task HandleTaskAsync(DocumentTask task, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}