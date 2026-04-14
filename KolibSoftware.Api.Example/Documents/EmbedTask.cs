using System.Text.Json;
using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Example.Services;
using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Repo;
using KolibSoftware.Api.Infra.Tasks;
using KolibSoftware.Api.Infra.Tasks.Attributes;

namespace KolibSoftware.Api.Example.Documents;

[Task]
public class EmbedTask
{
    public IEnumerable<Guid> InputDocuments { get; set; } = [];
    public IEnumerable<Guid> OutputDocuments { get; set; } = [];
}

[TaskHandler<EmbedTask>]
public sealed class EmbedTaskHandler(
    IRepository<DocumentModel> repository,
    OllamaService ollamaService
) : ITaskHandler
{

    public async Task<bool> HandleTaskAsync(TaskModel model, CancellationToken cancellationToken = default)
    {
        var task = model.Data.Deserialize<EmbedTask>() ?? throw new InvalidOperationException("Failed to deserialize task data");

        var nextDocumentId = task.InputDocuments.FirstOrDefault(x => !task.OutputDocuments.Contains(x));
        if (nextDocumentId == Guid.Empty) return true;

        var document = await repository.GetByRidAsync(nextDocumentId, cancellationToken) ?? throw new Exception("Document not found");
        if (string.IsNullOrEmpty(document.Summary)) throw new Exception("Document summary is empty");
        var embedding = await ollamaService.EmbedAsync(document.Summary);
        document.Embedding = embedding;
        await repository.UpdateAsync(document, cancellationToken);
        task.OutputDocuments = task.OutputDocuments.Append(document.Rid);

        model.Data = JsonSerializer.Serialize(task);
        return task.InputDocuments.Count() == task.OutputDocuments.Count();
    }
}