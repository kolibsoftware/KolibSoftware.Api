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
    public static PropertyBuilder<T> IsUuid<T>(this PropertyBuilder<T> propertyBuilder)
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

    /// <summary>
    /// Configures a string property to use the "tinytext" column type in the database. This is useful for properties that need to store short text values, as the "tinytext" type can help optimize storage for small strings. By using this extension method, you can ensure that the appropriate column type is used for these properties, which can improve performance and reduce storage requirements for short text fields. Additionally, it marks the property as required, which means that it cannot be null in the database.
    /// </summary>
    /// <param name="propertyBuilder"></param>
    /// <returns></returns>
    public static PropertyBuilder<T> IsTinyText<T>(this PropertyBuilder<T> propertyBuilder)
    {
        propertyBuilder.HasColumnType("tinytext");
        return propertyBuilder;
    }

    /// <summary>
    /// Configures a string property to use the "text" column type in the database. This is useful for properties that need to store longer text values, as the "text" type can handle larger strings than the default string mapping. By using this extension method, you can ensure that the appropriate column type is used for these properties, which can improve performance and reduce storage requirements for longer text fields. Additionally, it marks the property as required, which means that it cannot be null in the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyBuilder"></param>
    /// <returns></returns>
    public static PropertyBuilder<T> IsText<T>(this PropertyBuilder<T> propertyBuilder)
    {
        propertyBuilder.HasColumnType("text");
        return propertyBuilder;
    }

    /// <summary>
    /// Configures a string property to use the "mediumtext" column type in the database. This is useful for properties that need to store medium-length text values, as the "mediumtext" type can handle larger strings than the "text" type. By using this extension method, you can ensure that the appropriate column type is used for these properties, which can improve performance and reduce storage requirements for medium-length text fields. Additionally, it marks the property as required, which means that it cannot be null in the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyBuilder"></param>
    /// <returns></returns>
    public static PropertyBuilder<T> IsMediumText<T>(this PropertyBuilder<T> propertyBuilder)
    {
        propertyBuilder.HasColumnType("mediumtext");
        return propertyBuilder;
    }

    /// <summary>
    /// Configures a string property to use the "longtext" column type in the database. This is useful for properties that need to store very long text values, as the "longtext" type can handle larger strings than the "mediumtext" type. By using this extension method, you can ensure that the appropriate column type is used for these properties, which can improve performance and reduce storage requirements for very long text fields. Additionally, it marks the property as required, which means that it cannot be null in the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyBuilder"></param>
    /// <returns></returns>
    public static PropertyBuilder<T> IsLongText<T>(this PropertyBuilder<T> propertyBuilder)
    {
        propertyBuilder.HasColumnType("longtext");
        return propertyBuilder;
    }

    /// <summary>
    /// Configures a property to use the "json" column type in the database. This is useful for properties that need to store JSON data, as the "json" type can handle structured data in a flexible way. By using this extension method, you can ensure that the appropriate column type is used for these properties, which can improve performance and provide better support for querying and indexing JSON data. Additionally, it marks the property as required, which means that it cannot be null in the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyBuilder"></param>
    /// <returns></returns>
    public static PropertyBuilder<T> IsJson<T>(this PropertyBuilder<T> propertyBuilder)
    {
        propertyBuilder.HasColumnType("json");
        return propertyBuilder;
    }

    /// <summary>
    /// Configures a property to use an enum column type in the database. This extension method takes an enum type parameter and sets up the column type to be an enum with the appropriate values based on the names of the enum members. By using this extension method, you can ensure that the appropriate column type is used for properties that represent enum values, which can improve performance and provide better support for querying and indexing enum data. Additionally, it marks the property as required, which means that it cannot be null in the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyBuilder"></param>
    /// <returns></returns>
    public static PropertyBuilder<T> IsEnum<T>(this PropertyBuilder<T> propertyBuilder) where T : struct, Enum
    {
        propertyBuilder.HasColumnType($"enum('{string.Join("','", Enum.GetNames<T>())}')");
        return propertyBuilder;
    }

}