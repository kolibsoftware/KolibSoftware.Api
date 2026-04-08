using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KolibSoftware.Api.Infra.Data;

/// <summary>
/// Utility class for database-related extensions and configurations. This class provides extension methods for configuring entity properties (e.g., string types, UUIDs, vectors) and for applying global configurations (e.g., using UTC for DateTime properties). It also includes a method for defining a custom database function for vector distance calculations. These utilities help standardize the way entities are configured and how certain data types are handled across the application.
/// </summary>
public static class DatabaseUtils
{

    /// <summary>
    /// Configures a Guid property to use the "uuid" column type in the database. This is useful for properties that need to store UUID values, which are commonly used as unique identifiers. By using this extension method, you can ensure that the appropriate column type is used for these properties, which can help optimize storage and performance for UUID fields. Additionally, it adds an annotation to indicate that this column is intended to store UUID values, which can be useful for tooling and migrations.
    /// </summary>
    /// <param name="propertyBuilder"></param>
    /// <returns></returns>
    public static PropertyBuilder<Guid> IsUuid(this PropertyBuilder<Guid> propertyBuilder)
    {
        propertyBuilder.HasColumnType("uuid").IsRequired();
        return propertyBuilder;
    }

    /// <summary>
    /// Configures a nullable Guid property to use the "uuid" column type in the database. This is useful for properties that need to store UUID values but can also be null. By using this extension method, you can ensure that the appropriate column type is used for these properties, which can help optimize storage and performance for nullable UUID fields. Additionally, it adds an annotation to indicate that this column is intended to store UUID values, which can be useful for tooling and migrations.
    /// </summary>
    /// <param name="propertyBuilder"></param>
    /// <returns></returns>
    public static PropertyBuilder<Guid?> IsUuid(this PropertyBuilder<Guid?> propertyBuilder)
    {
        propertyBuilder.HasColumnType("uuid");
        return propertyBuilder;
    }

    /// <summary>
    /// Configures a DateTime property to be stored in UTC in the database. This extension method sets up a value converter that converts DateTime values to UTC when saving to the database and converts them back to DateTime with the kind set to Utc when reading from the database. By using this extension method, you can ensure that all DateTime values are consistently stored in UTC, which can help avoid issues with time zones and daylight saving time when working with date and time data across different regions. This is especially important for applications that have users in multiple time zones or that need to perform date and time calculations accurately regardless of the user's location.
    /// </summary>
    /// <param name="propertyBuilder"></param>
    /// <returns></returns>
    public static PropertyBuilder<DateTime> IsUtc(this PropertyBuilder<DateTime> propertyBuilder)
    {
        var converter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        propertyBuilder.HasConversion(converter).IsRequired();
        return propertyBuilder;
    }

    /// <summary>
    /// Configures a nullable DateTime property to be stored in UTC in the database. This extension method sets up a value converter that converts DateTime values to UTC when saving to the database and converts them back to DateTime with the kind set to Utc when reading from the database. By using this extension method, you can ensure that all nullable DateTime values are consistently stored in UTC, which can help avoid issues with time zones and daylight saving time when working with date and time data across different regions. This is especially important for applications that have users in multiple time zones or that need to perform date and time calculations accurately regardless of the user's location.
    /// </summary>
    /// <param name="propertyBuilder"></param>
    /// <returns></returns>
    public static PropertyBuilder<DateTime?> IsUtc(this PropertyBuilder<DateTime?> propertyBuilder)
    {
        var converter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
        propertyBuilder.HasConversion(converter);
        return propertyBuilder;
    }

}