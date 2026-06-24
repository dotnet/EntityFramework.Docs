---
title: Breaking changes in EF Core 11 (EF11) - EF Core
description: List of breaking changes introduced in Entity Framework Core 11 (EF11)
author: roji
ms.date: 03/27/2026
uid: core/what-is-new/ef-core-11.0/breaking-changes
---

# Breaking changes in EF Core 11 (EF11)

This page documents API and behavior changes that have the potential to break existing applications updating from EF Core 10 to EF Core 11. Make sure to review earlier breaking changes if updating from an earlier version of EF Core:

- [Breaking changes in EF Core 10](xref:core/what-is-new/ef-core-10.0/breaking-changes)
- [Breaking changes in EF Core 9](xref:core/what-is-new/ef-core-9.0/breaking-changes)
- [Breaking changes in EF Core 8](xref:core/what-is-new/ef-core-8.0/breaking-changes)

## Summary

> [!NOTE]
> If you are using Microsoft.Data.Sqlite, please see the [separate section below on Microsoft.Data.Sqlite breaking changes](#MDS-breaking-changes).

| **Breaking change**                                                                                             | **Impact** |
|:--------------------------------------------------------------------------------------------------------------- | -----------|
| [Sync I/O via the Azure Cosmos DB provider has been fully removed](#cosmos-nosync)                              | Medium     |
| [Microsoft.Data.SqlClient has been updated to 7.0](#sqlclient-7)                                                | Medium     |
| [Cosmos: illegal `id` characters are no longer escaped](#cosmos-no-id-escape)                                   | Medium     |
| [SQL Server compatibility level now defaults to 160](#sqlserver-compatibility-level-160)                       | Low        |
| [EF Core now throws by default when no migrations are found](#migrations-not-found)                             | Low        |
| [`EFOptimizeContext` MSBuild property has been removed](#ef-optimize-context-removed)                           | Low        |
| [EF tools packages no longer reference Microsoft.EntityFrameworkCore.Design](#ef-tools-no-design-dep)           | Low        |
| [SqlVector properties are no longer loaded by default](#sqlvector-not-auto-loaded)                              | Low        |
| [Cosmos: empty owned collections now return an empty collection instead of null](#cosmos-empty-collections)     | Low        |
| [Cosmos: the default discriminator property is now named `Discriminator` in the model](#cosmos-discriminator-property-name) | Low        |
| [Owned JSON collections without an explicit key are obsolete](#owned-json-collections-obsolete)                 | Low        |

## Medium-impact changes

<a name="cosmos-nosync"></a>

### Sync I/O via the Azure Cosmos DB provider has been fully removed

[Tracking Issue #37059](https://github.com/dotnet/efcore/issues/37059)

#### Old behavior

Synchronous I/O via the Azure Cosmos DB provider has been unsupported since EF 9.0 ([note](/ef/core/what-is-new/ef-core-9.0/breaking-changes#cosmos-nosync)); calling any sync I/O API - like `ToList` or `SaveChanges` threw an exception, unless a special opt-in was configured. When the opt-in was configured, sync I/O APIs worked as before, causing the provider to perform "sync-over-async" blocking against the Azure Cosmos DB SDK, which could result in deadlocks and other performance issues.

#### New behavior

Starting with EF Core 11.0, EF now always throws when a synchronous I/O API is called. There is no way to opt back into using sync I/O APIs.

#### Why

Synchronous blocking on asynchronous methods ("sync-over-async") is highly discouraged, and can lead to deadlock and other performance problems. Since the Azure Cosmos DB SDK only supports async methods, so does the EF Cosmos provider.

#### Mitigations

Convert your code to use async I/O APIs instead of sync I/O ones. For example, replace calls to `SaveChanges()` with `await SaveChangesAsync()`.

<a name="sqlclient-7"></a>

### Microsoft.Data.SqlClient has been updated to 7.0

#### Old behavior

EF Core 10 used [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/) 6.x, which included Azure/Entra ID authentication dependencies (such as `Azure.Core`, `Azure.Identity`, and `Microsoft.Identity.Client`) in the core package.

#### New behavior

EF Core 11 now depends on [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/) 7.0. This version removes Azure/Entra ID (formerly Azure Active Directory) authentication dependencies from the core package. If your application uses Entra ID authentication (for example, `ActiveDirectoryDefault`, `ActiveDirectoryInteractive`, `ActiveDirectoryManagedIdentity`, or `ActiveDirectoryServicePrincipal`), you must now install the [`Microsoft.Data.SqlClient.Extensions.Azure`](https://www.nuget.org/packages/Microsoft.Data.SqlClient.Extensions.Azure/) package separately.

In addition, `SqlAuthenticationMethod.ActiveDirectoryPassword` has been marked as obsolete.

For more details, see the [Microsoft.Data.SqlClient 7.0 release notes](https://github.com/dotnet/SqlClient/blob/main/release-notes/7.0/7.0.0.md).

#### Why

This change was made in [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/) to reduce dependency bloat for applications that don't use Azure authentication, which is especially beneficial for containerized deployments and local development.

#### Mitigations

If your application uses Entra ID authentication with SQL Server, add a reference to the `Microsoft.Data.SqlClient.Extensions.Azure` package in your project:

```xml
<PackageReference Include="Microsoft.Data.SqlClient.Extensions.Azure" Version="7.0.0" />
```

No code changes are required beyond adding this package reference. If you use `SqlAuthenticationMethod.ActiveDirectoryPassword`, migrate to a modern authentication method such as `ActiveDirectoryDefault` or `ActiveDirectoryInteractive`.

<a name="cosmos-no-id-escape"></a>

### Cosmos: illegal `id` characters are no longer escaped

[Tracking Issue #38244](https://github.com/dotnet/efcore/issues/38244)

#### Old behavior

Previously, when generating the Cosmos `id` property value from a composite key that contains multiple parts, the Azure Cosmos DB provider escaped certain characters that are illegal in Cosmos resource `id` values:

| Character | Escaped as |
|-----------|------------|
| `/`       | `^2F`      |
| `\`       | `^5C`      |
| `?`       | `^3F`      |
| `#`       | `^23`      |

#### New behavior

Starting with EF Core 11.0, these characters are no longer escaped in the generated `id` value. The `id` value will contain the raw key values without modification. Note that when `id` values are concatenated (i.e. when using a composite key or when the discriminator-in-id behavior is opted into), the `|` character is used as a separator—and any `|` characters already present in key values are escaped to avoid ambiguity. No other escaping is applied.

The old escape behavior can be re-enabled by setting an `AppContext` switch:

```csharp
AppContext.SetSwitch("Microsoft.EntityFrameworkCore.EscapeIllegalCosmosIdCharacters", true);
```

#### Why

The previous escaping scheme was non-injective: the escape character `^` was never itself escaped. This meant that a key value containing the literal string `^2F` would produce the same `id` as a key value containing `/`, resulting in silent data corruption where two entities with distinct primary keys would be mapped to the same Cosmos document. Stopping the escaping altogether fixes the collision problem.

#### Mitigations

If your application uses composite keys whose values can contain the characters `/`, `\`, `?`, or `#`, be aware of the following:

- **Existing data**: Documents previously stored in Cosmos DB have `id` values using the old escape sequences (e.g. `Post|1|^2F`). After upgrading to EF Core 11, EF will generate unescaped `id` values (e.g. `Post|1|/`) and will no longer find those existing documents. To continue accessing existing data without migration, opt back into the old behavior using the `AppContext` switch described above—however, be aware that the id-collision bug will still be present.

- **New data**: If you are creating a new application or database, avoid using these illegal characters in key values, as they are not valid in Cosmos DB resource `id` values. See the [Azure documentation](/dotnet/api/microsoft.azure.documents.resource.id) for details.

## Low-impact changes

<a name="sqlserver-compatibility-level-160"></a>

### SQL Server compatibility level now defaults to 160

[Tracking Issue #38198](https://github.com/dotnet/efcore/issues/38198)

#### Old behavior

Previously, when using <xref:Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseSqlServer*> without explicitly configuring a SQL Server compatibility level, EF Core defaulted to compatibility level 150, corresponding to SQL Server 2019.

#### New behavior

Starting with EF Core 11.0, <xref:Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseSqlServer*> defaults to compatibility level 160, corresponding to SQL Server 2022. This allows EF to generate SQL which uses SQL Server 2022 features by default. For example, some queries now use `LEAST` and `GREATEST`, including translations for `Math.Min`, `Math.Max`, <xref:Microsoft.EntityFrameworkCore.RelationalDbFunctionsExtensions.Least*>, <xref:Microsoft.EntityFrameworkCore.RelationalDbFunctionsExtensions.Greatest*>, and some `Take`/`Skip` patterns.

If your database runs on SQL Server 2019 or older, or is configured with a compatibility level lower than 160, some SQL generated by EF Core may no longer be supported by the database.

#### Why

SQL Server 2022 has been available for several years, and using compatibility level 160 by default allows EF Core to generate simpler and more efficient SQL for newer SQL Server versions.

#### Mitigations

If your database does not support compatibility level 160, configure EF Core to use the compatibility level supported by your database:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer("<connection string>", o => o.UseCompatibilityLevel(150));
}
```

For more information, see the [SQL Server compatibility level documentation](xref:core/providers/sql-server/index#compatibility-level).

<a name="migrations-not-found"></a>

### EF Core now throws by default when no migrations are found

[Tracking Issue #35218](https://github.com/dotnet/efcore/issues/35218)

#### Old behavior

Previously, when calling <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.Migrate*> or <xref:Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.MigrateAsync*> on a database with no migrations in the assembly, EF Core logged an informational message and returned without applying any changes.

#### New behavior

Starting with EF Core 11.0, EF Core throws an exception by default when no migrations are found in the assembly. This is consistent with the `PendingModelChangesWarning` behavior [introduced in EF 9.0](xref:core/what-is-new/ef-core-9.0/breaking-changes#pending-model-changes).

#### Why

Calling `Migrate()` or `MigrateAsync()` when no migrations exist typically indicates a misconfiguration. Rather than silently continuing and leaving the database in a potentially incorrect state, EF Core now alerts developers to this issue immediately.

#### Mitigations

If you intentionally call `Migrate()` without having any migrations (for example, because you manage the database schema through other means), remove the `Migrate()` call or suppress the exception by configuring warnings:

```csharp
options.ConfigureWarnings(w => w.Ignore(RelationalEventId.MigrationsNotFound))
```

Or to log the event instead of throwing:

```csharp
options.ConfigureWarnings(w => w.Log(RelationalEventId.MigrationsNotFound))
```

<a name="ef-optimize-context-removed"></a>

### `EFOptimizeContext` MSBuild property has been removed

[Tracking Issue #35079](https://github.com/dotnet/efcore/issues/35079)

#### Old behavior

Previously, the `EFOptimizeContext` MSBuild property could be set to `true` to enable compiled model and precompiled query code generation during build or publish:

```xml
<EFOptimizeContext Condition="'$(Configuration)'=='Release'">true</EFOptimizeContext>
```

#### New behavior

Starting with EF Core 11.0, the `EFOptimizeContext` MSBuild property has been removed. Code generation is now controlled exclusively through the `EFScaffoldModelStage` and `EFPrecompileQueriesStage` properties. When `PublishAOT` is set to `true`, code generation is automatically enabled during publish without needing any additional property.

#### Why

The `EFScaffoldModelStage` and `EFPrecompileQueriesStage` properties already provide fine-grained control over when code generation occurs. `EFOptimizeContext` was a redundant enablement gate.

#### Mitigations

Replace usages of `EFOptimizeContext` with the `EFScaffoldModelStage` and `EFPrecompileQueriesStage` properties. These can be set to `publish` or `build` to control at which stage code generation occurs:

```xml
<EFScaffoldModelStage>publish</EFScaffoldModelStage>
<EFPrecompileQueriesStage>publish</EFPrecompileQueriesStage>
```

Any other value (for example, `none`) disables the corresponding generation.

If you have `PublishAOT` set to `true`, code generation is automatically enabled during publish and no additional configuration is needed.

<a name="ef-tools-no-design-dep"></a>

### EF tools packages no longer reference Microsoft.EntityFrameworkCore.Design

[Tracking Issue #37739](https://github.com/dotnet/efcore/issues/37739)

#### Old behavior

Previously, the `Microsoft.EntityFrameworkCore.Tools` and `Microsoft.EntityFrameworkCore.Tasks` NuGet packages had a dependency on `Microsoft.EntityFrameworkCore.Design`.

#### New behavior

Starting with EF Core 11.0, the `Microsoft.EntityFrameworkCore.Tools` and `Microsoft.EntityFrameworkCore.Tasks` NuGet packages no longer have a dependency on `Microsoft.EntityFrameworkCore.Design`.

#### Why

There was no hard dependency on the code in `Microsoft.EntityFrameworkCore.Design`, and this dependency was causing issues when using the latest `Microsoft.EntityFrameworkCore.Tools` with projects targeting older frameworks.

#### Mitigations

If your project relies on `Microsoft.EntityFrameworkCore.Design` being brought in transitively through the tools packages, add a direct reference to it in your project:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="11.0.0" PrivateAssets="all" />
```

<a name="sqlvector-not-auto-loaded"></a>

### SqlVector properties are no longer loaded by default

[Tracking Issue #37279](https://github.com/dotnet/efcore/issues/37279)

#### Old behavior

Previously, when querying entities with `SqlVector<T>` properties, EF Core included the vector column in `SELECT` statements and populated the property on the returned entity.

#### New behavior

Starting with EF Core 11.0, `SqlVector<T>` properties are no longer included in `SELECT` statements when materializing entities. The property will be `null` on returned entities.

Vector properties can still be used in `WHERE` and `ORDER BY` clauses—including with `VectorDistance()` and `VectorSearch()`; they just won't be included in the entity projection.

#### Why

Vector columns can be very large, containing hundreds or thousands of floating-point values. In the vast majority of cases, vectors are written to the database and then used for search, without needing to be read back. Excluding them from `SELECT` by default avoids unnecessary data transfer.

#### Mitigations

> [!NOTE]
> A mechanism for opting vector properties back into automatic loading will be introduced later in the EF Core 11 release.

If you need to read back vector values, use an explicit projection:

```csharp
var embeddings = await context.Blogs
    .Select(b => new { b.Id, b.Embedding })
    .ToListAsync();
```

<a name="cosmos-empty-collections"></a>

### Cosmos: empty owned collections now return an empty collection instead of null

[Tracking Issue #36577](https://github.com/dotnet/efcore/issues/36577)

#### Old behavior

Previously, when querying entities via the Azure Cosmos DB provider where an owned collection contained no items, the collection property was `null` on the materialized entity.

#### New behavior

Starting with EF Core 11.0, the Azure Cosmos DB provider correctly initializes empty owned collections, returning an empty collection instead of `null`.

#### Why

The previous behavior of materializing empty owned collections as `null` was a bug.

#### Mitigations

If your code explicitly checks owned collection properties for `null` to detect that the collection is empty, those checks can simply be removed, since the collection is now always initialized:

```csharp
// Before
if (entity.OwnedCollection is null or { Count: 0 })
{
    // treated as empty
}

// After
if (entity.OwnedCollection is { Count: 0 })
{
    // treated as empty
}
```

<a name="cosmos-discriminator-property-name"></a>

### Cosmos: the default discriminator property is now named `Discriminator` in the model

[Pull Request #38458](https://github.com/dotnet/efcore/pull/38458)

#### Old behavior

EF automatically adds a discriminator property to identify the entity type that a JSON document represents. The name of this property in the JSON document was changed from `Discriminator` to `$type` in [EF Core 9.0](xref:core/what-is-new/ef-core-9.0/breaking-changes#cosmos-discriminator-name-change). To achieve this, EF used `$type` as the name of the discriminator property both in the EF model and in the stored JSON document.

Because `$type` is not a valid C# identifier, the resulting shadow property name caused invalid code to be generated for [compiled models](xref:core/performance/advanced-performance-topics#compiled-models) and [precompiled queries](xref:core/performance/nativeaot-and-precompiled-queries) used with Native AOT.

#### New behavior

Starting with EF Core 11.0, the default discriminator property is once again named `Discriminator` in the EF model, while the name written to the JSON document is unchanged and remains `$type` by default. In other words, the model property name and the JSON property name are now decoupled:

- `entityType.FindDiscriminatorProperty().Name` returns `Discriminator`.
- `entityType.FindDiscriminatorProperty().GetJsonPropertyName()` returns `$type`.

The format of the stored documents is **not** affected by this change, so existing data continues to work without modification.

#### Why

EF derives some generated C# identifiers (for example, shadow property variable names) from model metadata such as property names. Since `$type` is not a valid C# identifier, using it as the model property name produced uncompilable code for compiled models and precompiled queries. Naming the model property `Discriminator` (a valid identifier) while still writing `$type` to the document keeps generated code valid without changing the on-disk format.

#### Mitigations

For most applications no action is needed, since stored documents are unaffected and continue to use `$type`.

If your code references the discriminator by its **model** property name, `$type` (for example, via <xref:Microsoft.EntityFrameworkCore.EF.Property*> in a query or query filter, or by looking the property up in the metadata), update it to use `Discriminator` instead:

```csharp
// Before
var query = context.Set<Session>().Where(e => EF.Property<string>(e, "$type") == "Lecture");

// After
var query = context.Set<Session>().Where(e => EF.Property<string>(e, "Discriminator") == "Lecture");
```

To change the JSON discriminator property name for the whole model in a single place--for example, to align it with the model property name--use the model-level <xref:Microsoft.EntityFrameworkCore.ModelBuilder.HasEmbeddedDiscriminatorName*> API instead of configuring each entity type individually:

```csharp
modelBuilder.HasEmbeddedDiscriminatorName("Discriminator");
```

To change only the JSON name for a specific entity type--for example, to align it with the model property name--configure the discriminator property's JSON name with <xref:Microsoft.EntityFrameworkCore.CosmosPropertyBuilderExtensions.ToJsonProperty*>:

```csharp
modelBuilder.Entity<Session>().Property<string>("Discriminator").ToJsonProperty("Discriminator");
```

To restore the previous behavior where the discriminator property is also named `$type` in the model, configure its name explicitly with <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder.HasDiscriminator*>. Note that this reintroduces an invalid C# identifier and is not recommended when using compiled models or precompiled queries:

```csharp
modelBuilder.Entity<Session>().HasDiscriminator<string>("$type");
```

<a name="owned-json-collections-obsolete"></a>

### Owned JSON collections without an explicit key are obsolete

[Tracking Issue #37289](https://github.com/dotnet/efcore/issues/37289)

#### Old behavior

Previously, owned entity types mapped to a JSON column via <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.OwnedNavigationBuilder.ToJson*> could be used as collections without configuring an explicit primary key. EF Core would synthesize an ordinal (positional) key behind the scenes to identify each item in the collection:

```csharp
public class Blog
{
    public int Id { get; set; }
    public List<Post> Posts { get; set; } = new();
}

public class Post
{
    // No key property
    public required string Title { get; set; }
    public required string Content { get; set; }
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
    => modelBuilder.Entity<Blog>().OwnsMany(b => b.Posts, b => b.ToJson());
```

#### New behavior

Starting with EF Core 11.0, configuring an owned JSON collection without an explicit key produces an `OwnedEntityMappedToJsonCollectionWarning` warning. The mapping continues to work, but is now considered deprecated and is expected to be removed in a future release.

Owned JSON entities that have an explicit primary key, as well as non-collection owned JSON references, are not affected by this change.

#### Why

[Complex types](xref:core/what-is-new/ef-core-10.0/whatsnew#complex-types) became fully supported in EF Core 10, including for [JSON mapping](xref:core/what-is-new/ef-core-10.0/whatsnew#json). Complex types are a better fit than owned types for JSON documents: they have value semantics and no identity, which avoids many of the issues that come from using owned entity types—which are entity types—to model what is fundamentally a value embedded in another document. In particular, owned JSON collections without an explicit key relied on a synthetic ordinal key, which has known limitations and corner cases.

#### Mitigations

The recommended mitigation is to migrate the type to a complex type, which is now the preferred way to map types to JSON:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
    => modelBuilder.Entity<Blog>().ComplexCollection(b => b.Posts, b => b.ToJson());
```

Alternatively, if you need to keep the owned-type mapping, configure a non-shadow primary key on the owned type. Once a key is configured, the warning no longer applies:

```csharp
public class Post
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
    => modelBuilder.Entity<Blog>().OwnsMany(b => b.Posts, b =>
    {
        b.ToJson();
        b.HasKey(p => p.Id);
    });
```

If you cannot migrate immediately, you can suppress the warning via `ConfigureWarnings`:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.ConfigureWarnings(w => w.Ignore(CoreEventId.OwnedEntityMappedToJsonCollectionWarning));
```

<a name="MDS-breaking-changes"></a>

## Microsoft.Data.Sqlite breaking changes

> [!NOTE]
> [SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw) is an external, community-maintained library that is not owned or maintained by Microsoft. Microsoft.Data.Sqlite depends on it for its SQLite connectivity.

### Summary

| **Breaking change**                                                                                       | **Impact** |
|:----------------------------------------------------------------------------------------------------------|------------|
| [Encryption-enabled SQLite packages have been removed](#sqlite-encryption-removed)                        | Medium     |
| [Some SQLitePCLRaw bundle packages have been removed](#sqlite-bundles-removed)                            | Medium     |

### Medium-impact changes

<a name="sqlite-encryption-removed"></a>

#### Encryption-enabled SQLite packages have been removed

[Tracking Issue #5108](https://github.com/dotnet/EntityFramework.Docs/issues/5108)

##### Old behavior

Previously, the `SQLitePCLRaw.bundle_e_sqlcipher` NuGet package provided encryption-enabled SQLite builds at no cost.

##### New behavior

Starting with SQLitePCLRaw 3.0 (used by Microsoft.Data.Sqlite 11.0), the `SQLitePCLRaw.bundle_e_sqlcipher` package has been deprecated and removed from NuGet. No-cost encryption-enabled SQLite builds are no longer distributed.

##### Why

The previous no-cost `SQLitePCLRaw.bundle_e_sqlcipher` package was barely maintained, which is a significant concern for encryption software where security vulnerabilities may go unpatched. The SQLitePCLRaw maintainer removed these builds in version 3.0 in favor of professionally maintained, paid alternatives that provide ongoing security updates.

##### Mitigations

If you need SQLite encryption, you have the following options:

- **SQLite Encryption Extension (SEE)**: This is the official encryption implementation from the SQLite team. A paid license is required. See [https://sqlite.org/com/see.html](https://sqlite.org/com/see.html) for details. NuGet packages are available through [SourceGear's SQLite build service](https://github.com/ericsink/SQLitePCL.raw/wiki/SQLite-encryption-options-for-use-with-SQLitePCLRaw).
- **SQLCipher**: Purchase supported builds from [Zetetic](https://www.zetetic.net/sqlcipher/), or build the [open source code](https://github.com/sqlcipher/sqlcipher) yourself.
- **SQLite3 Multiple Ciphers**: NuGet packages are available from [SQLite3MultipleCiphers-NuGet](https://github.com/utelle/SQLite3MultipleCiphers-NuGet).
  - When encrypting a new database or opening an existing database that was encrypted with SQLCipher, you must configure the cipher scheme in the connection string using URI parameters—for example: `Data Source=file:example.db?cipher=sqlcipher&legacy=4;Password=<password>`. See [How to open an existing database encrypted with SQLCipher](https://github.com/utelle/SQLite3MultipleCiphers-NuGet#how-to-open-an-existing-database-encrypted-with-sqlcipher) for details.

For more details, see [SQLite encryption options for use with SQLitePCLRaw](https://github.com/ericsink/SQLitePCL.raw/wiki/SQLite-encryption-options-for-use-with-SQLitePCLRaw) and [SQLitePCLRaw 3.0 Release Notes](https://github.com/ericsink/SQLitePCL.raw/blob/main/v3.md).

<a name="sqlite-bundles-removed"></a>

#### Some SQLitePCLRaw bundle packages have been removed

[Tracking Issue #5108](https://github.com/dotnet/EntityFramework.Docs/issues/5108)

##### Old behavior

Previously, the `SQLitePCLRaw.bundle_sqlite3`, `SQLitePCLRaw.bundle_winsqlite3`, `SQLitePCLRaw.bundle_green`, and `SQLitePCLRaw.bundle_e_sqlite3mc` packages provided a convenient way to configure SQLitePCLRaw with the corresponding SQLite provider.

##### New behavior

Starting with SQLitePCLRaw 3.0 (used by Microsoft.Data.Sqlite 11.0), these bundle packages have been removed. If your application depended on one of these bundles, you must now reference the corresponding provider package and explicitly initialize it.

##### Why

Each of these bundle packages contained only a single line of configuration code and added unnecessary packaging overhead. The corresponding provider packages are still supported.

##### Mitigations

Replace the removed bundle package with the corresponding provider package and add explicit initialization code.

**If using `bundle_sqlite3` or `bundle_winsqlite3`**, replace the package reference:

```xml
<!-- Old -->
<PackageReference Include="SQLitePCLRaw.bundle_sqlite3" Version="2.x.x" />
<!-- or -->
<PackageReference Include="SQLitePCLRaw.bundle_winsqlite3" Version="2.x.x" />

<!-- New -->
<PackageReference Include="SQLitePCLRaw.provider.sqlite3" Version="3.x.x" />
<!-- or -->
<PackageReference Include="SQLitePCLRaw.provider.winsqlite3" Version="3.x.x" />
```

Then add explicit initialization before using SQLite:

```csharp
// For sqlite3
static void Init()
{
    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
}

// For winsqlite3
static void Init()
{
    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_winsqlite3());
}
```

**If using `bundle_green`**, the recommended migration path is to switch to `SQLitePCLRaw.bundle_e_sqlite3`. Alternatively, use `SQLitePCLRaw.config.e_sqlite3` paired with a separate native library package like `SourceGear.sqlite3`, which allows you to update the SQLite version independently:

```xml
<PackageReference Include="SQLitePCLRaw.bundle_e_sqlite3" Version="3.x.x" />
```

If you only target iOS and want to continue using the system SQLite library, reference the provider directly:

```xml
<PackageReference Include="SQLitePCLRaw.core" Version="3.x.x" />
<PackageReference Include="SQLitePCLRaw.provider.sqlite3" Version="3.x.x" />
```

And initialize it explicitly:

```csharp
static void Init()
{
    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
}
```

> [!NOTE]
> If you are using `SQLitePCLRaw.bundle_e_sqlite3`, no changes are required—just update the version number. See the [SQLitePCLRaw 3.0 Release Notes](https://github.com/ericsink/SQLitePCL.raw/blob/main/v3.md) for details.
