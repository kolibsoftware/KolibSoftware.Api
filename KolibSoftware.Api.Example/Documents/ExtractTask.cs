using System.Text.Json;
using Docnet.Core;
using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Repo;
using KolibSoftware.Api.Infra.Tasks;
using KolibSoftware.Api.Infra.Tasks.Attributes;
using Tesseract;

namespace KolibSoftware.Api.Example.Documents;

[Task]
public class ExtractTask
{
    public string InputPath { get; set; } = string.Empty;
    public IEnumerable<Guid>? OutputDocuments { get; set; }
}

[TaskHandler<ExtractTask>]
public sealed class ExtractTaskHandler(
    IRepository<DocumentModel> repository
) : ITaskHandler
{

    private static async IAsyncEnumerable<string> ExtractTextAsync(byte[] pdf)
    {
        using var engine = new TesseractEngine("tessdata", "eng", EngineMode.Default);
        var rawTexts = DocLib.Instance.ExtractTextsAsync(pdf);
        var visualTexts = engine.ExtractVisualTextsAsync(pdf);

        var results = rawTexts.Zip(visualTexts, (raw, visual) =>
        {
            if (visual.Confidence > 9.0) return visual.Text;
            if (raw.Length > visual.Text.Length) return raw;
            return visual.Text;
        });

        await foreach (var text in results)
            yield return text;
    }

    public async Task<bool> HandleTaskAsync(TaskModel model, CancellationToken cancellationToken = default)
    {
        var task = model.Data.Deserialize<ExtractTask>() ?? throw new InvalidOperationException("Failed to deserialize task data");
        if (!File.Exists(task.InputPath)) throw new FileNotFoundException("File not found", task.InputPath);
        var bytes = File.ReadAllBytes(task.InputPath);
        var documentIds = new List<Guid>();
        await foreach (var text in ExtractTextAsync(bytes))
        {
            var document = new DocumentModel
            {
                Rid = Guid.NewGuid(),
                Title = $"Document extracted from {Path.GetFileName(task.InputPath)}",
                Content = text
            };
            await repository.InsertAsync(document, cancellationToken);
            documentIds.Add(document.Rid);
        }
        task.OutputDocuments = documentIds;
        model.Data = JsonSerializer.Serialize(task);
        return true;
    }
}