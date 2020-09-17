---
title: Breaking changes in EF Core 5.0 - EF Core
description: Complete list of breaking changes introduced in Entity Framework Core 5.0
author: bricelam
ms.date: 09/09/2020
uid: core/what-is-new/ef-core-5.0/breaking-changes
---

# Breaking changes in EF Core 5.0

The following API and behavior changes have the potential to break existing applications updating to EF Core 5.0.0.

## Summary

| **Breaking change**                                                                                                                   | **Impact** |
|:--------------------------------------------------------------------------------------------------------------------------------------|------------|
| [Required on the navigation from principal to dependent has different semantics](#required-dependent)                                 | Medium     |
| [Defining query is replaced with provider-specific methods](#defining-query)                                                          | Medium     |
| [Removed HasGeometricDimension method from SQLite NTS extension](#geometric-sqlite)                                                   | Low        |
| [Cosmos: Partition key is now added to the primary key](#cosmos-partition-key)                                                        | Low        |
| [Cosmos: `id` property renamed to `__id`](#cosmos-id)                                                                                 | Low        |
| [Cosmos: byte[] is now stored as a base64 string instead of a number array](#cosmos-byte)                                             | Low        |
| [Cosmos: GetPropertyName and SetPropertyName were renamed](#cosmos-metadata)                                                          | Low        |
| [Value generators are called when the entity state is changed from Detached to Unchanged, Updated, or Deleted](#non-added-generation) | Low        |
| [IMigrationsModelDiffer now uses IRelationalModel](#relational-model)                                                                 | Low        |
| [Discriminators are read-only](#read-only-discriminators)                                                                             | Low        |
| [Provider-specific EF.Functions methods throw for InMemory provider](#no-client-methods)                                              | Low        |

<a name="geometric-sqlite"></a>

### Removed HasGeometricDimension method from SQLite NTS extension

[Tracking Issue #14257](https://github.com/aspnet/EntityFrameworkCore/issues/14257)

**Old behavior**

HasGeometricDimension was used to enable additional dimensions (Z and M) on geometry columns. However, it only ever affected database creation. It was unnecessary to specify it to query values with additional dimensions. It also didn't work correctly when inserting or updating values with additional dimensions ([see #14257](https://github.com/aspnet/EntityFrameworkCore/issues/14257)).

**New behavior**

To enable inserting and updating geometry values with additional dimensions (Z and M), the dimension needs to be specified as part of the column type name. This API matches more closely to the underlying behavior of SpatiaLite's AddGeometryColumn function.

**Why**

Using HasGeometricDimension after specifying the dimension in the column type is unnecessary and redundant, so we removed HasGeometricDimension entirely.

**Mitigations**

Use `HasColumnType` to specify the dimension:

```cs
modelBuilder.Entity<GeoEntity>(
    x =>
    {
        // Allow any GEOMETRY value with optional Z and M values
        x.Property(e => e.Geometry).HasColumnType("GEOMETRYZM");

        // Allow only POINT values with an optional Z value
        x.Property(e => e.Point).HasColumnType("POINTZ");
    });
```

<a name="required-dependent"></a>

### Required on the navigation from principal to dependent has different semantics

[Tracking Issue #17286](https://github.com/aspnet/EntityFrameworkCore/issues/17286)

**Old behavior**

Only the navigations to principal could be configured as required. Therefore using `RequiredAttribute` on the navigation to the dependent (the entity containing the foreign key) would instead create the foreign key on the defining entity type.

**New behavior**

With the added support for required dependents, it is now possible to mark any reference navigation as required, meaning that in the case shown above the foreign key will be defined on the other side of the relationship and the properties won't be marked as required.

Calling `IsRequired` before specifying the dependent end is now ambiguous:

```cs
modelBuilder.Entity<Blog>()
    .HasOne(b => b.BlogImage)
    .WithOne(i => i.Blog)
    .IsRequired()
    .HasForeignKey<BlogImage>(b => b.BlogForeignKey);
```

**Why**

The new behavior is necessary to enable support for required dependents ([see #12100](https://github.com/dotnet/efcore/issues/12100)).

**Mitigations**

Remove `RequiredAttribute` from the navigation to the dependent and place it instead on the navigation to the principal or configure the relationship in `OnModelCreating`:

```cs
modelBuilder.Entity<Blog>()
    .HasOne(b => b.BlogImage)
    .WithOne(i => i.Blog)
    .HasForeignKey<BlogImage>(b => b.BlogForeignKey)
    .IsRequired();
```

<a name="cosmos-partition-key"></a>

### Cosmos: Partition key is now added to the primary key

[Tracking Issue #15289](https://github.com/aspnet/EntityFrameworkCore/issues/15289)

**Old behavior**

The partition key property was only added to the alternate key that includes `id`.

**New behavior**

The partition key property is now also added to the primary key by convention.

**Why**

This change makes the model better aligned with Azure Cosmos DB semantics and improves the performance of `Find` and some queries.

**Mitigations**

To prevent the partition key property to be added to the primary key, configure it in `OnModelCreating`.

```cs
modelBuilder.Entity<Blog>()
    .HasKey(b => b.Id);
```

<a name="cosmos-id"></a>

### Cosmos: `id` property renamed to `__id`

[Tracking Issue #17751](https://github.com/aspnet/EntityFrameworkCore/issues/17751)

**Old behavior**

The shadow property mapped to the `id` JSON property was also named `id`.

**New behavior**

The shadow property created by convention is now named `__id`.

**Why**

This change makes it less likely that the `id` property clashes with an existing property on the entity type.

**Mitigations**

To go back to the 3.x behavior, configure the `id` property in `OnModelCreating`.

```cs
modelBuilder.Entity<Blog>()
    .Property<string>("id")
    .ToJsonProperty("id");
```

<a name="cosmos-byte"></a>

### Cosmos: byte[] is now stored as a base64 string instead of a number array

[Tracking Issue #17306](https://github.com/aspnet/EntityFrameworkCore/issues/17306)

**Old behavior**

Properties of type byte[] were stored as a number array.

**New behavior**

Properties of type byte[] are now stored as a base64 string.

**Why**

This representation of byte[] aligns better with expectations and is the default behavior of the major JSON serialization libraries.

**Mitigations**

Existing data stored as number arrays will still be queried correctly, but currently there isn't a supported way to change back the insert behavior. If this limitation is blocking your scenario, comment on [this issue](https://github.com/aspnet/EntityFrameworkCore/issues/17306)

<a name="cosmos-metadata"></a>

### Cosmos: GetPropertyName and SetPropertyName were renamed

[Tracking Issue #17874](https://github.com/aspnet/EntityFrameworkCore/issues/17874)

**Old behavior**

Previously the extension methods were called `GetPropertyName` and `SetPropertyName`

**New behavior**

The old API was obsoleted and new methods added: `GetJsonPropertyName`, `SetJsonPropertyName`

**Why**

This change removes the ambiguity around what these methods are configuring.

**Mitigations**

Use the new API or temporarily suspend the obsolete warnings.

<a name="non-added-generation"></a>

### Value generators are called when the entity state is changed from Detached to Unchanged, Updated, or Deleted

[Tracking Issue #15289](https://github.com/aspnet/EntityFrameworkCore/issues/15289)

**Old behavior**

Value generators were only called when the entity state changed to Added.

**New behavior**

Value generators are now called when the entity state is changed from Detached to Unchanged, Updated, or Deleted and the property contains the default values.

**Why**

This change was necessary to improve the experience with properties that are not persisted to the data store and have their value generated always on the client.

**Mitigations**

To prevent the value generator from being called, assign a non-default value to the property before the state is changed.

<a name="relational-model"></a>

### IMigrationsModelDiffer now uses IRelationalModel

[Tracking Issue #20305](https://github.com/aspnet/EntityFrameworkCore/issues/20305)

**Old behavior**

`IMigrationsModelDiffer` API was defined using `IModel`.

**New behavior**

`IMigrationsModelDiffer` API now uses `IRelationalModel`. However the model snapshot still contains only `IModel` as this code is part of the application and Entity Framework can't change it without making a bigger breaking change.

**Why**

`IRelationalModel` is a newly added representation of the database schema. Using it to find differences is faster and more accurate.

**Mitigations**

Use the following code to compare the model from `snapshot` with the model from `context`:

```cs
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

[Tracking Issue #21154](https://github.com/aspnet/EntityFrameworkCore/issues/21154)

**Old behavior**

It was possible to change the discriminator value before calling `SaveChanges`

**New behavior**

An exception will be throws in the above case.

**Why**

EF doesn't expect the entity type to change while it is still being tracked, so changing the discriminator value leaves the context in an inconsistent state, which might result in unexpected behavior.

**Mitigations**

If changing the discriminator value is necessary and the context will be disposed immediately after calling `SaveChanges`, the discriminator can be made mutable:

```cs
modelBuilder.Entity<BaseEntity>()
    .Property<string>("Discriminator")
    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
```

<a name="defining-query"></a>

### Defining query is replaced with provider-specific methods

[Tracking Issue #18903](https://github.com/dotnet/efcore/issues/18903)

**Old behavior**

Entity types were mapped to defining queries at the Core level. Anytime the entity type was used in the query root of the entity type was replaced by the defining query for any provider.

**New behavior**

APIs for defining query are deprecated. New provider-specific APIs were introduced.

**Why**

While defining queries were implemented as replacement query whenever query root is used in the query, it had a few issues:

- If defining query is projecting entity type using `new { ... }` in `Select` method, then identifying that as an entity required additional work and made it inconsistent with how EF Core treats nominal types in the query.
- For relational providers `FromSql` is still needed to pass the SQL string in LINQ expression form.

Initially defining queries were introduced as client-side views to be used with In-Memory provider for keyless entities (similar to database views in relational databases). Such definition makes it easy to test application against in-memory database. Afterwards they became broadly applicable, which was useful but brought inconsistent and hard to understand behavior. So we decided to simplify the concept. We made LINQ based defining query exclusive to In-Memory provider and treat them differently. For more information, [see this issue](https://github.com/dotnet/efcore/issues/20023).

**Mitigations**

For relational providers, use `ToSqlQuery` method in `OnModelCreating` and pass in a SQL string to use for the entity type.
For the In-Memory provider, use `ToInMemoryQuery` method in `OnModelCreating` and pass in a LINQ query to use for the entity type.

<a name="no-client-methods"></a>

### Provider-specific EF.Functions methods throw for InMemory provider

[Tracking Issue #20294](https://github.com/dotnet/efcore/issues/20294)

**Old behavior**

Provider-specific EF.Functions methods had a method body for client execution, which allowed them to be executed on the InMemory provider. For example, `EF.Functions.DateDiffDay` is a Provider-specific method.

**New behavior**

Provider-specific methods have been updated to throw an exception in their method body to block evaluating them on client side.

**Why**

Provider-specific methods map to a database function in the database. The computation done by the mapped database function can't always be replicated on the client side to execute it in LINQ. This causes an issue that results from the server may differ when executing the same method on client. Since these methods are used in LINQ to translate to specific database function, they don't need to be evaluated on client side. Because the InMemory provider is a different database, these methods aren't available for the InMemory provider. Trying to execute them for InMemory provider, or any other provider where these methods aren't defined, throws an exception.

**Mitigations**

Because there's no way to mimic the behavior of database functions, testing queries, which use them should be done against same kind of database.
