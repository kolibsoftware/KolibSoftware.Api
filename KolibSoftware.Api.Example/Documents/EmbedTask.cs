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
    public Guid Rid { get; set; }
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
        var document = await repository.GetByRidAsync(task.Rid, cancellationToken) ?? throw new Exception("Document not found");
        if (string.IsNullOrEmpty(document.Summary)) throw new Exception("Document summary is empty");
        var embedding = await ollamaService.EmbedAsync(document.Summary);
        document.Embedding = embedding;
        await repository.UpdateAsync(document, cancellationToken);
        return true;
    }
}