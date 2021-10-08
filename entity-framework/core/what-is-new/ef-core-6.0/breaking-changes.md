---
title: Breaking changes in EF Core 6.0 - EF Core
description: Complete list of breaking changes introduced in Entity Framework Core 6.0
author: ajcvickers
ms.date: 10/1/2021
uid: core/what-is-new/ef-core-6.0/breaking-changes
---

# Breaking changes in EF Core 6.0

The following API and behavior changes have the potential to break existing applications updating to EF Core 6.0.0.

## Summary

| **Breaking change**                                                                                                                   | **Impact** |
|:--------------------------------------------------------------------------------------------------------------------------------------|------------|
| [Changing the owner of an owned entity now throws an exception](#owned-reparenting)                                                   | Medium     |
| [Cosmos: Related entity types are discovered as owned](#cosmos-owned)                                                                 | Medium     |
| [Cleaned up mapping between DeleteBehavior and ON DELETE values](#on-delete)                                                          | Low        |
| [Removed last ORDER BY when joining for collections](#last-order-by)                                                                  | Low        |
| [DbSet no longer implements IAsyncEnumerable](#dbset-iasyncenumerable)                                                                | Low        |
| [TVF return entity type is also mapped to a table by default](#tvf-table)                                                             | Low        |
| [Check constraint name uniqueness is now validated](#unique-check-constraints)                                                        | Low        |
| [Added IReadOnly Metadata interfaces and removed extension methods](#ireadonly-metadata)                                              | Low        |
| [SQL Server: More errors are considered transient](#transient-errors)                                                                 | Low        |
| [Cosmos: More characters are escaped in 'id' values](#cosmos-id)                                                                      | Low        |
| [Some Singleton services are now Scoped](#query-services)                                                                             | Low        |
| [New caching API for extensions that add or replace services](#extensions-caching)                                                    | Low        |
| [New snapshot model initialization procedure](#snapshot-initialization)                                                               | Low        |
| [`OwnedNavigationBuilder.HasIndex` returns a different type now](#owned-index)                                                        | Low        |

## Medium-impact changes

<a name="owned-reparenting"></a>

### Changing the owner of an owned entity now throws an exception

[Tracking Issue #4073](https://github.com/dotnet/efcore/issues/4073)

#### Old behavior

It was possible to reassign an owned entity to a different owner entity.

#### New behavior

This action will now throw an exception:

> The property '{entityType}.{property}' is part of a key and so cannot be modified or marked as modified. To change the principal of an existing entity with an identifying foreign key, first delete the dependent and invoke 'SaveChanges', and then associate the dependent with the new principal.

#### Why

Even though we don't require key properties to exist on an owned type, EF will still create shadow properties to be used as the primary key and the foreign key pointing to the owner. When the owner entity is changed it causes the values of the foreign key on the owned entity to change, and since they are also used as the primary key this results in the entity identity to change. This isn't yet fully supported in EF Core and was only conditionally allowed for owned entities, sometimes resulting in the internal state becoming inconsistent.

#### Mitigations

Instead of assigning the same owned instance to a new owner you can assign a copy and delete the old one.

<a name="cosmos-owned"></a>

### Cosmos: Related entity types are discovered as owned

[Tracking Issue #24803](https://github.com/dotnet/efcore/issues/24803)
[What's new: Default to implicit ownership](/core/what-is-new/ef-core-6.0/whatsnew#default-to-implicit-ownership)

#### Old behavior

As in other providers, related entity types were discovered as normal (non-owned) types.

#### New behavior

Now related entity types will be owned by the entity type on which they were discovered. Only the entity types that correspond to a <xref:Microsoft.EntityFrameworkCore.DbSet%601> property will be discovered as non-owned.

#### Why

This behavior follows the common pattern of modeling data in Azure Cosmos DB of embedding related data into a single document.

#### Mitigations

To configure an entity type to be non-owned call `modelBuilder.Entity<MyEntity>();`

## Low-impact changes

<a name="on-delete"></a>

### Cleaned up mapping between DeleteBehavior and ON DELETE values

[Tracking Issue #21252](https://github.com/dotnet/efcore/issues/21252)

#### Old behavior

Some of the mappings between a relationship's `OnDelete()` behavior and the foreign keys' `ON DELETE` behavior in the database were inconsistent in both Migrations and Scaffolding.

#### New behavior

The following table illustrates the changes for **Migrations**.

OnDelete()     | ON DELETE
-------------- | ---------
NoAction       | NO ACTION
ClientNoAction | NO ACTION
Restrict       | RESTRICT
Cascasde       | CASCADE
ClientCascade  | ~~RESTRICT~~ **NO ACTION**
SetNull        | SET NULL
ClientSetNull  | ~~RESTRICT~~ **NO ACTION**

The changes for **Scaffolding** are as follows.

ON DELETE | OnDelete()
--------- | ----------
NO ACTION | ClientSetNull
RESTRICT  | ~~ClientSetNull~~ **Restrict**
CASCADE   | Cascade
SET NULL  | SetNull

#### Why

The new mappings are more consistent. The default database behavior of NO ACTION is now preferred over the more restrictive and less performant RESTRICT behavior.

#### Mitigations

The default OnDelete() behavior of optional relationships is ClientSetNull. Its mapping has changed from RESTRICT to NO ACTION. This may cause a lot of operations to be generated in your first migration added after upgrading to EF Core 6.0.

You can choose to either apply these operations or manually remove them from the migration since they have no functional impact on EF Core.

SQL Server doesn't support RESTRICT, so these foreign keys were already created using NO ACTION. The migration operations will have no affect on SQL Server and are safe to remove.

<a name="last-order-by"></a>

### Removed last ORDER BY when joining for collections

[Tracking Issue #19828](https://github.com/dotnet/efcore/issues/19828)

#### Old behavior

When performing SQL JOINs on collections (one-to-many relationships), EF Core used to add an ORDER BY for each key column of the joined table. For example, loading all Blogs with their related Posts was done via the following SQL:

```sql
SELECT [b].[BlogId], [b].[Name], [p].[PostId], [p].[BlogId], [p].[Title]
FROM [Blogs] AS [b]
LEFT JOIN [Post] AS [p] ON [b].[BlogId] = [p].[BlogId]
ORDER BY [b].[BlogId], [p].[PostId]
```

These orderings are necessary for proper materialization of the entities.

#### New behavior

The very last ORDER BY for a collection join is now omitted:

```sql
SELECT [b].[BlogId], [b].[Name], [p].[PostId], [p].[BlogId], [p].[Title]
FROM [Blogs] AS [b]
LEFT JOIN [Post] AS [p] ON [b].[BlogId] = [p].[BlogId]
ORDER BY [b].[BlogId]
```

An ORDER BY for the Post's ID column is no longer generated.

#### Why

Every ORDER BY imposes additional work at the database side, and the last ordering isn't necessary for EF Core's materialization needs. Data shows that removing this last ordering can produce a significant performance improvement in some scenarios.

#### Mitigations

If your application expects joined entities to be returned in a particular order, make that explicit by adding a LINQ `OrderBy` operator to your query.

<a name="dbset-iasyncenumerable"></a>

### DbSet no longer implements IAsyncEnumerable

[Tracking Issue #24041](https://github.com/dotnet/efcore/issues/24041)

#### Old behavior

<xref:Microsoft.EntityFrameworkCore.DbSet%601>, which is used to execute queries on DbContext, used to implement <xref:System.Collections.Generic.IAsyncEnumerable%601>.

#### New behavior

<xref:Microsoft.EntityFrameworkCore.DbSet%601> no longer directly implements <xref:System.Collections.Generic.IAsyncEnumerable%601>.

#### Why

<xref:Microsoft.EntityFrameworkCore.DbSet%601> was originally made to implement <xref:System.Collections.Generic.IAsyncEnumerable%601> mainly in order to allow direct enumeration on it via the `foreach` construct. Unfortunately, when a project also references [System.Linq.Async](https://www.nuget.org/packages/System.Linq.Async) in order to compose async LINQ operators client-side, this resulted in an ambiguous invocation error between the operators defined over `IQueryable<T>` and those defined over `IAsyncEnumerable<T>`. C# 9 added [extension `GetEnumerator` support for `foreach` loops](/dotnet/csharp/language-reference/proposals/csharp-9.0/extension-getenumerator), removing the original main reason to reference `IAsyncEnumerable`.

The vast majority of `DbSet` usages will continue to work as-is, since they compose LINQ operators over `DbSet`, enumerate it, etc. The only usages broken are those which attempt to cast `DbSet` directly to `IAsyncEnumerable`.

#### Mitigations

If you need to refer to a <xref:Microsoft.EntityFrameworkCore.DbSet%601> as an <xref:System.Collections.Generic.IAsyncEnumerable%601>, call <xref:Microsoft.EntityFrameworkCore.DbSet%601.AsAsyncEnumerable%2A?displayProperty=nameWithType> to explicitly cast it.

<a name="tvf-table"></a>

### TVF return entity type is also mapped to a table by default

[Tracking Issue #23408](https://github.com/dotnet/efcore/issues/23408)

#### Old behavior

Entity type was not mapped to a table by default when used as a return type of a TVF configured with <xref:Microsoft.EntityFrameworkCore.RelationalModelBuilderExtensions.HasDbFunction%2A>.

#### New behavior

Entity types used as a return type of a TVF retains the default table mapping.

#### Why

It isn't intuitive that configuring a TVF removes the default table mapping for the return entity type.

#### Mitigations

To remove the default table mapping call <xref:Microsoft.EntityFrameworkCore.RelationalEntityTypeBuilderExtensions.ToTable(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder,System.String)>:

```csharp
modelBuilder.Entity<MyEntity>().ToTable((string?)null));
```

<a name="iunique-check-constraints"></a>

### Check constraint name uniqueness is now validated

[Tracking Issue #25061](https://github.com/dotnet/efcore/issues/25061)

#### Old behavior

Check constraints with the same name were allowed to be declared and used on the same table.

#### New behavior

Explicitly configuring two check constraints with the same name on the same table will now result in an exception. Check constraints created by a convention will be assigned a unique name.

#### Why

Most databases don't allow two check constraints with the same name to be created on the same table, and some require them to be unique even across tables. This would result in exception being thrown when applying a migration.

#### Mitigations

In some cases, valid check constraint names might be different due to this change. To specify the desired name explicitly, call <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.CheckConstraintBuilder.HasName%2A>:

```csharp
modelBuilder.Entity<MyEntity>().HasCheckConstraint("CK_Id", "Id > 0", c => c.HasName("CK_MyEntity_Id"));
```

<a name="ireadonly-metadata"></a>

### Added IReadOnly Metadata interfaces and removed extension methods

[Tracking Issue #19213](https://github.com/dotnet/efcore/issues/19213)

#### Old behavior

There were three sets of metadata interfaces: <xref:Microsoft.EntityFrameworkCore.Metadata.IModel>, <xref:Microsoft.EntityFrameworkCore.Metadata.IMutableModel> and <xref:Microsoft.EntityFrameworkCore.Metadata.IConventionModel> as well as extension methods.

#### New behavior

A new set of `IReadOnly` interfaces has been added, e.g. <xref:Microsoft.EntityFrameworkCore.Metadata.IReadOnlyModel>. Extension methods that were previously defined for the metadata interfaces have been converted to default interface methods.

#### Why

Default interface methods allow the implementation to be overridden, this is leveraged by the new run-time model implementation to offer better performance.

#### Mitigations

These changes shouldn't affect most code. However, if you were using the extension methods via the static invocation syntax, it would need to be converted to instance invocation syntax.

<a name="transient-errors"></a>

### SQL Server: More errors are considered transient

[Tracking Issue #25050](https://github.com/dotnet/efcore/issues/25050)

#### New behavior

The errors listed in the issue above are now considered transient. When using the default (non-retrying) execution strategy these errors will now be wrapped in an addition exception instance.

#### Why

We continue to gather feedback from both users and SQL Server team on which errors should be considered transient.

#### Mitigations

To change the set of errors that are considered transient use a custom execution strategy that could be derived from <xref:Microsoft.EntityFrameworkCore.SqlServerRetryingExecutionStrategy> - <xref:core/miscellaneous/connection-resiliency>.

<a name="cosmos-id"></a>

### Cosmos: More characters are escaped in 'id' values

[Tracking Issue #25100](https://github.com/dotnet/efcore/issues/25100)

#### Old behavior

In EF Core 5, only `'|'` was escaped in `id` values.

#### New behavior

Now only `'/'`, `'\'`, `'?'` and `'#'` are also escaped in `id` values.

#### Why

These characters are invalid, as documented in [Resource.Id](/dotnet/api/microsoft.azure.documents.resource.id). Using them in `id` will cause queries to fail.

#### Mitigations

You can override the generated value by setting it before the entity is marked as `Added`:

```csharp
var entry = context.Attach(entity);
entry.Property("__id").CurrentValue = "MyEntity|/\\?#";
entry.State = EntityState.Added;
```

<a name="query-services"></a>

### Some Singleton services are now Scoped

[Tracking Issue #25084](https://github.com/dotnet/efcore/issues/25084)

#### New behavior

Many query services and some design-time services that were registered as `Singleton` are now registered as `Scoped`.

#### Why

The lifetime had to be changed to allow a new feature - <xref:Microsoft.EntityFrameworkCore.ModelConfigurationBuilder.DefaultTypeMapping> - to affect queries.

The design-time services lifetimes have been adjusted to match the run-time services lifetimes to avoid errors when using both.

#### Mitigations

Use <xref:Microsoft.EntityFrameworkCore.Infrastructure.EntityFrameworkServicesBuilder.TryAdd%2A> to register EF Core services using the default lifetime. Only use <xref:Microsoft.EntityFrameworkCore.Infrastructure.EntityFrameworkServicesBuilder.TryAddProviderSpecificServices%2A> for services that are not added by EF.

<a name="extensions-caching"></a>

### New caching API for extensions that add or replace services

[Tracking Issue #19152](https://github.com/dotnet/efcore/issues/19152)

#### Old behavior

In EF Core 5, <xref:Microsoft.EntityFrameworkCore.Infrastructure.DbContextOptionsExtensionInfo.GetServiceProviderHashCode%2A> returned `long` and was used directly as part of the cache key for the service provider.

#### New behavior

<xref:Microsoft.EntityFrameworkCore.Infrastructure.DbContextOptionsExtensionInfo.GetServiceProviderHashCode%2A> now returns `int` and is only used to calculate the hash code of the cache key for the service provider.

Also, <xref:Microsoft.EntityFrameworkCore.Infrastructure.DbContextOptionsExtensionInfo.ShouldUseSameServiceProvider%2A> needs to be implemented to indicate whether the current object represents the same service configuration and thus can use the same service provider.

#### Why

Just using a hash code as part of the cache key resulted in occasional collisions that were hard to diagnose and fix. The additional method ensures that the same service provider is used only when appropriate.

#### Mitigations

Many extensions don't expose any options that affect registered services and can use the following implementation of <xref:Microsoft.EntityFrameworkCore.Infrastructure.DbContextOptionsExtensionInfo.ShouldUseSameServiceProvider%2A>:

```csharp
private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
{
    public ExtensionInfo(IDbContextOptionsExtension extension)
        : base(extension)
    {
    }

    ...

    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
        => other is ExtensionInfo;
}
```

Otherwise, additional predicates should be added to compare all relevant options.

<a name="snapshot-initialization"></a>

### New snapshot model initialization procedure

[Tracking Issue #22031](https://github.com/dotnet/efcore/issues/22031)

#### Old behavior

In EF Core 5, specific conventions needed to be invoked before the snapshot model was ready to be used.

#### New behavior

<xref:Microsoft.EntityFrameworkCore.Infrastructure.IModelRuntimeInitializer> was introduced to hide some of the required steps, and a run-time model was introduced that doesn't have all the migrations metadata, so the design-time model should be used for model diffing.

#### Why

<xref:Microsoft.EntityFrameworkCore.Infrastructure.IModelRuntimeInitializer> abstracts away the model finalization steps, so these can now be changed without further breaking changes for the users.

The optimized run-time model was introduced to improve run-time performance. It has several optimizations, one of which is removing metadata that is not used at run-time.

#### Mitigations

The following snippet illustrates how to check whether the current model is different from the snapshot model:

```csharp
var snapshotModel = migrationsAssembly.ModelSnapshot?.Model;

if (snapshotModel is IMutableModel mutableModel)
{
    snapshotModel = mutableModel.FinalizeModel();
}

if (snapshotModel != null)
{
    snapshotModel = context.GetService<IModelRuntimeInitializer>().Initialize(snapshotModel);
}

var hasDifferences = context.GetService<IMigrationsModelDiffer>().HasDifferences(
    snapshotModel?.GetRelationalModel(),
    context.GetService<IDesignTimeModel>().Model.GetRelationalModel());
```

<a name="owned-index"></a>

### `OwnedNavigationBuilder.HasIndex` returns a different type now

[Tracking Issue #24005](https://github.com/dotnet/efcore/issues/24005)

#### Old behavior

In EF Core 5, <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.OwnedNavigationBuilder.HasIndex%2A> returned `IndexBuilder<TEntity>` where `TEntity` is the owner type.

#### New behavior

<xref:Microsoft.EntityFrameworkCore.Metadata.Builders.OwnedNavigationBuilder.HasIndex%2A> now returns `IndexBuilder<TDependentEntity>`, where `TDependentEntity` is the owned type.

#### Why

The returned builder object wasn't typed correctly.

#### Mitigations

Recompiling your assembly against the latest version of EF Core will be enough to fix any issues caused by this change.
