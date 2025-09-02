---
title: Breaking changes in EF Core 10 (EF10) - EF Core
description: List of breaking changes introduced in Entity Framework Core 10 (EF10)
author: maumar
ms.date: 01/05/2025
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
| [SQL Server json data type used by default on Azure SQL and compatibility level 170](#sqlserver-json-data-type) | Low        |
| [ExecuteUpdateAsync now accepts a regular, non-expression lambda](#ExecuteUpdateAsync-lambda)                   | Low        |

## Low-impact changes

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

> [!NOTE]
> For 10.0.0 rc1, support for the new JSON data type has been temporarily disabled for Azure SQL Database, due to lacking support. These issues are expected to be resolved by the time EF 10.0 is released, and the JSON data type will become the default until then.

#### Why

The new JSON data type introduced by SQL Server is a superior, 1st-class way to store and interact with JSON data in the database; it notably brings significant performance improvements ([see documentation](/sql/t-sql/data-types/json-data-type)). All applications using Azure SQL Database or SQL Server 2025 are encouraged to migrate to the new JSON data type.

#### Mitigations

If you are targeting Azure SQL Databasse and do not wish to transition to the new JSON data type right away, you can configure EF with a compatibility level lower than 170:

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
