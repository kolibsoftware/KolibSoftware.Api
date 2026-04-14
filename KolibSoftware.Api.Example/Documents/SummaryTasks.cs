using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Example.Services;
using KolibSoftware.Api.Infra.Repo;
using KolibSoftware.Api.Infra.Tasks;
using KolibSoftware.Api.Infra.Tasks.Attributes;

namespace KolibSoftware.Api.Example.Documents;

[Task]
public class SummaryTask
{
    public string Progress { get; set; } = "0%";
    public IDictionary<Guid, bool> Documents { get; set; } = new Dictionary<Guid, bool>();
}

[TaskHandler]
public sealed class SummaryTaskHandler(
    IRepository<DocumentModel> repository,
    OpenAIService summaryService
) : ITaskHandler<SummaryTask>
{

    public async Task<ITaskResult> HandleTaskAsync(SummaryTask data, IEnumerable<object> dependencies, CancellationToken cancellationToken = default)
    {
        if (!data.Documents.Any()) throw new Exception("No input document IDs provided");

        var nextId = data.Documents.FirstOrDefault(x => !x.Value).Key;
        var document = await repository.GetByRidAsync(nextId, cancellationToken) ?? throw new Exception("Document not found");

        if (string.IsNullOrEmpty(document.Content)) throw new Exception("Document content is empty");
        var summary = await summaryService.GenerateAsync($"Clean the following text, then summarize it including only the key points and critical data on 100 words or less:\n\n{document.Content}", cancellationToken);
        document.Summary = summary;
        await repository.UpdateAsync(document, cancellationToken);

        data.Documents[nextId] = true;
        data.Progress = $"{data.Documents.Count(x => x.Value) / (float)data.Documents.Count * 100:0.##}%";
        return data.Documents.All(x => x.Value) ? TaskResult.Completed(data) : TaskResult.Suspended(data);
    }
}