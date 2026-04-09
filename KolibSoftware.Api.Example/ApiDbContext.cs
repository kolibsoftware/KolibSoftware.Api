using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Infra.Data;
using KolibSoftware.Api.Infra.Models;
using Microsoft.EntityFrameworkCore;

namespace KolibSoftware.Api.Example;

public class ApiDbContext(DbContextOptions<ApiDbContext> options) : DbContext(options)
{

    public DbSet<EventModel> Events { get; set; }
    public DbSet<TaskModel> Tasks { get; set; }

    public DbSet<DocumentModel> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseEvents();
        modelBuilder.UseTasks();
        modelBuilder.UseVectors();
        modelBuilder.Entity<DocumentModel>(entity =>
        {
            entity.ToTable("document");
            entity.HasKey(e => e.Id);

            entity.IsResource()
                .IsCreateAuditable()
                .IsUpdateAuditable()
                .IsDeleteAuditable();

            entity.Property(e => e.Title).IsTinyText().IsRequired();
            entity.Property(e => e.Content).IsText().IsRequired();
            entity.Property(e => e.Embedding).IsVector(DocumentModel.EmptyEmbedding.Length).IsRequired();

            // entity.HasIndex(e => e.Embedding).IsVector();
        });
    }
}