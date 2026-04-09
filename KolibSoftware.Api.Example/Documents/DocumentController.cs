using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Example.Services;
using KolibSoftware.Api.Infra.Data;
using KolibSoftware.Api.Infra.Events;
using KolibSoftware.Api.Infra.Models;
using KolibSoftware.Api.Infra.Repo;
using KolibSoftware.Api.Infra.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KolibSoftware.Api.Example.Documents;

[Route("api/[controller]")]
public sealed class DocumentController(
    IQueryableRepository<DocumentModel> repository,
    IEventService eventService,
    ITaskService taskService,
    OllamaService ollamaService,
    BitNetService bitNetService,
    ApiDbContext context
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

    [HttpPost("upload")]
    public async Task<IActionResult> Upload()
    {
        Directory.CreateDirectory("files");
        var path = Path.Combine("files", $"{Guid.NewGuid()}.pdf");
        using (var stream = new FileStream(path, FileMode.Create))
            await Request.Body.CopyToAsync(stream);
        await taskService.PublishAsync(new ExtractTask { Path = path });
        return Ok();
    }

    [HttpGet("search")]
    public async Task<IActionResult> QueryTextDocuments([FromQuery] string query)
    {
        var queryEmbedding = await ollamaService.EmbedAsync(query);
        var documents = await context.Documents
            .OrderBy(d => DatabaseUtils.VecDistance(d.Embedding, queryEmbedding))
            .Take(5)
            .ToListAsync();
        var prompt = $"Query: {query}\n\nTop 5 relevant documents:\n {string.Join("\n\n", documents.Select(d => $"- {d.Summary}"))}\n\nGenerate a response based on the query and the relevant documents.";
        var response = await bitNetService.GenerateAsync(prompt);
        return Ok(new
        {
            response,
            documents = documents.Select(d => new
            {
                d.Rid,
                d.Title,
                d.Summary,
            })
        });
    }
}