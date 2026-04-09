using KolibSoftware.Api.Infra.Events;
using KolibSoftware.Api.Infra.Events.Attributes;

namespace KolibSoftware.Api.Example.Controllers;

[EventHandler]
public sealed class DocumentHandler : IEventHandler<DocumentEvent>
{
    public Task HandleEventAsync(DocumentEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Document {@event.Action}: {@event.Document.Title}");
        return Task.CompletedTask;
    }
}