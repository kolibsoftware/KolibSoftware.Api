using KolibSoftware.Api.Infra.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KolibSoftware.Api.Infra.Data;

/// <summary>
/// Provides extension methods for configuring entity properties and model behavior in Entity Framework Core. These methods include configurations for resource entities, auditable entities (for creation, update, and deletion), and custom database types such as UUIDs and vectors. By using these extension methods, you can ensure consistent configuration across your entities and take advantage of specific database features for optimized storage and performance.
/// </summary>
public static class DataUtils
{

    /// <summary>
    /// Configures an entity to be treated as a resource by setting up the appropriate properties and keys. This method assumes that the entity implements the IResource interface, which includes a Guid property named Rid. By calling this extension method in the entity configuration, you can ensure that the Rid property is configured as a UUID column type in the database and is set as the primary key for the entity. This helps to standardize how resources are identified and stored in the database, making it easier to manage and query these entities.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static EntityTypeBuilder<T> IsResource<T>(this EntityTypeBuilder<T> builder) where T : class, IResource
    {
        builder.Property(x => x.Rid).IsUuid().IsRequired();
        builder.HasIndex(x => x.Rid).IsUnique();
        return builder;
    }

    /// <summary>
    /// Configures an entity to be treated as auditable for creation by setting up the appropriate properties. This method assumes that the entity implements the ICreateAuditable interface, which includes properties for CreatedAt (DateTime) and CreatedBy (Guid). By calling this extension method in the entity configuration, you can ensure that these properties are configured as required fields in the database, with CreatedBy using the UUID column type. This helps to standardize how creation auditing information is stored for entities, making it easier to track when and by whom entities were created. Similar methods are provided for update and delete auditing as well.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static EntityTypeBuilder<T> IsCreateAuditable<T>(this EntityTypeBuilder<T> builder) where T : class, ICreateAuditable
    {
        builder.Property(x => x.CreatedAt).IsUtc().IsRequired();
        builder.Property(x => x.CreatedBy).IsUuid().IsRequired();
        builder.HasIndex(x => x.CreatedBy);
        return builder;
    }

    /// <summary>
    /// Configures an entity to be treated as auditable for updates by setting up the appropriate properties. This method assumes that the entity implements the IUpdateAuditable interface, which includes properties for UpdatedAt (DateTime) and UpdatedBy (Guid). By calling this extension method in the entity configuration, you can ensure that these properties are configured as required fields in the database, with UpdatedBy using the UUID column type. This helps to standardize how update auditing information is stored for entities, making it easier to track when and by whom entities were last updated. Similar methods are provided for creation and delete auditing as well.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static EntityTypeBuilder<T> IsUpdateAuditable<T>(this EntityTypeBuilder<T> builder) where T : class, IUpdateAuditable
    {
        builder.Property(x => x.UpdatedAt).IsUtc().IsRequired();
        builder.Property(x => x.UpdatedBy).IsUuid().IsRequired();
        builder.HasIndex(x => x.UpdatedBy);
        return builder;
    }

    /// <summary>
    /// Configures an entity to be treated as auditable for deletion by setting up the appropriate properties. This method assumes that the entity implements the IDeleteAuditable interface, which includes properties for DeletedAt (DateTime) and DeletedBy (Guid). By calling this extension method in the entity configuration, you can ensure that these properties are configured as required fields in the database, with DeletedBy using the UUID column type. This helps to standardize how deletion auditing information is stored for entities, making it easier to track when and by whom entities were deleted. Similar methods are provided for creation and update auditing as well.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static EntityTypeBuilder<T> IsDeleteAuditable<T>(this EntityTypeBuilder<T> builder) where T : class, IDeleteAuditable
    {
        builder.Property(x => x.DeletedAt).IsUtc();
        builder.Property(x => x.DeletedBy).IsUuid();
        builder.HasIndex(x => x.DeletedBy);
        return builder;
    }

    /// <summary>
    /// Configures the model builder to use the EventModel entity and its related configurations. This method sets up the EventModel entity with the appropriate table name, primary key, properties, and indexes. It also ensures that the event data is stored as JSON and that the created and handled timestamps are stored in UTC. By calling this extension method in the model builder configuration, you can standardize how events are stored and managed in the database.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static ModelBuilder UseEvents(this ModelBuilder builder)
    {
        builder.Entity<EventModel>(entity =>
        {
            entity.ToTable("event");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Rid).IsUuid().IsRequired();
            entity.Property(x => x.Name).IsTinyText().IsRequired();
            entity.Property(x => x.Data).IsJson().IsRequired();
            entity.Property(x => x.CreatedAt).IsUtc().IsRequired();
            entity.Property(x => x.HandledAt).IsUtc();
            entity.Property(x => x.Status).IsEnum().IsRequired();

            entity.HasIndex(x => x.Rid).IsUnique();
        });
        return builder;
    }

    public static ModelBuilder UseTasks(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskModel>(entity =>
        {
            entity.ToTable("task");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Rid).IsUuid().IsRequired();
            entity.Property(e => e.Name).IsTinyText().IsRequired();
            entity.Property(e => e.Data).IsJson().IsRequired();
            entity.Property(e => e.CreatedAt).IsUtc().IsRequired();
            entity.Property(e => e.HandledAt).IsUtc();
            entity.Property(e => e.Status).IsEnum().IsRequired();

            entity.HasIndex(e => e.Rid).IsUnique();

            entity.HasMany(e => e.Dependencies)
                .WithOne(d => d.Dependent)
                .HasForeignKey(d => d.DependentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Dependents)
                .WithOne(d => d.Dependency)
                .HasForeignKey(d => d.DependencyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<TaskDependency>(entity =>
        {
            entity.ToTable("task_dependency");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DependentId).IsRequired();
            entity.Property(e => e.DependencyId).IsRequired();

            entity.HasIndex(e => e.DependentId);
            entity.HasIndex(e => e.DependencyId);

            entity.HasOne(x => x.Dependent)
                .WithMany(t => t.Dependencies)
                .HasForeignKey(e => e.DependentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Dependency)
                .WithMany(t => t.Dependents)
                .HasForeignKey(e => e.DependencyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        return modelBuilder;
    }
}