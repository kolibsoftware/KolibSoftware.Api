using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Infra.Repo;
using Microsoft.AspNetCore.Mvc;

namespace KolibSoftware.Api.Example.Controllers;

[Route("api/[controller]")]
public sealed class DocumentController(
    IQueryableRepository<DocumentModel> repository
) : ControllerBase()
{

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var documents = await repository.GetAllAsync();
        return Ok(documents);
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
        document.UpdatedAt = document.CreatedAt = DateTime.UtcNow;
        document.UpdatedBy = document.CreatedBy = userId;
        await repository.InsertAsync(document);
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
        existingDocument.UpdatedAt = DateTime.UtcNow;
        existingDocument.UpdatedBy = userId;
        await repository.UpdateAsync(existingDocument);
        return NoContent();
    }

    [HttpDelete("{rid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid rid, [FromHeader(Name = "UserId")] Guid userId)
    {
        var document = await repository.GetByRidAsync(rid);
        if (document == null) return NotFound();
        document.DeletedAt = DateTime.UtcNow;
        document.DeletedBy = userId;
        await repository.UpdateAsync(document);
        return NoContent();
    }

}