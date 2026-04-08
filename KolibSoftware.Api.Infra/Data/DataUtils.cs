using KolibSoftware.Api.Infra.Models;
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
        builder.Property(x => x.Rid).IsUuid();
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
        builder.Property(x => x.CreatedAt).IsUtc();
        builder.Property(x => x.CreatedBy).IsUuid();
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
        builder.Property(x => x.UpdatedAt).IsUtc();
        builder.Property(x => x.UpdatedBy).IsUuid();
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
}