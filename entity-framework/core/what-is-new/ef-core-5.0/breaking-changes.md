---
title: Breaking changes in EF Core 5.0 - EF Core
description: Complete list of breaking changes introduced in Entity Framework Core 5.0
author: bricelam
ms.date: 06/05/2020
uid: core/what-is-new/ef-core-5.0/breaking-changes
---

# Breaking changes in EF Core 5.0

The following API and behavior changes have the potential to break existing applications updating to EF Core 5.0.0.

## Summary

| **Breaking change**                                                                                                               | **Impact** |
|:----------------------------------------------------------------------------------------------------------------------------------|------------|
| [Removed HasGeometricDimension method from SQLite NTS extension](#removed-hasgeometricdimension-method-from-sqlite-nts-extension) | Low        |

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
