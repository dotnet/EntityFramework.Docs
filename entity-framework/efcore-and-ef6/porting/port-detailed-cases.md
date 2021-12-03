---
title: Port from EF6 to EF Core - Detailed Cases
description: Guide to solutions, fixes, and workarounds for common issues that come up during the port from EF 6 to EF Core.
author: jeremylikness
ms.alias: jeliknes
ms.date: 10/25/2021
uid: efcore-and-ef6/porting/port-detailed-cases
---
# Detailed Cases for Porting from EF6 to EF Core

This document details some specific differences between EF6 and EF Core. Consult this guide when porting your code.

## No ObjectContext

EF Core uses a [DbContext](/ef/core/dbcontext-configuration) instead of an `ObjectContext`. You will have to update code that uses [IObjectContextAdapter](xref:System.Data.Entity.Infrastructure.IObjectContextAdapter). This was sometimes used for queries with `PreserveChanges` or `OverwriteChanges` merge option. For similar capabilities in EF Core, look into the [Reload](xref:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.Reload) method.

## Model Building: Discovery of Types

In EF Core, Entity Types are discovered by the engine in three ways:

- Expose a `DbSet<TEntity>` on your `DbContext` where `TEntity` is the type you wish to track.
- Reference a `Set<TEntity>` from somewhere in your code.
- Complex types referenced by discovered types are recursively discovered (for example, if your `Blog` references a `Post` and `Blog` is discoverable, `Post` will be discovered as well)

## Mapping

The `.Map()` extension in EF6 has been replaced with overloads and extension methods in EF Core. For example, to call a stored procedure you can use the `FromSqlRaw()` method on the `DbSet<>` instance.

## Required and Optional

In EF Core model-building, `IsRequired` only configures the what is required on the principal end. `HasForeignKey` now configures the principal end. To port your code, it will be more straightforward to use `.Navigation().IsRequired()` instead. For example:

**EF6:**

```csharp
modelBuilder.Entity<Instructor>()
    .HasRequired(t => t.OfficeAssignment)
    .WithRequiredPrincipal(t => t.Instructor);
```

**EF Core 6:**

```csharp
modelBuilder.Entity<Instructor>()
    .HasOne(t => t.OfficeAssignment)
    .WithOne(t => t.Instructor)
    .HasForeignKey<OfficeAssignment>();

modelBuilder.Entity<Instructor>()
    .Navigation(t => t.OfficeAssignment)
    .IsRequired();

modelBuilder.Entity<OfficeAssignment>()
    .Navigation(t => t.Instructor)
    .IsRequired();
```

By default everything is optional, so usually it's not necessary to call `.IsRequired(false)`.
