---
title: Breaking changes in EF Core 8.0 (EF8) - EF Core
description: Complete list of breaking changes introduced in Entity Framework Core 8.0 (EF8)
author: SamMonoRT
ms.date: 10/04/2024
uid: core/what-is-new/ef-core-8.0/breaking-changes
---

# Breaking changes in EF Core 8 (EF8)

This page documents API and behavior changes that have the potential to break existing applications updating from EF Core 7 to EF Core 8. Make sure to review earlier breaking changes if updating from an earlier version of EF Core:

- [Breaking changes in EF Core 7](xref:core/what-is-new/ef-core-7.0/breaking-changes)
- [Breaking changes in EF Core 6](xref:core/what-is-new/ef-core-6.0/breaking-changes)

## Target Framework

EF Core 8 targets .NET 8. Applications targeting older .NET, .NET Core, and .NET Framework versions will need to update to target .NET 8.

## Summary

| **Breaking change**                                                                                           | **Impact** |
|:--------------------------------------------------------------------------------------------------------------|------------|
| [`Contains` in LINQ queries may stop working on older SQL Server versions](#sqlserver-contains-compatibility) | High       |
| [Possible query performance regressions around `Contains` in LINQ queries](#contains-perf-regression)         | High       |
| [Enums in JSON are stored as ints instead of strings by default](#enums-as-ints)                              | High       |
| [SQL Server `date` and `time` now scaffold to .NET `DateOnly` and `TimeOnly`](#sqlserver-date-time-only)      | Medium     |
| [Boolean columns with a database generated value are no longer scaffolded as nullable](#scaffold-bools)       | Medium     |
| [SQLite `Math` methods now translate to SQL](#sqlite-math)                                                    | Low        |
| [ITypeBase replaces IEntityType in some APIs](#type-base)                                                     | Low        |
| [ValueGenerator expressions must use public APIs](#value-converters)                                          | Low        |
| [ExcludeFromMigrations no longer excludes other tables in a TPC hierarchy](#exclude-from-migrations)          | Low        |
| [Non-shadow integer keys are persisted to Cosmos documents](#persist-to-cosmos)                               | Low        |
| [Relational model is generated in the compiled model](#compiled-relational-model)                             | Low        |
| [Scaffolding may generate different navigation names](#navigation-names)                                      | Low        |
| [Discriminators now have a max length](#discriminators)                                                       | Low        |
| [SQL Server key values are compared case-insensitively](#casekeys)                                            | Low        |
| [Multiple AddDbContext calls are applied in different order](#AddDbContext)                                   | Low        |
| [EntityTypeAttributeConventionBase replaced with TypeAttributeConventionBase](#attributeConventionBase)       | Low        |

## High-impact changes

<a name="sqlserver-contains-compatibility"></a>

### `Contains` in LINQ queries may stop working on older SQL Server versions

[Tracking Issue #13617](https://github.com/dotnet/efcore/issues/13617)

#### Old behavior

EF had specialized support for LINQ queries using `Contains` operator over a parameterized value list:

```c#
var names = new[] { "Blog1", "Blog2" };

var blogs = await context.Blogs
    .Where(b => names.Contains(b.Name))
    .ToArrayAsync();
```

Before EF Core 8.0, EF inserted the parameterized values as constants into the SQL:

```sql
SELECT [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
WHERE [b].[Name] IN (N'Blog1', N'Blog2')
```

#### New behavior

Starting with EF Core 8.0, EF now generates SQL that is more efficient in many cases, but is unsupported on SQL Server 2014 and below:

```sql
SELECT [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
WHERE [b].[Name] IN (
    SELECT [n].[value]
    FROM OPENJSON(@__names_0) WITH ([value] nvarchar(max) '$') AS [n]
)
```

Note that newer SQL Server versions may be configured with an older [compatibility level](/sql/t-sql/statements/alter-database-transact-sql-compatibility-level), also making them incompatible with the new SQL. This can also occur with an Azure SQL database which was migrated from a previous on-premises SQL Server instance, carrying over the old compatibility level.

#### Why

The insertion of constant values into the SQL creates many performance problems, defeating query plan caching and causing unneeded evictions of other queries. The new EF Core 8.0 translation uses the SQL Server [`OPENJSON`](/sql/t-sql/functions/openjson-transact-sql) function to instead transfer the values as a JSON array. This solves the performance issues inherent in the previous technique; however, the `OPENJSON` function is unavailable in SQL Server 2014 and below.

For more information about this change, [see this blog post](https://devblogs.microsoft.com/dotnet/announcing-ef8-preview-4/).

#### Mitigations

If your database is SQL Server 2016 (13.x) or newer, or if you're using Azure SQL, check the configured compatibility level of your database via the following command:

```sql
SELECT name, compatibility_level FROM sys.databases;
```

If the compatibility level is below 130 (SQL Server 2016), consider modifying it to a newer value ([documentation](/sql/t-sql/statements/alter-database-transact-sql-compatibility-level#best-practices-for-upgrading-database-compatibility-leve)).

Otherwise, if your database version really is older than SQL Server 2016, or is set to an old compatibility level which you cannot change for some reason, you can configure EF to revert to the older, pre-8.0 SQL. If you're using EF 9, you can use the newly-introduced <xref:Microsoft.EntityFrameworkCore.Infrastructure.RelationalDbContextOptionsBuilder`2.TranslateParameterizedCollectionsToConstants*>:

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseSqlServer("<CONNECTION STRING>", o => o.TranslateParameterizedCollectionsToConstants())
```

If you're using EF 8, you can achieve the same effect when using SQL Server by configuring EF's SQL compatibility level:

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseSqlServer(@"<CONNECTION STRING>", o => o.UseCompatibilityLevel(120));
```

<a name="contains-perf-regression"></a>

### Possible query performance regressions around `Contains` in LINQ queries

[Tracking Issue #32394](https://github.com/dotnet/efcore/issues/32394)

#### Old behavior

EF had specialized support for LINQ queries using `Contains` operator over a parameterized value list:

```c#
var names = new[] { "Blog1", "Blog2" };

var blogs = await context.Blogs
    .Where(b => names.Contains(b.Name))
    .ToArrayAsync();
```

Before EF Core 8.0, EF inserted the parameterized values as constants into the SQL:

```sql
SELECT [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
WHERE [b].[Name] IN (N'Blog1', N'Blog2')
```

#### New behavior

Starting with EF Core 8.0, EF now generates the following:

```sql
SELECT [b].[Id], [b].[Name]
FROM [Blogs] AS [b]
WHERE [b].[Name] IN (
    SELECT [n].[value]
    FROM OPENJSON(@__names_0) WITH ([value] nvarchar(max) '$') AS [n]
)
```

However, after the release of EF 8 it turned out that while the new SQL is more efficient for most cases, it can be dramatically less efficient in a minority of cases, even causing query timeouts in some cases.

Please see [this comment](https://github.com/dotnet/efcore/issues/32394#issuecomment-2266634632) for a summary of the change in EF 8, the partial mitigations provided in EF 9, and the plan going forward for EF 10.

#### Mitigations

If you're using EF 9, you can use the newly-introduced <xref:Microsoft.EntityFrameworkCore.Infrastructure.RelationalDbContextOptionsBuilder`2.TranslateParameterizedCollectionsToConstants*> to revert the `Contains` translation for all queries back to the pre-8.0 behavior:

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseSqlServer("<CONNECTION STRING>", o => o.TranslateParameterizedCollectionsToConstants())
```

If you're using EF 8, you can achieve the same effect when using SQL Server by configuring EF's SQL compatibility level:

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseSqlServer(@"<CONNECTION STRING>", o => o.UseCompatibilityLevel(120));
```

Finally, you can control the translation on a query-by-query basis using <xref:Microsoft.EntityFrameworkCore.EF.Constant*?displayProperty=nameWithType> as follows:

```c#
var blogs = await context.Blogs
    .Where(b => EF.Constant(names).Contains(b.Name))
    .ToArrayAsync();
```

<a name="enums-as-ints"></a>

### Enums in JSON are stored as ints instead of strings by default

[Tracking Issue #13617](https://github.com/dotnet/efcore/issues/31100)

#### Old behavior

In EF7, [enums mapped to JSON](xref:core/what-is-new/ef-core-7.0/whatsnew#json-columns) are, by default, stored as string values in the JSON document.

#### New behavior

Starting with EF Core 8.0, EF now, by default, maps enums to integer values in the JSON document.

#### Why

EF has always, by default, mapped enums to a numeric column in relational databases. Since EF supports queries where values from JSON interact with values from columns and parameters, it is important that the values in JSON match the values in the non-JSON column.

#### Mitigations

To continue using strings, configure the enum property with a conversion. For example:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>().Property(e => e.Status).HasConversion<string>();
}
```

Or, for all properties of the enum type::

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder.Properties<StatusEnum>().HaveConversion<string>();
}
```

## Medium-impact changes

<a name="sqlserver-date-time-only"></a>

### SQL Server `date` and `time` now scaffold to .NET `DateOnly` and `TimeOnly`

[Tracking Issue #24507](https://github.com/dotnet/efcore/issues/24507)

#### Old behavior

Previously, when scaffolding a SQL Server database with `date` or `time` columns, EF would generate entity properties with types <xref:System.DateTime> and <xref:System.TimeSpan>.

#### New behavior

Starting with EF Core 8.0, `date` and `time` are scaffolded as <xref:System.DateOnly> and <xref:System.TimeOnly>.

#### Why

<xref:System.DateOnly> and <xref:System.TimeOnly> were introduced in .NET 6.0, and are a perfect match for mapping the database date and time types. <xref:System.DateTime> notably contains a time component that goes unused and can cause confusion when mapping it to `date`, and <xref:System.TimeSpan> represents a time interval - possibly including days - rather than a time of day at which an event occurs. Using the new types prevents bugs and confusion, and provides clarity of intent.

#### Mitigations

This change only affects users which regularly re-scaffold their database into an EF code model ("database-first" flow).

It is recommended to react to this change by modifying your code to use the newly scaffolded <xref:System.DateOnly> and <xref:System.TimeOnly> types. However, if that isn't possible, you can edit the scaffolding templates to revert to the previous mapping. To do this, set up the templates as described on [this page](xref:core/managing-schemas/scaffolding/templates). Then, edit the `EntityType.t4` file, find where the entity properties get generated (search for `property.ClrType`), and change the code to the following:

```c#
        var clrType = property.GetColumnType() switch
        {
            "date" when property.ClrType == typeof(DateOnly) => typeof(DateTime),
            "date" when property.ClrType == typeof(DateOnly?) => typeof(DateTime?),
            "time" when property.ClrType == typeof(TimeOnly) => typeof(TimeSpan),
            "time" when property.ClrType == typeof(TimeOnly?) => typeof(TimeSpan?),
            _ => property.ClrType
        };

        usings.AddRange(code.GetRequiredUsings(clrType));

        var needsNullable = Options.UseNullableReferenceTypes && property.IsNullable && !clrType.IsValueType;
        var needsInitializer = Options.UseNullableReferenceTypes && !property.IsNullable && !clrType.IsValueType;
#>
    public <#= code.Reference(clrType) #><#= needsNullable ? "?" : "" #> <#= property.Name #> { get; set; }<#= needsInitializer ? " = null!;" : "" #>
<#
```

<a name="scaffold-bools"></a>

### Boolean columns with a database generated value are no longer scaffolded as nullable

[Tracking Issue #15070](https://github.com/dotnet/efcore/issues/15070)

#### Old behavior

Previously, non-nullable `bool` columns with a database default constraint were scaffolded as nullable `bool?` properties.

#### New behavior

Starting with EF Core 8.0, non-nullable `bool` columns are always scaffolded as non-nullable properties.

#### Why

A `bool` property will not have its value sent to the database if that value is `false`, which is the CLR default. If the database has a default value of `true` for the column, then even though the value of the property is `false`, the value in the database ends up as `true`. However, in EF8, the sentinel used to determine whether a property has a value can be changed. This is done automatically for `bool` properties with a database generated value of `true`, which means that it is no longer necessary to scaffold the properties as nullable.

#### Mitigations

This change only affects users which regularly re-scaffold their database into an EF code model ("database-first" flow).

It is recommended to react to this change by modifying your code to use the non-nullable bool property. However, if that isn't possible, you can edit the scaffolding templates to revert to the previous mapping. To do this, set up the templates as described on [this page](xref:core/managing-schemas/scaffolding/templates). Then, edit the `EntityType.t4` file, find where the entity properties get generated (search for `property.ClrType`), and change the code to the following:

```c#
#>
        var propertyClrType = property.ClrType != typeof(bool)
                              || (property.GetDefaultValueSql() == null && property.GetDefaultValue() != null)
            ? property.ClrType
            : typeof(bool?);
#>
    public <#= code.Reference(propertyClrType) #><#= needsNullable ? "?" : "" #> <#= property.Name #> { get; set; }<#= needsInitializer ? " = null!;" : "" #>
<#
<#
```

## Low-impact changes

<a name="sqlite-math"></a>

### SQLite `Math` methods now translate to SQL

[Tracking Issue #18843](https://github.com/dotnet/efcore/issues/18843)

#### Old Behavior

Previously only the Abs, Max, Min, and Round methods on `Math` were translated to SQL. All other members would be evaluated on the client if they appeared in the final Select expression of a query.

#### New behavior

In EF Core 8.0, all `Math` methods with corresponding [SQLite math functions](https://sqlite.org/lang_mathfunc.html) are translated to SQL.

These math functions have been enabled in the native SQLite library that we provide by default (through our dependency on the SQLitePCLRaw.bundle_e_sqlite3 NuGet package). They have also been enabled in the library provided by SQLitePCLRaw.bundle_e_sqlcipher. If you're using one of these libraries, your application should not be affected by this change.

There is a chance, however, that applications including the native SQLite library by other means may not enable the math functions. In these cases, the `Math` methods will be translated to SQL and encounter *no such function* errors when executed.

#### Why

SQLite added built-in math functions in version 3.35.0. Even though they're disabled by default, they've become pervasive enough that we decided to provide default translations for them in our EF Core SQLite provider.

We also collaborated with Eric Sink on the SQLitePCLRaw project to enable math functions in all of the native SQLite libraries provided as part of that project.

#### Mitigations

The simplest way to fix breaks is, when possible, to enable the math function is the native SQLite library by specifying the [SQLITE_ENABLE_MATH_FUNCTIONS](https://sqlite.org/compile.html#enable_math_functions) compile-time option.

If you don't control compilation of the native library, you can also fix breaks by create the functions yourself at runtime using the [Microsoft.Data.Sqlite](/dotnet/standard/data/sqlite/user-defined-functions) APIs.

```csharp
sqliteConnection
    .CreateFunction<double, double, double>(
        "pow",
        Math.Pow,
        isDeterministic: true);
```

Alternatively, you can force client-evaluation by splitting the Select expression into two parts separated by `AsEnumerable`.

```csharp
// Before
var query = dbContext.Cylinders
    .Select(
        c => new
        {
            Id = c.Id
            // May throw "no such function: pow"
            Volume = Math.PI * Math.Pow(c.Radius, 2) * c.Height
        });

// After
var query = dbContext.Cylinders
    // Select the properties you'll need from the database
    .Select(
        c => new
        {
            c.Id,
            c.Radius,
            c.Height
        })
    // Switch to client-eval
    .AsEnumerable()
    // Select the final results
    .Select(
        c => new
        {
            Id = c.Id,
            Volume = Math.PI * Math.Pow(c.Radius, 2) * c.Height
        });
```

<a name="type-base"></a>

### ITypeBase replaces IEntityType in some APIs

[Tracking Issue #13947](https://github.com/dotnet/efcore/issues/13947)

#### Old behavior

Previously, all mapped structural types were entity types.

#### New behavior

With the introduction of complex types in EF8, some APIs that were previously use an `IEntityType` now use `ITypeBase` so that the APIs can be used with either entity or complex types. This includes:

- `IProperty.DeclaringEntityType` is now obsolete and `IProperty.DeclaringType` should be used instead.
- `IEntityTypeIgnoredConvention` is now obsolete and `ITypeIgnoredConvention` should be used instead.
- `IValueGeneratorSelector.Select` now accepts an `ITypeBase` which may be, but does not have to be an `IEntityType`.

#### Why

With the introduction of complex types in EF8, these APIs can be used with either `IEntityType` or `IComplexType`.

#### Mitigations

The old APIs are obsoleted, but will not be removed until EF10. Code should be updated to use the new APIs ASAP.

<a name="value-converters"></a>

### ValueConverter and ValueComparer expressions must use public APIs for the compiled model

[Tracking Issue #24896](https://github.com/dotnet/efcore/issues/24896)

#### Old behavior

Previously, `ValueConverter` and `ValueComparer` definitions were not included in the compiled model, and so could contain arbitrary code.

#### New behavior

EF now extracts the expressions from the `ValueConverter` and `ValueComparer` objects and includes these C# in the compiled model. This means that these expressions must only use public API.

#### Why

The EF team is gradually moving more constructs into the compiled model to support using EF Core with AOT in the future.

#### Mitigations

Make the APIs used by the comparer public. For example, consider this simple converter:

```csharp
public class MyValueConverter : ValueConverter<string, byte[]>
{
    public MyValueConverter()
        : base(v => ConvertToBytes(v), v => ConvertToString(v))
    {
    }

    private static string ConvertToString(byte[] bytes)
        => ""; // ... TODO: Conversion code

    private static byte[] ConvertToBytes(string chars)
        => Array.Empty<byte>(); // ... TODO: Conversion code
}
```

To use this converter in a compiled model with EF8, the `ConvertToString` and `ConvertToBytes` methods must be made public. For example:

```csharp
public class MyValueConverter : ValueConverter<string, byte[]>
{
    public MyValueConverter()
        : base(v => ConvertToBytes(v), v => ConvertToString(v))
    {
    }

    public static string ConvertToString(byte[] bytes)
        => ""; // ... TODO: Conversion code

    public static byte[] ConvertToBytes(string chars)
        => Array.Empty<byte>(); // ... TODO: Conversion code
}
```

<a name="exclude-from-migrations"></a>

### ExcludeFromMigrations no longer excludes other tables in a TPC hierarchy

[Tracking Issue #30079](https://github.com/dotnet/efcore/issues/30079)

#### Old behavior

Previously, using `ExcludeFromMigrations` on a table in a TPC hierarchy would also exclude other tables in the hierarchy.

#### New behavior

Starting with EF Core 8.0, `ExcludeFromMigrations` does not impact other tables.

#### Why

The old behavior was a bug and prevented migrations from being used to manage hierarchies across projects.

#### Mitigations

Use `ExcludeFromMigrations` explicitly on any other table that should be excluded.<a name="exclude-from-migrations"></a>

<a name="persist-to-cosmos"></a>

### Non-shadow integer keys are persisted to Cosmos documents

[Tracking Issue #31664](https://github.com/dotnet/efcore/issues/31664)

#### Old behavior

Previously, non-shadow integer properties that match the criteria to be a synthesized key property would not be persisted into the JSON document, but were instead re-synthesized on the way out.

#### New behavior

Starting with EF Core 8.0, these properties are now persisted.

#### Why

The old behavior was a bug and prevented properties that match the synthesized key criteria from being persisted to Cosmos.

#### Mitigations

[Exclude the property from the model](xref:core/modeling/entity-properties#included-and-excluded-properties) if its value should not be persisted.
Additionally, you can disable this behavior entirely by setting `Microsoft.EntityFrameworkCore.Issue31664` AppContext switch to `true`, see [AppContext for library consumers](/dotnet/api/system.appcontext#ForConsumers) for more details.

```C#
AppContext.SetSwitch("Microsoft.EntityFrameworkCore.Issue31664", isEnabled: true);
```

<a name="compiled-relational-model"></a>

### Relational model is generated in the compiled model

[Tracking Issue #24896](https://github.com/dotnet/efcore/issues/24896)

#### Old behavior

Previously, the relational model was computed at run-time even when using a compiled model.

#### New behavior

Starting with EF Core 8.0, the relational model is part of the generated compiled model. However, for particularly large models the generated file may fail to compile.

#### Why

This was done to further improve startup time.

#### Mitigations

Edit the generated `*ModelBuilder.cs` file and remove the line `AddRuntimeAnnotation("Relational:RelationalModel", CreateRelationalModel());` as well as the method `CreateRelationalModel()`.

<a name="navigation-names"></a>

### Scaffolding may generate different navigation names

[Tracking Issue #27832](https://github.com/dotnet/efcore/issues/27832)

#### Old behavior

Previously when scaffolding a `DbContext` and entity types from an existing database, the navigation names for relationships were sometimes derived from a common prefix of multiple foreign key column names.

#### New behavior

Starting with EF Core 8.0, common prefixes of column names from a composite foreign key are no longer used to generate navigation names.

#### Why

This is an obscure naming rule which sometimes generates very poor names like, `S`, `Student_`, or even just `_`. Without this rule, strange names are no longer generated, and the naming conventions for navigations are also made simpler, thereby making it easier to understand and predict which names will be generated.

#### Mitigations

The [EF Core Power Tools](https://github.com/ErikEJ/EFCorePowerTools/issues/2143) have an option to keep generating navigations in the old way. Alternatively, the code generated can be fully customized using [T4 templates](xref:core/managing-schemas/scaffolding/templates). This can be used to example the foreign key properties of scaffolding relationships and use whatever rule is appropriate for your code to generate the navigation names you need.

<a name="discriminators"></a>

### Discriminators now have a max length

[Tracking Issue #10691](https://github.com/dotnet/efcore/issues/10691)

#### Old behavior

Previously, discriminator columns created for [TPH inheritance mapping](xref:core/modeling/inheritance) were configured as `nvarchar(max)` on SQL Server/Azure SQL, or the equivalent unbounded string type on other databases.

#### New behavior

Starting with EF Core 8.0, discriminator columns are created with a max length that covers all the known discriminator values. EF will generate a migration to make this change. However, if the discriminator column is constrained in some way -- for example, as part of an index -- then the `AlterColumn` created by Migrations may fail.

#### Why

`nvarchar(max)` columns are inefficient and unnecessary when the lengths of all possible values are known.

#### Mitigations

The column size can be made explicitly unbounded:

```csharp
modelBuilder.Entity<Foo>()
    .Property<string>("Discriminator")
    .HasMaxLength(-1);
```

<a name="casekeys"></a>

### SQL Server key values are compared case-insensitively

[Tracking Issue #27526](https://github.com/dotnet/efcore/issues/27526)

#### Old behavior

Previously, when tracking entities with string keys with the SQL Server/Azure SQL database providers, the key values were compared using the default .NET case-sensitive ordinal comparer.

#### New behavior

Starting with EF Core 8.0, SQL Server/Azure SQL string key values are compared using the default .NET case-insensitive ordinal comparer.

#### Why

By default, SQL Server uses case-insensitive comparisons when comparing foreign key values for matches to principal key values. This means when EF uses case-sensitive comparisons it may not connect a foreign key to a principal key when it should.

#### Mitigations

Case-sensitive comparisons can be used by setting a custom `ValueComparer`. For example:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    var comparer = new ValueComparer<string>(
        (l, r) => string.Equals(l, r, StringComparison.Ordinal),
        v => v.GetHashCode(),
        v => v);

    modelBuilder.Entity<Blog>()
        .Property(e => e.Id)
        .Metadata.SetValueComparer(comparer);

    modelBuilder.Entity<Post>(
        b =>
        {
            b.Property(e => e.Id).Metadata.SetValueComparer(comparer);
            b.Property(e => e.BlogId).Metadata.SetValueComparer(comparer);
        });
}
```

<a name="AddDbContext"></a>

### Multiple AddDbContext calls are applied in different order

[Tracking Issue #32518](https://github.com/dotnet/efcore/issues/32518)

#### Old behavior

Previously, when multiple calls to `AddDbContext`, `AddDbContextPool`, `AddDbContextFactory` or `AddPooledDbContextFactor` were made with the same context type but conflicting configuration, the first one won.

#### New behavior

Starting with EF Core 8.0, the configuration from the last call one will take precedence.

#### Why

This was changed to be consistent with the new method `ConfigureDbContext` that can be used to add configuration either before or after the `Add*` methods.

#### Mitigations

Reverse the order of `Add*` calls.

<a name="attributeConventionBase"></a>

### EntityTypeAttributeConventionBase replaced with TypeAttributeConventionBase

#### New behavior

In EF Core 8.0 `EntityTypeAttributeConventionBase` was renamed to `TypeAttributeConventionBase`.

#### Why

`TypeAttributeConventionBase` represents the functionality better as it now can be used for complex types and entity types.

#### Mitigations

Replace `EntityTypeAttributeConventionBase` usages with `TypeAttributeConventionBase`.
