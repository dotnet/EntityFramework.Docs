---
title: Breaking changes in EF Core 5.0 - EF Core
description: Complete list of breaking changes introduced in Entity Framework Core 5.0
author: SamMonoRT
ms.date: 09/21/2022
uid: core/what-is-new/ef-core-5.0/breaking-changes
---

# Breaking changes in EF Core 5.0

The following API and behavior changes have the potential to break existing applications updating to EF Core 5.0.0.

## Summary

| **Breaking change**                                                                                                                   | **Impact** |
|:--------------------------------------------------------------------------------------------------------------------------------------|------------|
| [EF Core 5.0 does not support .NET Framework](#netstandard21)                                                                         | Medium     |
| [IProperty.GetColumnName() is now obsolete](#getcolumnname-obsolete)                                                                  | Medium     |
| [Precision and scale are required for decimals](#decimals)                                                                            | Medium     |
| [Required or non-nullable navigation from principal to dependent has different semantics](#required-dependent)                        | Medium     |
| [Defining query is replaced with provider-specific methods](#defining-query)                                                          | Medium     |
| [Non-null reference navigations are not overwritten by queries](#nonnullreferences)                                                   | Medium     |
| [ToView() is treated differently by migrations](#toview)                                                                              | Medium     |
| [ToTable(null) marks the entity type as not mapped to a table](#totable)                                                              | Medium     |
| [Removed HasGeometricDimension method from SQLite NTS extension](#geometric-sqlite)                                                   | Low        |
| [Azure Cosmos DB: Partition key is now added to the primary key](#cosmos-partition-key)                                               | Low        |
| [Azure Cosmos DB: `id` property renamed to `__id`](#cosmos-id)                                                                        | Low        |
| [Azure Cosmos DB: byte[] is now stored as a base64 string instead of a number array](#cosmos-byte)                                    | Low        |
| [Azure Cosmos DB: GetPropertyName and SetPropertyName were renamed](#cosmos-metadata)                                                 | Low        |
| [Value generators are called when the entity state is changed from Detached to Unchanged, Updated, or Deleted](#non-added-generation) | Low        |
| [IMigrationsModelDiffer now uses IRelationalModel](#relational-model)                                                                 | Low        |
| [Discriminators are read-only](#read-only-discriminators)                                                                             | Low        |
| [Provider-specific EF.Functions methods throw for in-memory provider](#no-client-methods)                                             | Low        |
| [IndexBuilder.HasName is now obsolete](#index-obsolete)                                                                               | Low        |
| [A pluralizer is now included for scaffolding reverse engineered models](#pluralizer)                                                 | Low        |
| [INavigationBase replaces INavigation in some APIs to support skip navigations](#inavigationbase)                                     | Low        |
| [Some queries with correlated collection that also use `Distinct` or `GroupBy` are no longer supported](#collection-distinct-groupby) | Low        |
| [Using a collection of Queryable type in projection is not supported](#queryable-projection)                                          | Low        |

## Medium-impact changes

<a name="netstandard21"></a>

### EF Core 5.0 does not support .NET Framework

[Tracking Issue #15498](https://github.com/dotnet/efcore/issues/15498)

#### Old behavior

EF Core 3.1 targets .NET Standard 2.0, which is supported by .NET Framework.

#### New behavior

EF Core 5.0 targets .NET Standard 2.1, which is not supported by .NET Framework. This means EF Core 5.0 cannot be used with .NET Framework applications.

#### Why

This is part of the wider movement across .NET teams aimed at unification to a single .NET target framework. For more information see [the future of .NET Standard](https://devblogs.microsoft.com/dotnet/the-future-of-net-standard/).

#### Mitigations

.NET Framework applications can continue to use EF Core 3.1, which is a [long-term support (LTS) release](https://dotnet.microsoft.com/platform/support/policy/dotnet-core). Alternately, applications can be updated to use .NET Core 3.1 or .NET 5, both of which support .NET Standard 2.1.

<a name="getcolumnname-obsolete"></a>

### IProperty.GetColumnName() is now obsolete

[Tracking Issue #2266](https://github.com/dotnet/efcore/issues/2266)

#### Old behavior

`GetColumnName()` returned the name of the column that a property is mapped to.

#### New behavior

`GetColumnName()` still returns the name of a column that a property is mapped to, but this behavior is now ambiguous since EF Core 5 supports TPT and simultaneous mapping to a view or a function where these mappings could use different column names for the same property.

#### Why

We marked this method as obsolete to guide users to a more accurate overload - <xref:Microsoft.EntityFrameworkCore.RelationalPropertyExtensions.GetColumnName(Microsoft.EntityFrameworkCore.Metadata.IProperty,Microsoft.EntityFrameworkCore.Metadata.StoreObjectIdentifier@)>.

#### Mitigations

If the entity type is only ever mapped to a single table, and never to views, functions, or multiple tables, the <xref:Microsoft.EntityFrameworkCore.RelationalPropertyExtensions.GetColumnBaseName(Microsoft.EntityFrameworkCore.Metadata.IReadOnlyProperty)> can be used in EF Core 5.0 and 6.0 to obtain the table name. For example:

```csharp
var columnName = property.GetColumnBaseName();
```

In EF Core 7.0, this can again be replaced with the new `GetColumnName`, which behaves as the original did for simple, single table only mappings.

If the entity type may be mapped to views, functions, or multiple tables, then a <xref:Microsoft.EntityFrameworkCore.Metadata.StoreObjectIdentifier> must be obtained to identity the table, view, or function. This can be then be used to get the column name for that store object. For example:

```csharp
var columnName = property.GetColumnName(StoreObjectIdentifier.Table("Users", null)));
```

<a name="decimals"></a>

### Precision and scale are required for decimals

[Tracking Issue #19293](https://github.com/dotnet/efcore/issues/19293)

#### Old behavior

EF Core did not normally set precision and scale on <xref:Microsoft.Data.SqlClient.SqlParameter> objects. This means the full precision and scale was sent to SQL Server, at which point SQL Server would round based on the precision and scale of the database column.

#### New behavior

EF Core now sets precision and scale on parameters using the values configured for properties in the EF Core model. This means rounding now happens in SqlClient. Consequentially, if the configured precision and scale do not match the database precision and scale, then the rounding seen may change.

#### Why

Newer SQL Server features, including Always Encrypted, require that parameter facets are fully specified. In addition, SqlClient made a change to round instead of truncate decimal values, thereby matching the SQL Server behavior. This made it possible for EF Core to set these facets without changing the behavior for correctly configured decimals.

#### Mitigations

Map your decimal properties using a type name that includes precision and scale. For example:

```csharp
public class Blog
{
    public int Id { get; set; }

    [Column(TypeName = "decimal(16, 5)")]
    public decimal Score { get; set; }
}
```

Or use `HasPrecision` in the model building APIs. For example:

```csharp
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>().Property(e => e.Score).HasPrecision(16, 5);
    }
```

<a name="required-dependent"></a>

### Required or non-nullable navigation from principal to dependent has different semantics

[Tracking Issue #17286](https://github.com/dotnet/efcore/issues/17286)

#### Old behavior

Only the navigations to principal could be configured as required. Therefore, using `RequiredAttribute` on the navigation to the dependent (the entity containing the foreign key) or marking it as non-nullable would instead create the foreign key on the defining entity type.

#### New behavior

With the added support for required dependents, it is now possible to mark any reference navigation as required, meaning that in the case shown above the foreign key will be defined on the other side of the relationship and the properties won't be marked as required.

Calling `IsRequired` before specifying the dependent end is now ambiguous:

```csharp
modelBuilder.Entity<Blog>()
    .HasOne(b => b.BlogImage)
    .WithOne(i => i.Blog)
    .IsRequired()
    .HasForeignKey<BlogImage>(b => b.BlogForeignKey);
```

#### Why

The new behavior is necessary to enable support for required dependents ([see #12100](https://github.com/dotnet/efcore/issues/12100)).

#### Mitigations

Remove `RequiredAttribute` from the navigation to the dependent and place it instead on the navigation to the principal or configure the relationship in `OnModelCreating`:

```csharp
modelBuilder.Entity<Blog>()
    .HasOne(b => b.BlogImage)
    .WithOne(i => i.Blog)
    .HasForeignKey<BlogImage>(b => b.BlogForeignKey)
    .IsRequired();
```

<a name="defining-query"></a>

### Defining query is replaced with provider-specific methods

[Tracking Issue #18903](https://github.com/dotnet/efcore/issues/18903)

#### Old behavior

Entity types were mapped to defining queries at the Core level. Anytime the entity type was used in the query root of the entity type was replaced by the defining query for any provider.

#### New behavior

APIs for defining query are deprecated. New provider-specific APIs were introduced.

#### Why

While defining queries were implemented as replacement query whenever query root is used in the query, it had a few issues:

- If defining query is projecting entity type using `new { ... }` in `Select` method, then identifying that as an entity required additional work and made it inconsistent with how EF Core treats nominal types in the query.
- For relational providers `FromSql` is still needed to pass the SQL string in LINQ expression form.

Initially defining queries were introduced as client-side views to be used with In-Memory provider for keyless entities (similar to database views in relational databases). Such definition makes it easy to test application against in-memory database. Afterwards they became broadly applicable, which was useful but brought inconsistent and hard to understand behavior. So we decided to simplify the concept. We made LINQ based defining query exclusive to In-Memory provider and treat them differently. For more information, [see this issue](https://github.com/dotnet/efcore/issues/20023).

#### Mitigations

For relational providers, use `ToSqlQuery` method in `OnModelCreating` and pass in a SQL string to use for the entity type.
For the In-Memory provider, use `ToInMemoryQuery` method in `OnModelCreating` and pass in a LINQ query to use for the entity type.

<a name="nonnullreferences"></a>

### Non-null reference navigations are not overwritten by queries

[Tracking Issue #2693](https://github.com/dotnet/EntityFramework.Docs/issues/2693)

#### Old behavior

In EF Core 3.1, reference navigations eagerly initialized to non-null values would sometimes be overwritten by entity instances from the database, regardless of whether or not key values matched. However, in other cases, EF Core 3.1 would do the opposite and leave the existing non-null value.

#### New behavior

Starting with EF Core 5.0, non-null reference navigations are never overwritten by instances returned from a query.

Note that eager initialization of a _collection_ navigation to an empty collection is still supported.

#### Why

Initialization of a reference navigation property to an "empty" entity instance results in an ambiguous state. For example:

```csharp
public class Blog
{
     public int Id { get; set; }
     public Author Author { get; set; ) = new Author();
}
```

Normally a query for Blogs and Authors will first create `Blog` instances and then set the appropriate `Author` instances based on the data returned from the database. However, in this case every `Blog.Author` property is already initialized to an empty `Author`. Except EF Core has no way to know that this instance is "empty". So overwriting this instance could potentially silently throw away a valid `Author`. Therefore, EF Core 5.0 now consistently does not overwrite a navigation that is already initialized.

This new behavior also aligns with the behavior of EF6 in most cases, although upon investigation we also found some cases of inconsistency in EF6.

#### Mitigations

If this break is encountered, then the fix is to stop eagerly initializing reference navigation properties.

<a name="toview"></a>

### ToView() is treated differently by migrations

[Tracking Issue #2725](https://github.com/dotnet/efcore/issues/2725)

#### Old behavior

Calling `ToView(string)` made the migrations ignore the entity type in addition to mapping it to a view.

#### New behavior

Now `ToView(string)` marks the entity type as not mapped to a table in addition to mapping it to a view. This results in the first migration after upgrading to EF Core 5 to try to drop the default table for this entity type as it's not longer ignored.

#### Why

EF Core now allows an entity type to be mapped to both a table and a view simultaneously, so `ToView` is no longer a valid indicator that it should be ignored by migrations.

#### Mitigations

Use the following code to mark the mapped table as excluded from migrations:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>().ToTable("UserView", t => t.ExcludeFromMigrations());
}
```

<a name="totable"></a>

### ToTable(null) marks the entity type as not mapped to a table

[Tracking Issue #21172](https://github.com/dotnet/efcore/issues/21172)

#### Old behavior

`ToTable(null)` would reset the table name to the default.

#### New behavior

`ToTable(null)` now marks the entity type as not mapped to any table.

#### Why

EF Core now allows an entity type to be mapped to both a table and a view simultaneously, so `ToTable(null)` is used to indicate that it isn't mapped to any table.

#### Mitigations

Use the following code to reset the table name to the default if it's not mapped to a view or a DbFunction:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>().Metadata.RemoveAnnotation(RelationalAnnotationNames.TableName);
}
```

## Low-impact changes

<a name="geometric-sqlite"></a>

### Removed HasGeometricDimension method from SQLite NTS extension

[Tracking Issue #14257](https://github.com/dotnet/efcore/issues/14257)

#### Old behavior

HasGeometricDimension was used to enable additional dimensions (Z and M) on geometry columns. However, it only ever affected database creation. It was unnecessary to specify it to query values with additional dimensions. It also didn't work correctly when inserting or updating values with additional dimensions ([see #14257](https://github.com/dotnet/efcore/issues/14257)).

#### New behavior

To enable inserting and updating geometry values with additional dimensions (Z and M), the dimension needs to be specified as part of the column type name. This API matches more closely to the underlying behavior of SpatiaLite's AddGeometryColumn function.

#### Why

Using HasGeometricDimension after specifying the dimension in the column type is unnecessary and redundant, so we removed HasGeometricDimension entirely.

#### Mitigations

Use `HasColumnType` to specify the dimension:

```csharp
modelBuilder.Entity<GeoEntity>(
    x =>
    {
        // Allow any GEOMETRY value with optional Z and M values
        x.Property(e => e.Geometry).HasColumnType("GEOMETRYZM");

        // Allow only POINT values with an optional Z value
        x.Property(e => e.Point).HasColumnType("POINTZ");
    });
```

<a name="cosmos-partition-key"></a>

### Azure Cosmos DB: Partition key is now added to the primary key

[Tracking Issue #15289](https://github.com/dotnet/efcore/issues/15289)

#### Old behavior

The partition key property was only added to the alternate key that includes `id`.

#### New behavior

The partition key property is now also added to the primary key by convention.

#### Why

This change makes the model better aligned with Azure Cosmos DB semantics and improves the performance of `Find` and some queries.

#### Mitigations

To prevent the partition key property to be added to the primary key, configure it in `OnModelCreating`.

```csharp
modelBuilder.Entity<Blog>()
    .HasKey(b => b.Id);
```

<a name="cosmos-id"></a>

### Azure Cosmos DB: `id` property renamed to `__id`

[Tracking Issue #17751](https://github.com/dotnet/efcore/issues/17751)

#### Old behavior

The shadow property mapped to the `id` JSON property was also named `id`.

#### New behavior

The shadow property created by convention is now named `__id`.

#### Why

This change makes it less likely that the `id` property clashes with an existing property on the entity type.

#### Mitigations

To go back to the 3.x behavior, configure the `id` property in `OnModelCreating`.

```csharp
modelBuilder.Entity<Blog>()
    .Property<string>("id")
    .ToJsonProperty("id");
```

<a name="cosmos-byte"></a>

### Azure Cosmos DB: byte[] is now stored as a base64 string instead of a number array

[Tracking Issue #17306](https://github.com/dotnet/efcore/issues/17306)

#### Old behavior

Properties of type byte[] were stored as a number array.

#### New behavior

Properties of type byte[] are now stored as a base64 string.

#### Why

This representation of byte[] aligns better with expectations and is the default behavior of the major JSON serialization libraries.

#### Mitigations

Existing data stored as number arrays will still be queried correctly, but currently there isn't a supported way to change back the insert behavior. If this limitation is blocking your scenario, comment on [this issue](https://github.com/dotnet/efcore/issues/17306)

<a name="cosmos-metadata"></a>

### Azure Cosmos DB: GetPropertyName and SetPropertyName were renamed

[Tracking Issue #17874](https://github.com/dotnet/efcore/issues/17874)

#### Old behavior

Previously the extension methods were called `GetPropertyName` and `SetPropertyName`

#### New behavior

The old API was removed and new methods added: `GetJsonPropertyName`, `SetJsonPropertyName`

#### Why

This change removes the ambiguity around what these methods are configuring.

#### Mitigations

Use the new API.

<a name="non-added-generation"></a>

### Value generators are called when the entity state is changed from Detached to Unchanged, Updated, or Deleted

[Tracking Issue #15289](https://github.com/dotnet/efcore/issues/15289)

#### Old behavior

Value generators were only called when the entity state changed to Added.

#### New behavior

Value generators are now called when the entity state is changed from Detached to Unchanged, Updated, or Deleted and the property contains the default values.

#### Why

This change was necessary to improve the experience with properties that are not persisted to the data store and have their value generated always on the client.

#### Mitigations

To prevent the value generator from being called, assign a non-default value to the property before the state is changed.

<a name="relational-model"></a>

### IMigrationsModelDiffer now uses IRelationalModel

[Tracking Issue #20305](https://github.com/dotnet/efcore/issues/20305)

#### Old behavior

`IMigrationsModelDiffer` API was defined using `IModel`.

#### New behavior

`IMigrationsModelDiffer` API now uses `IRelationalModel`. However the model snapshot still contains only `IModel` as this code is part of the application and Entity Framework can't change it without making a bigger breaking change.

#### Why

`IRelationalModel` is a newly added representation of the database schema. Using it to find differences is faster and more accurate.

#### Mitigations

Use the following code to compare the model from `snapshot` with the model from `context`:

```csharp
var dependencies = context.GetService<ProviderConventionSetBuilderDependencies>();
var relationalDependencies = context.GetService<RelationalConventionSetBuilderDependencies>();

var typeMappingConvention = new TypeMappingConvention(dependencies);
typeMappingConvention.ProcessModelFinalizing(((IConventionModel)modelSnapshot.Model).Builder, null);

var relationalModelConvention = new RelationalModelConvention(dependencies, relationalDependencies);
var sourceModel = relationalModelConvention.ProcessModelFinalized(snapshot.Model);

var modelDiffer = context.GetService<IMigrationsModelDiffer>();
var hasDifferences = modelDiffer.HasDifferences(
    ((IMutableModel)sourceModel).FinalizeModel().GetRelationalModel(),
    context.Model.GetRelationalModel());
```

We are planning to improve this experience in 6.0 ([see #22031](https://github.com/dotnet/efcore/issues/22031))

<a name="read-only-discriminators"></a>

### Discriminators are read-only

[Tracking Issue #21154](https://github.com/dotnet/efcore/issues/21154)

#### Old behavior

It was possible to change the discriminator value before calling `SaveChanges`

#### New behavior

An exception will be thrown in the above case.

#### Why

EF doesn't expect the entity type to change while it is still being tracked, so changing the discriminator value leaves the context in an inconsistent state, which might result in unexpected behavior.

#### Mitigations

If changing the discriminator value is necessary and the context will be disposed immediately after calling `SaveChanges`, the discriminator can be made mutable:

```csharp
modelBuilder.Entity<BaseEntity>()
    .Property<string>("Discriminator")
    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
```

<a name="no-client-methods"></a>

### Provider-specific EF.Functions methods throw for in-memory provider

[Tracking Issue #20294](https://github.com/dotnet/efcore/issues/20294)

#### Old behavior

Provider-specific EF.Functions methods contained implementation for client execution, which allowed them to be executed on the in-memory provider. For example, `EF.Functions.DateDiffDay` is a Sql Server specific method, which worked on InMemory provider.

#### New behavior

Provider-specific methods have been updated to throw an exception in their method body to block evaluating them on client side.

#### Why

Provider-specific methods map to a database function. The computation done by the mapped database function can't always be replicated on the client side in LINQ. It may cause the result from the server to differ when executing the same method on client. Since these methods are used in LINQ to translate to specific database functions, they don't need to be evaluated on client side. As the in-memory provider is a different _database_, these methods aren't available for this provider. Trying to execute them for InMemory provider, or any other provider that doesn't translate these methods, throws an exception.

#### Mitigations

Since there's no way to mimic behavior of database functions accurately, you should test the queries containing them against same kind of database as in production.

<a name="index-obsolete"></a>

### IndexBuilder.HasName is now obsolete

[Tracking Issue #21089](https://github.com/dotnet/efcore/issues/21089)

#### Old behavior

Previously, only one index could be defined over a given set of properties. The database name of an index was configured using IndexBuilder.HasName.

#### New behavior

Multiple indexes are now allowed on the same set or properties. These indexes are now distinguished by a name in the model. By convention, the model name is used as the database name; however it can also be configured independently using HasDatabaseName.

#### Why

In the future, we'd like to enable both ascending and descending indexes or indexes with different collations on the same set of properties. This change moves us another step in that direction.

#### Mitigations

Any code that was previously calling IndexBuilder.HasName should be updated to call HasDatabaseName instead.

If your project includes migrations generated prior to EF Core version 2.0.0, you can safely ignore the warning in those files and suppress it by adding `#pragma warning disable 612, 618`.

<a name="pluralizer"></a>

### A pluralizer is now included for scaffolding reverse engineered models

[Tracking Issue #11160](https://github.com/dotnet/efcore/issues/11160)

#### Old behavior

Previously, you had to install a separate pluralizer package in order to pluralize DbSet and collection navigation names and singularize table names when scaffolding a DbContext and entity types by reverse engineering a database schema.

#### New behavior

EF Core now includes a pluralizer that uses the [Humanizer](https://github.com/Humanizr/Humanizer) library. This is the same library Visual Studio uses to recommend variable names.

#### Why

Using plural forms of words for collection properties and singular forms for types and reference properties is idiomatic in .NET.

#### Mitigations

To disable the pluralizer, use the `--no-pluralize` option on `dotnet ef dbcontext scaffold` or the `-NoPluralize` switch on `Scaffold-DbContext`.

<a name="inavigationbase"></a>

### INavigationBase replaces INavigation in some APIs to support skip navigations

[Tracking Issue #2568](https://github.com/dotnet/EntityFramework.Docs/issues/2568)

#### Old behavior

EF Core prior to 5.0 supported only one form of navigation property, represented by the `INavigation` interface.

#### New behavior

EF Core 5.0 introduces many-to-many relationships which use "skip navigations". These are represented by the `ISkipNavigation` interface, and most of the functionality of `INavigation` has been pushed down to a common base interface: `INavigationBase`.

#### Why

Most of the functionality between normal and skip navigations is the same. However, skip navigations have a different relationship to foreign keys than normal navigations, since the FKs involved are not directly on either end of the relationship, but rather in the join entity.

#### Mitigations

In many cases applications can switch to using the new base interface with no other changes. However, in cases where the navigation is used to access foreign key properties, application code should either be constrained to only normal navigations, or updated to do the appropriate thing for both normal and skip navigations.

<a name="collection-distinct-groupby"></a>

### Some queries with correlated collection that also use `Distinct` or `GroupBy` are no longer supported

[Tracking Issue #15873](https://github.com/dotnet/efcore/issues/15873)

**Old behavior**

Previously, queries involving correlated collections followed by `GroupBy`, as well as some queries using `Distinct` we allowed to execute.

GroupBy example:

```csharp
context.Parents
    .Select(p => p.Children
        .GroupBy(c => c.School)
        .Select(g => g.Key))
```

`Distinct` example - specifically `Distinct` queries where inner collection projection doesn't contain the primary key:

```csharp
context.Parents
    .Select(p => p.Children
        .Select(c => c.School)
        .Distinct())
```

These queries could return incorrect results if the inner collection contained any duplicates, but worked correctly if all the elements in the inner collection were unique.

**New behavior**

These queries are no longer supported. Exception is thrown indicating that we don't have enough information to correctly build the results.

**Why**

For correlated collection scenarios we need to know entity's primary key in order to assign collection entities to the correct parent. When inner collection doesn't use `GroupBy` or `Distinct`, the missing primary key can simply be added to the projection. However in case of `GroupBy` and `Distinct` it can't be done because it would change the result of `GroupBy` or `Distinct` operation.

**Mitigations**

Rewrite the query to not use `GroupBy` or `Distinct` operations on the inner collection, and perform these operations on the client instead.

```csharp
(await context.Parents
    .Select(p => p.Children.Select(c => c.School))
    .ToListAsync())
    .Select(x => x.GroupBy(c => c).Select(g => g.Key))
```

```csharp
(await context.Parents
    .Select(p => p.Children.Select(c => c.School))
    .ToListAsync())
    .Select(x => x.Distinct())
```

<a name="queryable-projection"></a>

### Using a collection of Queryable type in projection is not supported

[Tracking Issue #16314](https://github.com/dotnet/efcore/issues/16314)

**Old behavior**

Previously, it was possible to use collection of a Queryable type inside the projection in some cases, for example as an argument to a `List<T>` constructor:

```csharp
context.Blogs
    .Select(b => new List<Post>(context.Posts.Where(p => p.BlogId == b.Id)))
```

**New behavior**

These queries are no longer supported. Exception is thrown indicating that we can't create an object of Queryable type and suggesting how this could be fixed.

**Why**

We can't materialize an object of a Queryable type, so they would automatically be created using `List<T>` type instead. This would often cause an exception due to type mismatch which was not very clear and could be surprising to some users. We decided to recognize the pattern and throw a more meaningful exception.

**Mitigations**

Add `ToList()` call after the Queryable object in the projection:

```csharp
context.Blogs.Select(b => context.Posts.Where(p => p.BlogId == b.Id).ToList())
```
