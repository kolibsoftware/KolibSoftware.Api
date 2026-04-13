using System.Text.Json;
using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Infra.Events;
using KolibSoftware.Api.Infra.Events.Attributes;
using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Tasks;

namespace KolibSoftware.Api.Example.Documents;

[Event("DocumentEvent")]
public sealed class DocumentEvent
{
    public DocumentModel Document { get; set; } = default!;
    public string Action { get; set; } = default!;
}

[EventHandler<DocumentEvent>]
public sealed class DocumentEventHandler(
    ITaskService taskService
) : IEventHandler
{
    public async Task HandleEventAsync(EventModel model, CancellationToken cancellationToken = default)
    {
        var @event = model.Data.Deserialize<DocumentEvent>() ?? throw new InvalidOperationException("Failed to deserialize event data to DocumentEvent");
        if (@event.Document == null) return;
        if (@event.Action != "Created" && @event.Action != "Updated") return;
    }
}