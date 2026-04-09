using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Infra.Events;
using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Repo;
using Microsoft.AspNetCore.Mvc;

namespace KolibSoftware.Api.Example.Controllers;

[Route("api/[controller]")]
public sealed class DocumentController(
    IQueryableRepository<DocumentModel> repository,
    IEventService eventService
) : ControllerBase()
{

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DocumentQuery query)
    {
        var documents = await repository.GetAllAsync(query);
        return Ok(documents);
    }

    [HttpGet("page")]
    public async Task<IActionResult> GetPage([FromQuery] DocumentQuery query)
    {
        var page = await repository.GetPageAsync(query);
        return Ok(page);
    }

    [HttpGet("{rid}")]
    public async Task<IActionResult> GetById(Guid rid)
    {
        var document = await repository.GetByRidAsync(rid);
        if (document == null) return NotFound();
        return Ok(document);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromHeader(Name = "UserId")] Guid userId, [FromBody] DocumentModel document)
    {
        document.Rid = Guid.NewGuid();
        document.MarkAsCreated(userId);
        await repository.InsertAsync(document);
        await eventService.PublishAsync(new DocumentEvent { Document = document, Action = "Created" });
        return CreatedAtAction(nameof(GetById), new { rid = document.Rid }, document);
    }

    [HttpPut("{rid}")]
    public async Task<IActionResult> Update([FromRoute] Guid rid, [FromHeader(Name = "UserId")] Guid userId, [FromBody] DocumentModel document)
    {
        if (rid != document.Rid) return BadRequest();
        var existingDocument = await repository.GetByRidAsync(rid);
        if (existingDocument == null) return NotFound();
        existingDocument.Title = document.Title;
        existingDocument.Content = document.Content;
        existingDocument.MarkAsUpdated(userId);
        await repository.UpdateAsync(existingDocument);
        await eventService.PublishAsync(new DocumentEvent { Document = existingDocument, Action = "Updated" });
        return NoContent();
    }

    [HttpDelete("{rid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid rid, [FromHeader(Name = "UserId")] Guid userId)
    {
        var document = await repository.GetByRidAsync(rid);
        if (document == null) return NotFound();
        document.MarkAsDeleted(userId);
        await repository.UpdateAsync(document);
        await eventService.PublishAsync(new DocumentEvent { Document = document, Action = "Deleted" });
        return NoContent();
    }

}