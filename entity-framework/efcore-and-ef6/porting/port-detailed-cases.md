---
title: Port from EF6 to EF Core - Detailed Cases
description: Guide to solutions, fixes, and workarounds for common issues that come up during the port from EF 6 to EF Core.
author: jeremylikness
ms.alias: jeliknes
ms.date: 12/09/2021
uid: efcore-and-ef6/porting/port-detailed-cases
---
# Detailed Cases for Porting from EF6 to EF Core

This document details some specific differences between EF6 and EF Core. Consult this guide when porting your code.

## Configuring the database connection

There are several differences between how EF6 connects to various data sources compared to EF Core. They are important to understand when you port your code.

- **Connection strings**: EF Core does not directly support multiple constructor overloads for different connection strings as EF6 does. Instead, it relies on [DbContextOptions](xref:core/dbcontext-configuration/index#dbcontextoptions). You can still provide multiple constructor overloads in derived types, but will need to map connections through the options.
- **Configuration and cache**: EF Core supports a more robust and flexible implementation of dependency injection with an internal infrastructure that can connect to external service providers. This can be managed by the application to handle situations when the caches must be flushed. The EF6 version was limited and could not be flushed.
- **Configuration files**: EF6 supports configuration via config files that can include the provider. EF Core requires a direct reference to the provider assembly and explicit provider registration (i.e. `UseSqlServer`).
- **Connection factories**: EF6 supported connection factories. EF Core does not support connection factories and always requires a connection string.
- **Logging**: in general, [logging in EF Core](xref:core/logging-events-diagnostics/index) is far more robust and has multiple options for fine-tuned configuration.

## Conventions

EF6 supported custom ("lightweight") conventions and model conventions. The lightweight conventions are similar to EF Core's [pre-convention model configuration](xref:core/what-is-new/ef-core-6.0/whatsnew#pre-convention-model-configuration). Other conventions are supported as part of model building.

EF6 runs conventions after the model is built. EF Core applies them as the model is being built. In EF Core, you can decouple model building from active sessions with a DbContext. It is possible to create a model initialized with the conventions.

## Data validation

EF Core does not support data validation and only uses data annotations for building the model and migrations. Most client libraries from web/MVC to WinForms and WPF provide a data validation implementation to use.

## Features that are coming soon

There are a few features in EF6 that don't exist yet in EF Core, but are on the product roadmap.

- **Table-per-concrete type (TPC)** was supported in EF6 along with "entity splitting." TPC is on the roadmap for EF7.
- **Stored procedure mapping** in EF6 allows you to delegate create, update, and delete operations to stored procedures. EF Core currently only allows mapping to stored procedures for reads. Create, update, and delete (CUD) support is on the roadmap for EF7.
- **Complex types** in EF6 are similar to owned types in EF Core. However, the full set of capabilities will be addressed with value objects in EF7.

## Leave ObjectContext behind

EF Core uses a [DbContext](xref:core/dbcontext-configuration/index) instead of an `ObjectContext`. You will have to update code that uses [IObjectContextAdapter](xref:System.Data.Entity.Infrastructure.IObjectContextAdapter). This was sometimes used for queries with `PreserveChanges` or `OverwriteChanges` merge option. For similar capabilities in EF Core, look into the [Reload](xref:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.Reload) method.

## Model configuration

There are many important differences between how models in EF6 and EF Core are designed. EF Core lacks full support for conditional mapping. It does not have model builder versions.

Other differences include:

### Type discovery

In EF Core, Entity Types are discovered by the engine in three ways:

- Expose a `DbSet<TEntity>` on your `DbContext` where `TEntity` is the type you wish to track.
- Reference a `Set<TEntity>` from somewhere in your code.
- Complex types referenced by discovered types are recursively discovered (for example, if your `Blog` references a `Post` and `Blog` is discoverable, `Post` will be discovered as well)

Assemblies are _not_ scanned for derived types.

### Mapping

The `.Map()` extension in EF6 has been replaced with overloads and extension methods in EF Core. For example, you can use '.HasDiscriminator()` to configure table-per-hierarchy (TPH). See: [Modeling Inheritance](xref:core/modeling/inheritance).

### Inheritance mapping

EF6 supported table-per-hierarchy (TPH), table-per-type (TPT) and table-per-concrete-class (TPC) and enabled hybrid mapping of different flavors at different levels of the hierarchy. EF Core will continue to require an inheritance chain to modeled one way (TPT or TPH) and the plan is to add support for TPC in EF7.

See: [Modeling Inheritance](xref:core/modeling/inheritance).

### Attributes

EF6 supported index attributes on properties. In EF Core, they are applied at the type level which should make it easier for scenarios that require composite indexes. EF Core doesn't support composite keys with data annotations (i.e. using Order in `ColumnAttribute` together with `KeyAttribute`).

For more information, see: [Indexes and constraints](xref:core/modeling/indexes).

### Required and optional

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

### Spatial support

EF Core integrates with the third-party library community library [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite) to provide spatial support.

### Independent associations

EF Core does not support independent associations (an EDM concept that allows the relationship between two entities to be defined independent from the entities themselves). A similar concept supported in EF Core is [shadow properties](xref:core/modeling/shadow-properties).

## Migrations

EF Core does not support database initializers or automatic migrations. Although there is no `migrate.exe` in EF Core, you can produce [migration bundles](xref:core/what-is-new/ef-core-6.0/whatsnew#migration-bundles).

## Visual Studio Tooling

EF Core has no designer, no functionality to update the model from the database and no model-first flow. There is no reverse-engineering wizard and no built-in templates.

Although these features do not ship with EF Core, there are OSS community projects that provide additional tooling. Specifically, [EF Core Power Tools](https://github.com/ErikEJ/EFCorePowerTools) provides:

- Reverse engineering from inside Visual Studio with support for database projects (`.dacpac`). Includes template-based code customizations.
- Visual inspection of DbContext with model graphing and scripting.
- Management of migrations from within Visual Studio using a GUI.

For a complete list of community tools and extensions, see: [EF Core Tools and Extensions](xref:core/extensions/index).

## Change tracking

There are several differences between how EF6 and EF Core deal with change tracking. These are summarized in the following table:

|Feature|EF6|EF Core|
|---|---|---|
|Entity State|Adds/attaches entire graph|Supports navigations to detached entities|
|Orphans|Preserved|Deleted|
|Disconnected, self-tracking entities|Supported|Not supported|
|Mutations|Performed on properties|Performed on backing fields*|
|Data-binding|`.Local`|`.Local` plus `.ToObservableCollection` or `.ToBindingList`|
|Change detection|Full graph|Per entity|

\* By default, property notification will not be triggered in EF Core so it's important to [configure notification entities](xref:core/change-tracking/change-detection#notification-entities).

Note that EF Core does not call change detection automatically as often as EF6.

EF Core introduces a detailed `DebugView` for the change tracker. To learn more, read [Change Tracker Debugging](xref:core/change-tracking/debug-views).

## Queries

EF6 has some query capabilities that do not exist in EF Core. These include:

- Some common C# function and SQL function mappings.
- Interception of the command tree for queries and updates.
- Support for table-valued parameters (TVPs).

EF6 has built-in support for lazy-loading proxies. This is an opt-in package for EF Core (see [Lazy Loading of Related Data](xref:core/querying/related-data/lazy)).

EF Core allows you to compose over raw SQL using `FromSQL`.
