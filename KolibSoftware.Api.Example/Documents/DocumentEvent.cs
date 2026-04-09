using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Infra.Events;
using KolibSoftware.Api.Infra.Events.Attributes;
using KolibSoftware.Api.Infra.Tasks;

namespace KolibSoftware.Api.Example.Documents;

[Event]
public sealed class DocumentEvent
{
    public DocumentModel Document { get; set; } = default!;
    public string Action { get; set; } = default!;
}

[EventHandler]
public sealed class DocumentEventHandler(
    ITaskService taskService
) : IEventHandler<DocumentEvent>
{
    public async Task HandleEventAsync(DocumentEvent @event, CancellationToken cancellationToken = default)
    {
        if (@event.Document == null) return;
        if (@event.Action != "Created" && @event.Action != "Updated") return;
        var task = await taskService.PublishAsync(new DocumentTask { DocumentRid = @event.Document.Rid }, cancellationToken);
        Console.WriteLine($"Document Event: {@event.Action} - Task Id: {task.Id}");
    }
}