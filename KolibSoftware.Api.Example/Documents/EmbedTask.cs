using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Example.Services;
using KolibSoftware.Api.Infra.Repo;
using KolibSoftware.Api.Infra.Tasks;
using KolibSoftware.Api.Infra.Tasks.Attributes;

namespace KolibSoftware.Api.Example.Documents;

[Task]
public class EmbedTask
{
    public Guid Rid { get; set; }
}

[TaskHandler]
public sealed class EmbedTaskHandler(
    IRepository<DocumentModel> repository,
    OllamaService ollamaService
) : ITaskHandler<EmbedTask>
{

    public async Task HandleTaskAsync(EmbedTask data, CancellationToken cancellationToken = default)
    {
        var document = await repository.GetByRidAsync(data.Rid, cancellationToken) ?? throw new Exception("Document not found");
        if (string.IsNullOrEmpty(document.Summary)) throw new Exception("Document summary is empty");
        var embedding = await ollamaService.EmbedAsync(document.Summary);
        document.Embedding = embedding;
        await repository.UpdateAsync(document, cancellationToken);
    }
}