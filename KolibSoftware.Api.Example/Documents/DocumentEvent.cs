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
    }
}