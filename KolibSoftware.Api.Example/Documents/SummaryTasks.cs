using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Example.Services;
using KolibSoftware.Api.Infra.Repo;
using KolibSoftware.Api.Infra.Tasks;
using KolibSoftware.Api.Infra.Tasks.Attributes;

namespace KolibSoftware.Api.Example.Documents;

[Task]
public class SummaryTask
{
    public Guid Rid { get; set; }
}

[TaskHandler]
public sealed class SummaryTaskHandler(
    IRepository<DocumentModel> repository,
    BitNetService bitNetService
) : ITaskHandler<SummaryTask>
{

    public async Task HandleTaskAsync(SummaryTask data, CancellationToken cancellationToken = default)
    {
        var document = await repository.GetByRidAsync(data.Rid, cancellationToken) ?? throw new Exception("Document not found");
        if (string.IsNullOrEmpty(document.Content)) throw new Exception("Document content is empty");
        var summary = await bitNetService.GenerateAsync($"Summarize the following text including only the key points and critical data:\n\n{document.Content}");
        document.Summary = summary;
        await repository.UpdateAsync(document, cancellationToken);
    }
}