using KolibSoftware.Api.Example.Models;
using KolibSoftware.Api.Infra.Data;
using KolibSoftware.Api.Infra.Models;
using Microsoft.EntityFrameworkCore;

namespace KolibSoftware.Api.Example;

public class ApiDbContext(DbContextOptions<ApiDbContext> options) : DbContext(options)
{

    public DbSet<DocumentModel> Documents { get; set; }
    public DbSet<EventModel> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseEvents();
        modelBuilder.Entity<DocumentModel>(entity =>
        {
            entity.ToTable("document");
            entity.HasKey(e => e.Id);

            entity.IsResource()
                .IsCreateAuditable()
                .IsUpdateAuditable()
                .IsDeleteAuditable();
        });
    }
}