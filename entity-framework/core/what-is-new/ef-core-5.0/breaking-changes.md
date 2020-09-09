---
title: Breaking changes in EF Core 5.0 - EF Core
description: Complete list of breaking changes introduced in Entity Framework Core 5.0
author: bricelam
ms.date: 09/08/2020
uid: core/what-is-new/ef-core-5.0/breaking-changes
---

# Breaking changes in EF Core 5.0

The following API and behavior changes have the potential to break existing applications updating to EF Core 5.0.0.

## Summary

| **Breaking change**                                                                                                                   | **Impact** |
|:--------------------------------------------------------------------------------------------------------------------------------------|------------|
| [Required on the navigation from principal to dependent has different semantics](#required-dependent)                                 | Medium     |
| [Removed HasGeometricDimension method from SQLite NTS extension](#geometric-sqlite)                                                   | Low        |
| [Value generators are called when the entity state is changed from Detached to Unchanged, Updated or Deleted](#non-added-generation)  | Low        |
| [IMigrationsModelDiffer now uses IRelationalModel](#relational-model)                                                                 | Low        |
| [Discriminators are read-only](#read-only-discriminators)                                                                             | Low        |

<a name="geometric-sqlite"></a>
### Removed HasGeometricDimension method from SQLite NTS extension

[Tracking Issue #14257](https://github.com/aspnet/EntityFrameworkCore/issues/14257)

**Old behavior**

HasGeometricDimension was used to enable additional dimensions (Z and M) on geometry columns. However, it only ever affected database creation. It was unnecessary to specify it to query values with additional dimensions. It also didn't work correctly when inserting or updating values with additional dimensions ([see #14257](https://github.com/aspnet/EntityFrameworkCore/issues/14257)).

**New behavior**

To enable inserting and updating geometry values with additional dimensions (Z and M), the dimension needs to be specified as part of the column type name. This more closely matches the underlying behavior of SpatiaLite's AddGeometryColumn function.

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

With the added support for required dependents it is now possible to mark any reference navigation as required, meaning that in the case shown above the foreign key will be defined on the other side of the relationship and the properties won't be marked as required.

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

<a name="non-added-generation"></a>
### Value generators are called when the entity state is changed from Detached to Unchanged, Updated or Deleted

[Tracking Issue #15289](https://github.com/aspnet/EntityFrameworkCore/issues/15289)

**Old behavior**

Value generators were only called when the entity state changed to Added.

**New behavior**

Value generators are now called when the entity state is changed from Detached to Unchanged, Updated or Deleted and the property contains the default values.

**Why**

This change was necessary to improve the experience with properties that are not persisted to the data store and have their value generated always on the client.

**Mitigations**

To prevent the value generator from being called assign a non-default value to the property before the state is changed.

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

EF doesn't expect the entity type to change while it is still being tracked, so changing the discriminator value leaves the context in an inconsistent state which might result in unexpected behavior.

**Mitigations**

If changing the discriminator value is necessary and the context will be disposed immediately after calling `SaveChanges` the discriminator can be made mutable:

```cs
modelBuilder.Entity<BaseEntity>()
    .Property<string>("Discriminator")
    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
```
