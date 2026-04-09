using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Infra.Events.Attributes;

namespace KolibSoftware.Api.Example.Documents;

[Event]
public sealed class DocumentEvent
{
    public DocumentModel Document { get; set; } = default!;
    public string Action { get; set; } = default!;
}