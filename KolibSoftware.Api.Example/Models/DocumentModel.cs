using KolibSoftware.Api.Infra.Models;

namespace KolibSoftware.Api.Example.Models;

public class DocumentModel : IResource, ICreateAuditable, IUpdateAuditable, IDeleteAuditable
{
    public int Id { get; set; }
    public Guid Rid { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}