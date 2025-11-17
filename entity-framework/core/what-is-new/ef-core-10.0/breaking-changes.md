---
title: Breaking changes in EF Core 10 (EF10) - EF Core
description: List of breaking changes introduced in Entity Framework Core 10 (EF10)
author: roji
ms.date: 10/09/2025
uid: core/what-is-new/ef-core-10.0/breaking-changes
---

# Breaking changes in EF Core 10 (EF10)

This page documents API and behavior changes that have the potential to break existing applications updating from EF Core 9 to EF Core 10. Make sure to review earlier breaking changes if updating from an earlier version of EF Core:

- [Breaking changes in EF Core 9](xref:core/what-is-new/ef-core-9.0/breaking-changes)
- [Breaking changes in EF Core 8](xref:core/what-is-new/ef-core-8.0/breaking-changes)
- [Breaking changes in EF Core 7](xref:core/what-is-new/ef-core-7.0/breaking-changes)
- [Breaking changes in EF Core 6](xref:core/what-is-new/ef-core-6.0/breaking-changes)

## Summary

> [!NOTE]
> If you are using Microsoft.Data.Sqlite, please see the [separate section below on Microsoft.Data.Sqlite breaking changes](#MDS-breaking-changes).

| **Breaking change**                                                                                             | **Impact** |
|:--------------------------------------------------------------------------------------------------------------- | -----------|
| [Application Name is now injected into the connection string](#sqlserver-application-name)                      | Low        |
| [SQL Server json data type used by default on Azure SQL and compatibility level 170](#sqlserver-json-data-type) | Low        |
| [Parameterized collections now use multiple parameters by default](#parameterized-collections)                  | Low        |
| [ExecuteUpdateAsync now accepts a regular, non-expression lambda](#ExecuteUpdateAsync-lambda)                   | Low        |
| [Complex type column names are now uniquified](#complex-type-column-uniquification)                             | Low        |
| [Nested complex type properties use full path in column names](#nested-complex-type-column-names)               | Low        |
| [IDiscriminatorPropertySetConvention signature changed](#discriminator-convention-signature)                    | Low        |

## Low-impact changes

<a name="sqlserver-application-name"></a>

### Application Name is now injected into the connection string

[Tracking Issue #35730](https://github.com/dotnet/efcore/issues/35730)

#### New behavior

When a connection string without an `Application Name` is passed to EF, EF now inserts an `Application Name` containing anonymous information about the EF and SqlClient versions being used. In the vast majority of cases, this doesn't impact the application in any way, but can affect behavior in some edge cases. For example, if you connect to the same database with both EF and another non-EF data access technology (e.g. Dapper, ADO.NET), SqlClient will use a different internal connection pool, as EF will now use a different, updated connection string (one where `Application Name` has been injected). If this sort of mixed access is done within a `TransactionScope`, this can cause escalation to a distributed transaction where previously none was necessary, due of the usage of two connection strings which SqlClient identifies as two distinct databases.

#### Mitigations

A mitigation is to simply define an `Application Name` in your connection string. Once one is defined, EF does not overwrite it and the original connection string is preserved exactly as-is.

<a name="sqlserver-json-data-type"></a>

### SQL Server json data type used by default on Azure SQL and compatibility level 170

[Tracking Issue #36372](https://github.com/dotnet/efcore/issues/36372)

#### Old behavior

Previously, when mapping primitive collections or owned types to JSON in the database, the SQL Server provider stored the JSON data in an `nvarchar(max)` column:

```c#
public class Blog
{
    // ...

    // Primitive collection, mapped to nvarchar(max) JSON column
    public string[] Tags { get; set; }
    // Owned entity type mapped to nvarchar(max) JSON column
    public List<Post> Posts { get; set; }
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>().OwnsMany(b => b.Posts, b => b.ToJson());
}
```

For the above, EF previously generated the following table:

```sql
CREATE TABLE [Blogs] (
    ...
    [Tags] nvarchar(max),
    [Posts] nvarchar(max)
);
```

#### New behavior

With EF 10, if you configure EF with <xref:Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseAzureSql*> ([see documentation](xref:core/providers/sql-server/index#usage-and-configuration)), or configure EF with a compatibility level of 170 or above ([see documentation](xref:core/providers/sql-server/index#compatibility-level)), EF will map to the new JSON data type instead:

```sql
CREATE TABLE [Blogs] (
    ...
    [Tags] json
    [Posts] json
);
```

Although the new JSON data type is the recommended way to store JSON data in SQL Server going forward, there may be some behavioral differences when transitioning from `nvarchar(max)`, and some specific querying forms may not be supported. For example, SQL Server does not support the DISTINCT operator over JSON arrays, and queries attempting to do so will fail.

Note that if you have an existing table and are using <xref:Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseAzureSql*>, upgrading to EF 10 will cause a migration to be generated which alters all existing `nvarchar(max)` JSON columns to `json`. This alter operation is supported and should get applied seamlessly and without any issues, but is a non-trivial change to your database.

#### Why

The new JSON data type introduced by SQL Server is a superior, 1st-class way to store and interact with JSON data in the database; it notably brings significant performance improvements ([see documentation](/sql/t-sql/data-types/json-data-type)). All applications using Azure SQL Database or SQL Server 2025 are encouraged to migrate to the new JSON data type.

#### Mitigations

If you are targeting Azure SQL Database and do not wish to transition to the new JSON data type right away, you can configure EF with a compatibility level lower than 170:

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseAzureSql("<connection string>", o => o.UseCompatibilityLevel(160));
}
```

If you're targeting on-premises SQL Server, the default compatibility level with `UseSqlServer` is currently 150 (SQL Server 2019), so the JSON data type is not used.

As an alternative, you can explicitly set the column type on specific properties to be `nvarchar(max)`:

```c#
public class Blog
{
    public string[] Tags { get; set; }
    public List<Post> Posts { get; set; }
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>().PrimitiveCollection(b => b.Tags).HasColumnType("nvarchar(max)");
    modelBuilder.Entity<Blog>().OwnsMany(b => b.Posts, b => b.ToJson().HasColumnType("nvarchar(max)"));
    modelBuilder.Entity<Blog>().ComplexProperty(e => e.Posts, b => b.ToJson());
}
```

<a name="parameterized-collections"></a>

### Parameterized collections now use multiple parameters by default

[Tracking Issue #36368](https://github.com/dotnet/efcore/issues/36368)

#### Old behavior

In EF Core 9 and earlier, parameterized collections in LINQ queries (such as those used with `.Contains()`) were translated to SQL using a JSON array parameter with `OPENJSON` on SQL Server:

```c#
int[] ids = [1, 2, 3];
var blogs = await context.Blogs.Where(b => ids.Contains(b.Id)).ToListAsync();
```

This generated SQL like:

```sql
@__ids_0='[1,2,3]'

SELECT [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
WHERE [b].[Id] IN (
    SELECT [i].[value]
    FROM OPENJSON(@__ids_0) WITH ([value] int '$') AS [i]
)
```

#### New behavior

Starting with EF Core 10.0, parameterized collections are now translated using multiple scalar parameters by default:

```sql
SELECT [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
WHERE [b].[Id] IN (@ids1, @ids2, @ids3)
```

When the number of values in the collection approaches limits (e.g., SQL Server's 2,100 parameter limit), this can cause runtime errors such as `System.DivideByZeroException` or exceed database parameter limits.

EF also "pads" the parameter list to reduce the number of different SQL statements generated. For example, a collection with 8 values generates SQL with 10 parameters, with the extra parameters containing duplicate values.

#### Why

The new default translation provides the query planner with cardinality information about the collection, which can lead to better query plans in many scenarios. The multiple parameter approach balances between plan cache efficiency (by parameterizing) and query optimization (by providing cardinality).

However, different workloads may benefit from different translation strategies depending on collection sizes, query patterns, and database characteristics.

#### Mitigations

If you encounter issues with the new default behavior (such as parameter limit errors or performance regressions), you can configure the translation mode globally:

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseSqlServer("<CONNECTION STRING>", 
            o => o.UseParameterizedCollectionMode(ParameterTranslationMode.Constant));
```

Available modes are:
- `ParameterTranslationMode.MultipleParameters` - The new default (multiple scalar parameters)
- `ParameterTranslationMode.Constant` - Inlines values as constants (pre-EF8 behavior)
- `ParameterTranslationMode.SingleJsonParameter` - Uses JSON array parameter (EF8-9 default)

You can also control the translation on a per-query basis:

```c#
// Use constants instead of parameters for this specific query
var blogs = await context.Blogs.Where(b => EF.Constant(ids).Contains(b.Id)).ToListAsync();
```

For more information about parameterized collection translation, [see the documentation](xref:core/what-is-new/ef-core-10.0/whatsnew#improved-translation-for-parameterized-collection).

<a name="ExecuteUpdateAsync-lambda"></a>

### ExecuteUpdateAsync now accepts a regular, non-expression lambda

[Tracking Issue #32018](https://github.com/dotnet/efcore/issues/32018)

#### Old behavior

Previously, <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteUpdate*> accepted an expression tree argument (`Expression<Func<...>>`) for the column setters.

#### New behavior

Starting with EF Core 10.0, <xref:Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteUpdate*> now accepts a non-expression argument (`Func<...>`) for the column setters. If you were building expression trees to dynamically create the column setters argument, your code will no longer compile - but can be replaced with a much simpler alternative (see below).

#### Why

The fact that the column setters parameter was an expression tree made it quite difficult to do dynamic construction of the column setters, where some setters are only present based on some condition (see Mitigations below for an example).

#### Mitigations

Code that was building expression trees to dynamically create the column setters argument will need to be rewritten - but the result will be much simpler. For example, let's assume we want to update a Blog's Views, but conditionally also its Name. Since the setters argument was an expression tree, code such as the following needed to be written:

```c#
// Base setters - update the Views only
Expression<Func<SetPropertyCalls<Blog>, SetPropertyCalls<Blog>>> setters =
    s => s.SetProperty(b => b.Views, 8);

// Conditionally add SetProperty(b => b.Name, "foo") to setters, based on the value of nameChanged
if (nameChanged)
{
    var blogParameter = Expression.Parameter(typeof(Blog), "b");

    setters = Expression.Lambda<Func<SetPropertyCalls<Blog>, SetPropertyCalls<Blog>>>(
        Expression.Call(
            instance: setters.Body,
            methodName: nameof(SetPropertyCalls<Blog>.SetProperty),
            typeArguments: [typeof(string)],
            arguments:
            [
                Expression.Lambda<Func<Blog, string>>(Expression.Property(blogParameter, nameof(Blog.Name)), blogParameter),
                Expression.Constant("foo")
            ]),
        setters.Parameters);
}

await context.Blogs.ExecuteUpdateAsync(setters);
```

Manually creating expression trees is complicated and error-prone, and made this common scenario much more difficult than it should have been. Starting with EF 10, you can now write the following instead:

```c#
await context.Blogs.ExecuteUpdateAsync(s =>
{
    s.SetProperty(b => b.Views, 8);
    if (nameChanged)
    {
        s.SetProperty(b => b.Name, "foo");
    }
});
```

<a name="complex-type-column-uniquification"></a>

### Complex type column names are now uniquified

[Tracking Issue #4970](https://github.com/dotnet/EntityFramework.Docs/issues/4970)

#### Old behavior

Previously, when mapping complex types to table columns, if multiple properties in different complex types had the same column name, they would silently share the same column.

#### New behavior

Starting with EF Core 10.0, complex type column names are uniquified by appending a number at the end if another column with the same name exists on the table.

#### Why

This prevents data corruption that could occur when multiple properties are unintentionally mapped to the same column.

#### Mitigations

If you need multiple properties to share the same column, configure them explicitly:

```c#
modelBuilder.Entity<Customer>(b =>
{
    b.ComplexProperty(c => c.ShippingAddress, p => p.Property(a => a.Street).HasColumnName("Street"));
    b.ComplexProperty(c => c.BillingAddress, p => p.Property(a => a.Street).HasColumnName("Street"));
});
```

<a name="nested-complex-type-column-names"></a>

### Nested complex type properties use full path in column names

#### Old behavior

Previously, properties on nested complex types were mapped to columns using just the declaring type name. For example, `EntityType.Complex.NestedComplex.Property` was mapped to column `NestedComplex_Property`.

#### New behavior

Starting with EF Core 10.0, properties on nested complex types use the full path to the property as part of the column name. For example, `EntityType.Complex.NestedComplex.Property` is now mapped to column `Complex_NestedComplex_Property`.

#### Why

This provides better column name uniqueness and makes it clearer which property maps to which column.

#### Mitigations

If you need to maintain the old column names, configure them explicitly:

```c#
modelBuilder.Entity<EntityType>()
    .ComplexProperty(e => e.Complex)
    .ComplexProperty(o => o.NestedComplex)
    .Property(c => c.Property)
    .HasColumnName("NestedComplex_Property");
```

<a name="discriminator-convention-signature"></a>

### IDiscriminatorPropertySetConvention signature changed

#### Old behavior

Previously, `IDiscriminatorPropertySetConvention.ProcessDiscriminatorPropertySet` took `IConventionEntityTypeBuilder` as a parameter.

#### New behavior

Starting with EF Core 10.0, the method signature changed to take `IConventionTypeBaseBuilder` instead of `IConventionEntityTypeBuilder`.

#### Why

This change allows the convention to work with both entity types and complex types.

#### Mitigations

Update your custom convention implementations to use the new signature:

```c#
public virtual void ProcessDiscriminatorPropertySet(
    IConventionTypeBaseBuilder typeBaseBuilder, // Changed from IConventionEntityTypeBuilder
    string name,
    Type type,
    MemberInfo memberInfo,
    IConventionContext<IConventionProperty> context)
```

<a name="MDS-breaking-changes"></a>

## Microsoft.Data.Sqlite breaking changes

### Summary

| **Breaking change**                                                                                       | **Impact** |
|:----------------------------------------------------------------------------------------------------------|------------|
| [Using GetDateTimeOffset without an offset now assumes UTC](#DateTimeOffset-read)                         | High       |
| [Writing DateTimeOffset into REAL column now writes in UTC](#DateTimeOffset-write)                        | High       |
| [Using GetDateTime with an offset now returns value in UTC](#DateTime-read)                               | High       |

### High-impact changes

<a name="DateTimeOffset-read"></a>

#### Using GetDateTimeOffset without an offset now assumes UTC

[Tracking Issue #36195](https://github.com/dotnet/efcore/issues/36195)

##### Old behavior

Previously, when using `GetDateTimeOffset` on a textual timestamp that did not have an offset (e.g., `2014-04-15 10:47:16`), Microsoft.Data.Sqlite would assume the value was in the local time zone. I.e. the value was parsed as `2014-04-15 10:47:16+02:00` (assuming local time zone was UTC+2).

##### New behavior

Starting with Microsoft.Data.Sqlite 10.0, when using `GetDateTimeOffset` on a textual timestamp that does not have an offset, Microsoft.Data.Sqlite will assume the value is in UTC.

##### Why

Is is to align with SQLite's behavior where timestamps without an offset are treated as UTC.

##### Mitigations

Code should be adjusted accordingly.

As a last/temporary resort, you can revert to previous behavior by setting `Microsoft.Data.Sqlite.Pre10TimeZoneHandling` AppContext switch to `true`, see [AppContext for library consumers](/dotnet/api/system.appcontext#ForConsumers) for more details.

```C#
AppContext.SetSwitch("Microsoft.Data.Sqlite.Pre10TimeZoneHandling", isEnabled: true);
```

<a name="DateTimeOffset-write"></a>

#### Writing DateTimeOffset into REAL column now writes in UTC

[Tracking Issue #36195](https://github.com/dotnet/efcore/issues/36195)

##### Old behavior

Previously, when writing a `DateTimeOffset` value into a REAL column, Microsoft.Data.Sqlite would write the value without taking the offset into account.

##### New behavior

Starting with Microsoft.Data.Sqlite 10.0, when writing a `DateTimeOffset` value into a REAL column, Microsoft.Data.Sqlite will convert the value to UTC before doing the conversions and writing it.

##### Why

The value written was incorrect, not aligning with SQLite's behavior where REAL timestamps are asummed to be UTC.

##### Mitigations

Code should be adjusted accordingly.

As a last/temporary resort, you can revert to previous behavior by setting `Microsoft.Data.Sqlite.Pre10TimeZoneHandling` AppContext switch to `true`, see [AppContext for library consumers](/dotnet/api/system.appcontext#ForConsumers) for more details.

```C#
AppContext.SetSwitch("Microsoft.Data.Sqlite.Pre10TimeZoneHandling", isEnabled: true);
```

<a name="DateTime-read"></a>

#### Using GetDateTime with an offset now returns value in UTC

[Tracking Issue #36195](https://github.com/dotnet/efcore/issues/36195)

##### Old behavior

Previously, when using `GetDateTime` on a textual timestamp that had an offset (e.g., `2014-04-15 10:47:16+02:00`), Microsoft.Data.Sqlite would return the value with `DateTimeKind.Local` (even if the offset was not local). The time was parsed correctly taking the offset into account.

##### New behavior

Starting with Microsoft.Data.Sqlite 10.0, when using `GetDateTime` on a textual timestamp that has an offset, Microsoft.Data.Sqlite will convert the value to UTC and return it with `DateTimeKind.Utc`.

##### Why

Even though the time was parsed correctly it was dependent on the machine-configured local time zone, which could lead to unexpected results.

##### Mitigations

Code should be adjusted accordingly.

As a last/temporary resort, you can revert to previous behavior by setting `Microsoft.Data.Sqlite.Pre10TimeZoneHandling` AppContext switch to `true`, see [AppContext for library consumers](/dotnet/api/system.appcontext#ForConsumers) for more details.

```C#
AppContext.SetSwitch("Microsoft.Data.Sqlite.Pre10TimeZoneHandling", isEnabled: true);
```
