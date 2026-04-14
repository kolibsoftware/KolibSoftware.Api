using System.Text.Json;
using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Example.Services;
using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Repo;
using KolibSoftware.Api.Infra.Tasks;
using KolibSoftware.Api.Infra.Tasks.Attributes;

namespace KolibSoftware.Api.Example.Documents;

[Task]
public class SummaryTask
{
    public IEnumerable<Guid> InputDocuments { get; set; } = [];
    public IEnumerable<Guid> OutputDocuments { get; set; } = [];
}

[TaskHandler<SummaryTask>]
public sealed class SummaryTaskHandler(
    IRepository<DocumentModel> repository,
    BitNetService bitNetService
) : ITaskHandler
{

    public async Task<bool> HandleTaskAsync(TaskModel model, CancellationToken cancellationToken = default)
    {
        var task = model.Data.Deserialize<SummaryTask>() ?? throw new InvalidOperationException("Failed to deserialize task data");
        var nextDocumentId = task.InputDocuments.FirstOrDefault(x => !task.OutputDocuments.Contains(x));
        if (nextDocumentId == Guid.Empty) return true;

        var document = await repository.GetByRidAsync(nextDocumentId, cancellationToken) ?? throw new Exception("Document not found");
        if (string.IsNullOrEmpty(document.Content)) throw new Exception("Document content is empty");
        var summary = await bitNetService.GenerateAsync($"Summarize the following text including only the key points and critical data:\n\n{document.Content}");
        document.Summary = summary;
        await repository.UpdateAsync(document, cancellationToken);
        task.OutputDocuments = task.OutputDocuments.Append(document.Rid);

        model.Data = JsonSerializer.Serialize(task);
        return task.InputDocuments.Count() == task.OutputDocuments.Count();
    }
}