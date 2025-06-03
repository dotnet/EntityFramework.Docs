---
title: Breaking changes in EF Core 6.0 - EF Core
description: Complete list of breaking changes introduced in Entity Framework Core 6.0
author: SamMonoRT
ms.date: 09/21/2022
uid: core/what-is-new/ef-core-6.0/breaking-changes
---

# Breaking changes in EF Core 6.0

The following API and behavior changes have the potential to break existing applications updating to EF Core 6.0.

## Target Framework

EF Core 6.0 targets .NET 6. Applications targeting older .NET, .NET Core, and .NET Framework versions will need to target .NET 6 to use EF Core 6.0.

## Summary

| **Breaking change**                                                                                                                 | **Impact** |
|:------------------------------------------------------------------------------------------------------------------------------------|------------|
| [Nested optional dependents sharing a table and with no required properties cannot be saved](#nested-optionals)                     | High       |
| [Changing the owner of an owned entity now throws an exception](#owned-reparenting)                                                 | Medium     |
| [Azure Cosmos DB: Related entity types are discovered as owned](#cosmos-owned)                                                      | Medium     |
| [SQLite: Connections are pooled](#connection-pool)                                                                                  | Medium     |
| [Many-to-many relationships without mapped join entities are now scaffolded](#many-to-many)                                         | Medium     |
| [Cleaned up mapping between DeleteBehavior and ON DELETE values](#on-delete)                                                        | Low        |
| [In-memory database validates required properties do not contain nulls](#in-memory-required)                                        | Low        |
| [Removed last ORDER BY when joining for collections](#last-order-by)                                                                | Low        |
| [DbSet no longer implements IAsyncEnumerable](#dbset-iasyncenumerable)                                                              | Low        |
| [TVF return entity type is also mapped to a table by default](#tvf-table)                                                           | Low        |
| [Check constraint name uniqueness is now validated](#unique-check-constraints)                                                      | Low        |
| [Added IReadOnly Metadata interfaces and removed extension methods](#ireadonly-metadata)                                            | Low        |
| [IExecutionStrategy is now a singleton service](#iexecutionstrategy)                                                                | Low        |
| [SQL Server: More errors are considered transient](#transient-errors)                                                               | Low        |
| [Azure Cosmos DB: More characters are escaped in 'id' values](#cosmos-id)                                                           | Low        |
| [Some Singleton services are now Scoped](#query-services)                                                                           | Low*       |
| [New caching API for extensions that add or replace services](#extensions-caching)                                                  | Low*       |
| [New snapshot and design-time model initialization procedure](#snapshot-initialization)                                             | Low        |
| [`OwnedNavigationBuilder.HasIndex` returns a different type now](#owned-index)                                                      | Low        |
| [`DbFunctionBuilder.HasSchema(null)` overrides `[DbFunction(Schema = "schema")]`](#function-schema)                                 | Low        |
| [Pre-initialized navigations are overridden by values from database queries](#overwrite-navigations)                                | Low        |
| [Unknown enum string values in the database are not converted to the enum default when queried](#unknown-emums)                     | Low        |
| [DbFunctionBuilder.HasTranslation now provides the function arguments as IReadOnlyList rather than IReadOnlyCollection](#func-args) | Low        |
| [Default table mapping is not removed when the entity is mapped to a table-valued function](#tvf-default-mapping)                   | Low        |
| [dotnet-ef targets .NET 6](#dotnet-ef)                                                                                              | Low        |
| [`IModelCacheKeyFactory` implementations may need to be updated to handle design-time caching](#model-cache-key)                    | Low        |
| [`NavigationBaseIncludeIgnored` is now an error by default](#ignored-navigation)                                                    | Low        |

\* These changes are of particular interest to authors of database providers and extensions.

## High-impact changes

<a name="nested-optionals"></a>

### Nested optional dependents sharing a table and with no required properties are disallowed

[Tracking Issue #24558](https://github.com/dotnet/efcore/issues/24558)

#### Old behavior

Models with nested optional dependents sharing a table and with no required properties were allowed, but could result in data loss when querying for the data and then saving again. For example, consider the following model:

```csharp
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ContactInfo ContactInfo { get; set; }
}

[Owned]
public class ContactInfo
{
    public string Phone { get; set; }
    public Address Address { get; set; }
}

[Owned]
public class Address
{
    public string House { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string Postcode { get; set; }
}
```

None of the properties in `ContactInfo` or `Address` are required, and all these entity types are mapped to the same table. The rules for optional dependents (as opposed to required dependents) say that if all of the columns for `ContactInfo` are null, then no instance of `ContactInfo` will be created when querying for the owner `Customer`. However, this also means that no instance of `Address` will be created, even if the `Address` columns are non-null.

#### New behavior

Attempting to use this model will now throw the following exception:

> System.InvalidOperationException:
> Entity type 'ContactInfo' is an optional dependent using table sharing and containing other dependents without any required non shared property to identify whether the entity exists. If all nullable properties contain a null value in database then an object instance won't be created in the query causing nested dependent's values to be lost. Add a required property to create instances with null values for other properties or mark the incoming navigation as required to always create an instance.

This prevents data loss when querying and saving data.

#### Why

Using models with nested optional dependents sharing a table and with no required properties often resulted in silent data loss.

#### Mitigations

Avoid using optional dependents sharing a table and with no required properties. There are three easy ways to do this:

1. Make the dependents required. This means that the dependent entity will always have a value after it is queried, even if all its properties are null. For example:

   ```csharp
   public class Customer
   {
       public int Id { get; set; }
       public string Name { get; set; }
   
       [Required]
       public Address Address { get; set; }
   }
   ```

   Or:

   ```csharp
   modelBuilder.Entity<Customer>(
       b =>
           {
               b.OwnsOne(e => e.Address);
               b.Navigation(e => e.Address).IsRequired();
           });
   ```

2. Make sure that the dependent contains at least one required property.
3. Map optional dependents to their own table, instead of sharing a table with the principal. For example:

   ```csharp
   modelBuilder.Entity<Customer>(
       b =>
           {
               b.ToTable("Customers");
               b.OwnsOne(e => e.Address, b => b.ToTable("CustomerAddresses"));
           });
   ```

The problems with optional dependents and examples of these mitigations are included in the documentation for [What's new in EF Core 6.0](xref:core/what-is-new/ef-core-6.0/whatsnew#changes-to-owned-optional-dependent-handling).

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

### Azure Cosmos DB: Related entity types are discovered as owned

[Tracking Issue #24803](https://github.com/dotnet/efcore/issues/24803)
[What's new: Default to implicit ownership](xref:core/what-is-new/ef-core-6.0/whatsnew#default-to-implicit-ownership)

#### Old behavior

As in other providers, related entity types were discovered as normal (non-owned) types.

#### New behavior

Related entity types will now be owned by the entity type on which they were discovered. Only the entity types that correspond to a <xref:Microsoft.EntityFrameworkCore.DbSet`1> property will be discovered as non-owned.

#### Why

This behavior follows the common pattern of modeling data in Azure Cosmos DB of embedding related data into a single document. Azure Cosmos DB does not natively support joining different documents, so modeling related entities as non-owned has limited usefulness.

#### Mitigations

To configure an entity type to be non-owned call `modelBuilder.Entity<MyEntity>();`

<a name="connection-pool"></a>

### SQLite: Connections are pooled

[Tracking Issue #13837](https://github.com/dotnet/efcore/issues/13837)
[What's new: Default to implicit ownership](xref:core/what-is-new/ef-core-6.0/whatsnew#connection-pooling)

#### Old behavior

Previously, connections in Microsoft.Data.Sqlite were not pooled.

#### New behavior

Starting in 6.0, connections are now pooled by default. This results in database files being kept open by the process even after the ADO.NET connection object is closed.

#### Why

Pooling the underlying connections greatly improves the performance of opening and closing ADO.NET connection objects. This is especially noticeable for scenarios where opening the underlying connection is expensive as in the case of encryption, or in scenarios where there are a lot of short-lived connection to the database.

#### Mitigations

Connection pooling can be disabled by adding `Pooling=False` to a connection string.

Some scenarios (like deleting the database file) may now encounter errors stating that the file is still in use. You can manually clear the connection pool before performing operations of the file using `SqliteConnection.ClearPool()`.

```csharp
SqliteConnection.ClearPool(connection);
File.Delete(databaseFile);
```

<a name="many-to-many"></a>

### Many-to-many relationships without mapped join entities are now scaffolded

[Tracking Issue #22475](https://github.com/dotnet/efcore/issues/22475)

#### Old behavior

Scaffolding (reverse engineering) a `DbContext` and entity types from an existing database always explicitly mapped join tables to join entity types for many-to-many relationships.

#### New behavior

Simple join tables containing only two foreign key properties to other tables are no longer mapped to explicit entity types, but are instead mapped as a many-to-many relationship between the two joined tables.

#### Why

Many-to-many relationships without explicit join types were introduced in EF Core 5.0 and are a cleaner, more natural way to represent simple join tables.

#### Mitigations

There are two mitigations. The preferred approach is to update code to use the many-to-many relationships directly. It is very rare that the join entity type needs to be used directly when it contains only two foreign keys for the many-to-many relationships.

Alternately, the explicit join entity can be added back to the EF model. For example, assuming a many-to-many relationship between `Post` and `Tag`, add back the join type and navigations using partial classes:

```csharp
public partial class PostTag
{
    public int PostsId { get; set; }
    public int TagsId { get; set; }

    public virtual Post Posts { get; set; }
    public virtual Tag Tags { get; set; }
}

public partial class Post
{
    public virtual ICollection<PostTag> PostTags { get; set; }
}

public partial class Tag
{
    public virtual ICollection<PostTag> PostTags { get; set; }
}
```

Then add configuration for the join type and navigations to a partial class for the DbContext:

```csharp
public partial class DailyContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasMany(d => d.Tags)
                .WithMany(p => p.Posts)
                .UsingEntity<PostTag>(
                    l => l.HasOne<Tag>(e => e.Tags).WithMany(e => e.PostTags).HasForeignKey(e => e.TagsId),
                    r => r.HasOne<Post>(e => e.Posts).WithMany(e => e.PostTags).HasForeignKey(e => e.PostsId),
                    j =>
                    {
                        j.HasKey("PostsId", "TagsId");
                        j.ToTable("PostTag");
                    });
        });
    }
}
```

Finally, remove the generated configuration for the many-to-many relationship from the scaffolded context. This is needed because the scaffolded join entity type must be removed from the model before the explicit type can be used. This code will need to be removed each time the context is scaffolded, but because the code above is in partial classes it will persist.

Note that with this configuration, the join entity can be used explicitly, just like in previous versions of EF Core. However, the relationship can also be used as a many-to-many relationship. This means that updating the code like this can be a temporary solution while the rest of the code is updated to use the relationship as a many-to-many in the natural way.

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
Cascade        | CASCADE
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

<a name="in-memory-required"></a>

### In-memory database validates required properties do not contain nulls

[Tracking Issue #10613](https://github.com/dotnet/efcore/issues/10613)

#### Old behavior

The in-memory database allowed saving null values even when the property was configured as required.

#### New behavior

The in-memory database throws a `Microsoft.EntityFrameworkCore.DbUpdateException` when `SaveChanges` or `SaveChangesAsync` is called and a required property is set to null.

#### Why

The in-memory database behavior now matches the behavior of other databases.

#### Mitigations

The previous behavior (i.e. not checking null values) can be restored when configuring the in-memory provider. For example:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseInMemoryDatabase("MyDatabase", b => b.EnableNullChecks(false));
}
```

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

<xref:Microsoft.EntityFrameworkCore.DbSet`1>, which is used to execute queries on DbContext, used to implement <xref:System.Collections.Generic.IAsyncEnumerable`1>.

#### New behavior

<xref:Microsoft.EntityFrameworkCore.DbSet`1> no longer directly implements <xref:System.Collections.Generic.IAsyncEnumerable`1>.

#### Why

<xref:Microsoft.EntityFrameworkCore.DbSet`1> was originally made to implement <xref:System.Collections.Generic.IAsyncEnumerable`1> mainly in order to allow direct enumeration on it via the `foreach` construct. Unfortunately, when a project also references [System.Linq.Async](https://www.nuget.org/packages/System.Linq.Async) in order to compose async LINQ operators client-side, this resulted in an ambiguous invocation error between the operators defined over `IQueryable<T>` and those defined over `IAsyncEnumerable<T>`. C# 9 added [extension `GetEnumerator` support for `foreach` loops](/dotnet/csharp/language-reference/proposals/csharp-9.0/extension-getenumerator), removing the original main reason to reference `IAsyncEnumerable`.

The vast majority of `DbSet` usages will continue to work as-is, since they compose LINQ operators over `DbSet`, enumerate it, etc. The only usages broken are those which attempt to cast `DbSet` directly to `IAsyncEnumerable`.

#### Mitigations

If you need to refer to a <xref:Microsoft.EntityFrameworkCore.DbSet`1> as an <xref:System.Collections.Generic.IAsyncEnumerable`1>, call <xref:Microsoft.EntityFrameworkCore.DbSet`1.AsAsyncEnumerable*?displayProperty=nameWithType> to explicitly cast it.

<a name="tvf-table"></a>

### TVF return entity type is also mapped to a table by default

[Tracking Issue #23408](https://github.com/dotnet/efcore/issues/23408)

#### Old behavior

An entity type was not mapped to a table by default when used as a return type of a TVF configured with <xref:Microsoft.EntityFrameworkCore.RelationalModelBuilderExtensions.HasDbFunction*>.

#### New behavior

An entity type used as a return type of a TVF retains the default table mapping.

#### Why

It isn't intuitive that configuring a TVF removes the default table mapping for the return entity type.

#### Mitigations

To remove the default table mapping, call <xref:Microsoft.EntityFrameworkCore.RelationalEntityTypeBuilderExtensions.ToTable(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder,System.String)>:

```csharp
modelBuilder.Entity<MyEntity>().ToTable((string?)null));
```

<a name="unique-check-constraints"></a>

### Check constraint name uniqueness is now validated

[Tracking Issue #25061](https://github.com/dotnet/efcore/issues/25061)

#### Old behavior

Check constraints with the same name were allowed to be declared and used on the same table.

#### New behavior

Explicitly configuring two check constraints with the same name on the same table will now result in an exception. Check constraints created by a convention will be assigned a unique name.

#### Why

Most databases don't allow two check constraints with the same name to be created on the same table, and some require them to be unique even across tables. This would result in exception being thrown when applying a migration.

#### Mitigations

In some cases, valid check constraint names might be different due to this change. To specify the desired name explicitly, call <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.CheckConstraintBuilder.HasName*>:

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

<a name="iexecutionstrategy"></a>

### IExecutionStrategy is now a singleton service

[Tracking Issue #21350](https://github.com/dotnet/efcore/issues/21350)

#### New behavior

<xref:Microsoft.EntityFrameworkCore.Storage.IExecutionStrategy> is now a singleton service. This means that any added state in custom implementations will remain between executions and the delegate passed to <xref:Microsoft.EntityFrameworkCore.Infrastructure.RelationalDbContextOptionsBuilder`2.ExecutionStrategy*> will only be executed once.

#### Why

This reduced allocations on two hot paths in EF.

#### Mitigations

Implementations deriving from <xref:Microsoft.EntityFrameworkCore.Storage.ExecutionStrategy> should clear any state in <xref:Microsoft.EntityFrameworkCore.Storage.ExecutionStrategy.OnFirstExecution>.

Conditional logic in the delegate passed to <xref:Microsoft.EntityFrameworkCore.Infrastructure.RelationalDbContextOptionsBuilder`2.ExecutionStrategy*> should be moved to a custom implementation of <xref:Microsoft.EntityFrameworkCore.Storage.IExecutionStrategy>.

<a name="transient-errors"></a>

### SQL Server: More errors are considered transient

[Tracking Issue #25050](https://github.com/dotnet/efcore/issues/25050)

#### New behavior

The errors listed in the issue above are now considered transient. When using the default (non-retrying) execution strategy, these errors will now be wrapped in an addition exception instance.

#### Why

We continue to gather feedback from both users and SQL Server team on which errors should be considered transient.

#### Mitigations

To change the set of errors that are considered transient, use a custom execution strategy that could be derived from <xref:Microsoft.EntityFrameworkCore.SqlServerRetryingExecutionStrategy> - <xref:core/miscellaneous/connection-resiliency>.

<a name="cosmos-id"></a>

### Azure Cosmos DB: More characters are escaped in 'id' values

[Tracking Issue #25100](https://github.com/dotnet/efcore/issues/25100)

#### Old behavior

In EF Core 5, only `'|'` was escaped in `id` values.

#### New behavior

In EF Core 6, `'/'`, `'\'`, `'?'` and `'#'` are also escaped in `id` values.

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

The lifetime had to be changed to allow a new feature - <xref:Microsoft.EntityFrameworkCore.ModelConfigurationBuilder.DefaultTypeMapping*> - to affect queries.

The design-time services lifetimes have been adjusted to match the run-time services lifetimes to avoid errors when using both.

#### Mitigations

Use <xref:Microsoft.EntityFrameworkCore.Infrastructure.EntityFrameworkServicesBuilder.TryAdd*> to register EF Core services using the default lifetime. Only use <xref:Microsoft.EntityFrameworkCore.Infrastructure.EntityFrameworkServicesBuilder.TryAddProviderSpecificServices*> for services that are not added by EF.

<a name="extensions-caching"></a>

### New caching API for extensions that add or replace services

[Tracking Issue #19152](https://github.com/dotnet/efcore/issues/19152)

#### Old behavior

In EF Core 5, <xref:Microsoft.EntityFrameworkCore.Infrastructure.DbContextOptionsExtensionInfo.GetServiceProviderHashCode*> returned `long` and was used directly as part of the cache key for the service provider.

#### New behavior

<xref:Microsoft.EntityFrameworkCore.Infrastructure.DbContextOptionsExtensionInfo.GetServiceProviderHashCode*> now returns `int` and is only used to calculate the hash code of the cache key for the service provider.

Also, <xref:Microsoft.EntityFrameworkCore.Infrastructure.DbContextOptionsExtensionInfo.ShouldUseSameServiceProvider*> needs to be implemented to indicate whether the current object represents the same service configuration and thus can use the same service provider.

#### Why

Just using a hash code as part of the cache key resulted in occasional collisions that were hard to diagnose and fix. The additional method ensures that the same service provider is used only when appropriate.

#### Mitigations

Many extensions don't expose any options that affect registered services and can use the following implementation of <xref:Microsoft.EntityFrameworkCore.Infrastructure.DbContextOptionsExtensionInfo.ShouldUseSameServiceProvider*>:

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

### New snapshot and design-time model initialization procedure

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

This snippet shows how to implement <xref:Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory`1> by creating a model externally and calling <xref:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel*>:

```csharp
internal class MyDesignContext : IDesignTimeDbContextFactory<MyContext>
{
    public TestContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DB"));

        var modelBuilder = SqlServerConventionSetBuilder.CreateModelBuilder();
        CustomizeModel(modelBuilder);
        var model = modelBuilder.Model.FinalizeModel();

        var serviceContext = new MyContext(optionsBuilder.Options);
        model = serviceContext.GetService<IModelRuntimeInitializer>().Initialize(model);
        return new MyContext(optionsBuilder.Options);
    }
}
```

<a name="owned-index"></a>

### `OwnedNavigationBuilder.HasIndex` returns a different type now

[Tracking Issue #24005](https://github.com/dotnet/efcore/issues/24005)

#### Old behavior

In EF Core 5, <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.OwnedNavigationBuilder.HasIndex*> returned `IndexBuilder<TEntity>` where `TEntity` is the owner type.

#### New behavior

<xref:Microsoft.EntityFrameworkCore.Metadata.Builders.OwnedNavigationBuilder.HasIndex*> now returns `IndexBuilder<TDependentEntity>`, where `TDependentEntity` is the owned type.

#### Why

The returned builder object wasn't typed correctly.

#### Mitigations

Recompiling your assembly against the latest version of EF Core will be enough to fix any issues caused by this change.

<a name="function-schema"></a>

### `DbFunctionBuilder.HasSchema(null)` overrides `[DbFunction(Schema = "schema")]`

[Tracking Issue #24228](https://github.com/dotnet/efcore/issues/24228)

#### Old behavior

In EF Core 5, calling <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.DbFunctionBuilder.HasSchema*> with `null` value didn't store the configuration source, thus <xref:Microsoft.EntityFrameworkCore.DbFunctionAttribute> was able to override it.

#### New behavior

Calling <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.DbFunctionBuilder.HasSchema*> with `null` value now stores the configuration source and prevents the attribute from overriding it.

#### Why

Configuration specified with the <xref:Microsoft.EntityFrameworkCore.ModelBuilder> API should not be overridable by data annotations.

#### Mitigations

Remove the `HasSchema` call to let the attribute configure the schema.

<a name="overwrite-navigations"></a>

### Pre-initialized navigations are overridden by values from database queries

[Tracking Issue #23851](https://github.com/dotnet/efcore/issues/23851)

#### Old behavior

Navigation properties set to an empty object were left unchanged for tracking queries, but were overwritten for non-tracking queries. For example, consider the following entity types:

```csharp
public class Foo
{
    public int Id { get; set; }

    public Bar Bar { get; set; } = new(); // Don't do this.
}

public class Bar
{
    public int Id { get; set; }
}
```

A no-tracking query for `Foo` including `Bar` set `Foo.Bar` to the entity queried from the database. For example, this code:

```csharp
var foo = await context.Foos.AsNoTracking().Include(e => e.Bar).SingleAsync();
Console.WriteLine($"Foo.Bar.Id = {foo.Bar.Id}");
```

Printed `Foo.Bar.Id = 1`.

However, the same query run for tracking didn't overwrite `Foo.Bar` with the entity queried from the database. For example, this code:

```csharp
var foo = await context.Foos.Include(e => e.Bar).SingleAsync();
Console.WriteLine($"Foo.Bar.Id = {foo.Bar.Id}");
```

Printed `Foo.Bar.Id = 0`.

#### New behavior

In EF Core 6.0, the behavior of tracking queries now matches that of no-tracking queries. This means that both this code:

```csharp
var foo = await context.Foos.AsNoTracking().Include(e => e.Bar).SingleAsync();
Console.WriteLine($"Foo.Bar.Id = {foo.Bar.Id}");
```

And this code:

```csharp
var foo = await context.Foos.Include(e => e.Bar).SingleAsync();
Console.WriteLine($"Foo.Bar.Id = {foo.Bar.Id}");
```

Print `Foo.Bar.Id = 1`.

#### Why

There are two reasons for making this change:

1. To ensure that tracking and no-tracking queries have consistent behavior.
2. When a database is queried it is reasonable to assume that the application code wants to get back the values that are stored in the database.

#### Mitigations

There are two mitigations:

1. Do not query for objects from the database that should not be included in the results. For example, in the code snippets above, do not `Include` `Foo.Bar` if the `Bar` instance should not be returned from the database and included in the results.
2. Set the value of the navigation after querying from the database. For example, in the code snippets above, call `foo.Bar = new()` after running the query.

Also, consider not initializing related entity instances to default objects. This implies that the related instance is a new entity, not saved to the database, with no key value set. If instead the related entity does exist in the database, then the data in code is fundamentally at odds with the data stored in the database.

<a name="unknown-emums"></a>

### Unknown enum string values in the database are not converted to the enum default when queried

[Tracking Issue #24084](https://github.com/dotnet/efcore/issues/24084)

#### Old behavior

Enum properties can be mapped to string columns in the database using `HasConversion<string>()` or `EnumToStringConverter`. This results in EF Core converting string values in the column to matching members of the .NET enum type. However, if the string value did not match and enum member, then the property was set to the default value for the enum.

#### New behavior

EF Core 6.0 now throws an `InvalidOperationException` with the message "Cannot convert string value '`{value}`' from the database to any value in the mapped '`{enumType}`' enum."

#### Why

Converting to the default value can result in database corruption if the entity is later saved back to the database.

#### Mitigations

Ideally, ensure that the database column only contains valid values. Alternately, implement a `ValueConverter` with the old behavior.

<a name="func-args"></a>

### DbFunctionBuilder.HasTranslation now provides the function arguments as IReadOnlyList rather than IReadOnlyCollection

[Tracking Issue #23565](https://github.com/dotnet/efcore/issues/23565)

#### Old behavior

When configuring translation for a user-defined function using `HasTranslation` method, the arguments to the function were provided as `IReadOnlyCollection<SqlExpression>`.

#### New behavior

In EF Core 6.0, the arguments are now provided as `IReadOnlyList<SqlExpression>`.

#### Why

`IReadOnlyList` allows to use indexers, so the arguments are now easier to access.

#### Mitigations

None. `IReadOnlyList` implements `IReadOnlyCollection` interface, so the transition should be straightforward.

<a name="tvf-default-mapping"></a>

### Default table mapping is not removed when the entity is mapped to a table-valued function

[Tracking Issue #23408](https://github.com/dotnet/efcore/issues/23408)

#### Old behavior

When an entity was mapped to a table-valued function, its default mapping to a table was removed.

#### New behavior

In EF Core 6.0, the entity is still mapped to a table using default mapping, even if it's also mapped to table-valued function.

#### Why

Table-valued functions which return entities are often used either as a helper or to encapsulate an operation returning a collection of entities, rather than as a strict replacement of the entire table. This change aims to be more in line with the likely user intention.

#### Mitigations

Mapping to a table can be explicitly disabled in the model configuration:

```csharp
modelBuilder.Entity<MyEntity>().ToTable((string)null);
```

<a name="dotnet-ef"></a>

### dotnet-ef targets .NET 6

[Tracking Issue #27787](https://github.com/dotnet/efcore/issues/27787)

#### Old behavior

The dotnet-ef command has targeted .NET Core 3.1 for a while now. This allowed you to use newer version of the tool without installing newer versions of the .NET runtime.

#### New behavior

In EF Core 6.0.6, the dotnet-ef tool now targets .NET 6. You can still use the tool on projects targeting older versions of .NET and .NET Core, but you'll need to install the .NET 6 runtime in order to run the tool.

#### Why

The .NET 6.0.200 SDK updated the behavior of `dotnet tool install` on osx-arm64 to create an osx-x64 shim for tools targeting .NET Core 3.1. In order to maintain a working default experience for dotnet-ef, we had to update it to target .NET 6.

#### Mitigations

To run dotnet-ef without installing the .NET 6 runtime, you can install an older version of the tool:

```dotnetcli
dotnet tool install dotnet-ef --version 3.1.*
```

<a name="model-cache-key"></a>

### `IModelCacheKeyFactory` implementations may need to be updated to handle design-time caching

[Tracking Issue #25154](https://github.com/dotnet/efcore/issues/25154)

#### Old behavior

`IModelCacheKeyFactory` did not have an option to cache the design-time model separately from the runtime model.

#### New behavior

`IModelCacheKeyFactory` has a new overload that allows the design-time model to be cached separately from the runtime model. Not implementing this method may result in an exception similar to:

> System.InvalidOperationException: 'The requested configuration is not stored in the read-optimized model, please use 'DbContext.GetService&lt;IDesignTimeModel&gt;().Model'.'

#### Why

Implementation of compiled models required separation of the design-time (used when building the model) and runtime (used when executing queries, etc.) models. If the runtime code needs access to design-time information, then the design-time model must be cached.

#### Mitigations

Implement the new overload. For example:

```csharp
public object Create(DbContext context, bool designTime)
    => context is DynamicContext dynamicContext
        ? (context.GetType(), dynamicContext.UseIntProperty, designTime)
        : (object)context.GetType();
```

The navigation '{navigation}' was ignored from 'Include' in the query since the fix-up will automatically populate it. If any further navigations are specified in 'Include' afterwards then they will be ignored. Walking back in the include tree is not allowed.

<a name="ignored-navigation"></a>

### `NavigationBaseIncludeIgnored` is now an error by default

[Tracking Issue #4315](https://github.com/dotnet/EntityFramework.Docs/issues/4315)

#### Old behavior

The event `CoreEventId.NavigationBaseIncludeIgnored` was logged as a warning by default.

#### New behavior

The event `CoreEventId.NavigationBaseIncludeIgnored` was logged as an error by default and causes an exception to be thrown.

#### Why

These query patterns are not allowed, so EF Core now throws to indicate that the queries should be updated.

#### Mitigations

The old behavior can be restored by configuring the event as a warning. For example:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.ConfigureWarnings(b => b.Warn(CoreEventId.NavigationBaseIncludeIgnored));
```
