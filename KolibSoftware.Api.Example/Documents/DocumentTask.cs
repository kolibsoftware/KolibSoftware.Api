using KolibSoftware.Api.Infra.Tasks;
using KolibSoftware.Api.Infra.Tasks.Attributes;

namespace KolibSoftware.Api.Example.Documents;

[Task]
public class DocumentTask
{
    public string Path { get; set; } = string.Empty;
    public string Stage { get; set; } = "Init";
}

[TaskHandler]
public class DocumentTaskHandler(

) : ITaskHandler<DocumentTask>
{
    public async Task<ITaskResult> HandleTaskAsync(DocumentTask task, IEnumerable<object> dependencies, CancellationToken cancellationToken = default)
    {
        switch (task.Stage)
        {
            case "Init":
                task.Stage = "Extract";
                var extractTask = new ExtractTask { Path = task.Path };
                return TaskResult.Waiting(task, [extractTask]);
            case "Extract":
                var extractResult = dependencies.OfType<ExtractTask>().FirstOrDefault() ?? throw new Exception("Extract task result not found");
                task.Stage = "Summarize";
                var summaryTask = new SummaryTask { Documents = extractResult.DocumentIds.ToDictionary(id => id, id => false) };
                return TaskResult.Waiting(task, [summaryTask]);
            case "Summarize":
                var summaryResult = dependencies.OfType<SummaryTask>().FirstOrDefault() ?? throw new Exception("Summary task result not found");
                task.Stage = "Embed";
                var embedTask = new EmbedTask { Documents = summaryResult.Documents.Select(x => x.Key).ToDictionary(id => id, id => false) };
                return TaskResult.Waiting(task, [embedTask]);
            case "Embed":
                return TaskResult.Completed(task);
            default:
                throw new Exception("Unknown stage");
        }
    }
}