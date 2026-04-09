using Docnet.Core;
using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Infra.Repo;
using KolibSoftware.Api.Infra.Tasks;
using KolibSoftware.Api.Infra.Tasks.Attributes;
using Tesseract;

namespace KolibSoftware.Api.Example.Documents;

[Task]
public class ExtractTask
{
    public string Path { get; set; } = string.Empty;
}

[TaskHandler]
public sealed class ExtractTaskHandler(
    IRepository<DocumentModel> repository,
    ITaskService taskService
) : ITaskHandler<ExtractTask>
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

    public async Task HandleTaskAsync(ExtractTask data, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(data.Path)) throw new FileNotFoundException("File not found", data.Path);
        var bytes = File.ReadAllBytes(data.Path);
        await foreach (var text in ExtractTextAsync(bytes))
        {
            var document = new DocumentModel
            {
                Rid = Guid.NewGuid(),
                Title = text.Length > 100 ? text[..100] : text,
                Content = text
            };
            await repository.InsertAsync(document, cancellationToken);
            var summaryTask = await taskService.PublishAsync(new SummaryTask { Rid = document.Rid }, cancellationToken);
            var embedTask = await taskService.PublishAsync(new EmbedTask { Rid = document.Rid }, [summaryTask], cancellationToken);
        }
    }
}