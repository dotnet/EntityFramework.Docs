---
title: Breaking changes in EF Core 9 (EF9) - EF Core
description: List of breaking changes introduced in Entity Framework Core 9 (EF9)
author: SamMonoRT
ms.date: 01/17/2025
uid: core/what-is-new/ef-core-9.0/breaking-changes
---

# Breaking changes in EF Core 9 (EF9)

This page documents API and behavior changes that have the potential to break existing applications updating from EF Core 8 to EF Core 9. Make sure to review earlier breaking changes if updating from an earlier version of EF Core:

- [Breaking changes in EF Core 8](xref:core/what-is-new/ef-core-8.0/breaking-changes)
- [Breaking changes in EF Core 7](xref:core/what-is-new/ef-core-7.0/breaking-changes)
- [Breaking changes in EF Core 6](xref:core/what-is-new/ef-core-6.0/breaking-changes)

## Target Framework

EF Core 9 targets .NET 8. This means that existing applications that target .NET 8 can continue to do so. Applications targeting older .NET, .NET Core, and .NET Framework versions will need to target .NET 8 or .NET 9 to use EF Core 9.

## Summary

> [!NOTE]
> If you are using Azure Cosmos DB, please see the [separate section below on Azure Cosmos DB breaking changes](#azure-cosmos-db-breaking-changes).

| **Breaking change**                                                                                       | **Impact** |
|:----------------------------------------------------------------------------------------------------------|------------|
| [Exception is thrown when applying migrations if there are pending model changes](#pending-model-changes) | High       |
| [Exception is thrown when applying migrations in an explicit transaction](#migrations-transaction)        | High       |
| [`Microsoft.EntityFrameworkCore.Design` not found when using EF tools](#tools-design)                     | Medium     |
| [`EF.Functions.Unhex()` now returns `byte[]?`](#unhex)                                                    | Low        |
| [SqlFunctionExpression's nullability arguments' arity validated](#sqlfunctionexpression-nullability)      | Low        |
| [`ToString()` method now returns empty string for `null` instances](#nullable-tostring)                   | Low        |
| [Shared framework dependencies were updated to 9.0.x](#shared-framework-dependencies)                     | Low        |

## High-impact changes

<a name="pending-model-changes"></a>

### Exception is thrown when applying migrations if there are pending model changes

[Tracking Issue #33732](https://github.com/dotnet/efcore/issues/33732)

#### Old behavior

If the model has pending changes compared to the last migration they are not applied with the rest of the migrations when `Migrate` is called.

#### New behavior

Starting with EF Core 9.0, if the model has pending changes compared to the last migration an exception is thrown when `dotnet ef database update`, `Migrate` or `MigrateAsync` is called:
> :::no-loc text="The model for context 'DbContext' has pending changes. Add a new migration before updating the database. This exception can be suppressed or logged by passing event ID 'RelationalEventId.PendingModelChangesWarning' to the 'ConfigureWarnings' method in 'DbContext.OnConfiguring' or 'AddDbContext'.":::

#### Why

Forgetting to add a new migration after making model changes is a common mistake that can be hard to diagnose in some cases. The new exception ensures that the app's model matches the database after the migrations are applied.

#### Mitigations

There are several common situations when this exception can be thrown:

- There are no migrations at all. This is common when the database is updated through other means.
  - **Mitigation**: If you don't plan to use migrations for managing the database schema then remove the `Migrate` or `MigrateAsync` call, otherwise add a migration.
- There is at least one migration, but the model snapshot is missing. This is common for migrations created manually.
  - **Mitigation**: Add a new migration using EF tooling, this will update the model snapshot.
- The model wasn't modified by the developer, but it's built in a non-deterministic way causing EF to detect it as modified. This is common when `new DateTime()`, `DateTime.Now`, `DateTime.UtcNow`, or `Guid.NewGuid()` are used in objects supplied to `HasData()`.
  - **Mitigation**: Add a new migration, examine its contents to locate the cause, and replace the dynamic data with a static, hardcoded value in the model. The migration should be recreated after the model is fixed. If dynamic data has to be used for seeding consider using [the new seeding pattern](/ef/core/what-is-new/ef-core-9.0/whatsnew#improved-data-seeding) instead of `HasData()`.
- The last migration was created for a different provider than the one used to apply the migrations.
  - **Mitigation**: This is an unsupported scenario. The warning can be suppressed using the code snippet below, but this scenario will likely stop working in a future EF Core release. The recommended solution is [to generate a separate set of migrations for each provider](xref:core/managing-schemas/migrations/providers).
- The migrations are generated or chosen dynamically by replacing some of the EF services.
  - **Mitigation**: The warning is a false positive in this case and should be suppressed:
  
    `options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))`

If your scenario doesn't fall under any of the above cases and adding a new migration creates the same migration each time or an empty migration and the exception is still thrown then create a small repro project and [share it with the EF team in a new issue](https://github.com/dotnet/efcore/issues/new/choose).

<a name="migrations-transaction"></a>

### Exception is thrown when applying migrations in an explicit transaction

[Tracking Issue #17578](https://github.com/dotnet/efcore/issues/17578)

#### Old behavior

To apply migrations resiliently the following pattern was commonly used:

```csharp
await dbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
{
    await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
    await dbContext.Database.MigrateAsync(cancellationToken);
    await transaction.CommitAsync(cancellationToken);
});
```

#### New behavior

Starting with EF Core 9.0, `Migrate` and `MigrateAsync` calls will start a transaction and execute the commands using an `ExecutionStrategy` and if your app uses the above pattern an exception is thrown:
> :::no-loc text="An error was generated for warning 'Microsoft.EntityFrameworkCore.Migrations.MigrationsUserTransactionWarning': A transaction was started before applying migrations. This prevents a database lock to be acquired and hence the database will not be protected from concurrent migration applications. The transactions and execution strategy are already managed by EF as needed. Remove the external transaction. This exception can be suppressed or logged by passing event ID 'RelationalEventId.MigrationsUserTransactionWarning' to the 'ConfigureWarnings' method in 'DbContext.OnConfiguring' or 'AddDbContext'.":::

#### Why

Using an explicit transaction prevents a database lock to be acquired and hence the database will not be protected from concurrent migration applications, it also limits EF on how it can manage the transactions internally.

#### Mitigations

If there is only one database call inside the transaction then remove the external transaction and `ExecutionStrategy`:

```csharp
await dbContext.Database.MigrateAsync(cancellationToken);
```

Otherwise, if your scenario requires an explicit transaction and you have other mechanism in place to prevent concurrent migration application, then ignore the warning:
  
```csharp
options.ConfigureWarnings(w => w.Ignore(RelationalEventId.MigrationsUserTransactionWarning))
```

## Medium-impact changes

<a name="tools-design"></a>

### `Microsoft.EntityFrameworkCore.Design` not found when using EF tools

[Tracking Issue #35265](https://github.com/dotnet/efcore/issues/35265)

#### Old behavior

Previusly, the EF tools required `Microsoft.EntityFrameworkCore.Design` to be referenced in the following way.

```XML
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="*.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
```

#### New behavior

Starting with .NET SDK 9.0.200 an exception is thrown when an EF tool is invoked:
> :::no-loc text="Could not load file or assembly 'Microsoft.EntityFrameworkCore.Design, Culture=neutral, PublicKeyToken=null'. The system cannot find the file specified.":::

#### Why

EF tools were relying on an undocumented behavior of .NET SDK that caused private assets to be included in the generated `.deps.json` file. This was fixed in [sdk#45259](https://github.com/dotnet/sdk/pull/45259). Unfortunately, the EF change to account for this doesn't meet the servicing bar for EF 9.0.x, so it will be fixed in EF 10.

#### Mitigations

As a workaround before EF 10 is released you can mark the `Design` assembly reference as publishable:

```XML
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.1">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <Publish>true</Publish>
    </PackageReference>
```

This will include it in the generated `.deps.json` file, but has a side effect of copying `Microsoft.EntityFrameworkCore.Design.dll` to the output and publish folders.

## Low-impact changes

<a name="unhex"></a>

### `EF.Functions.Unhex()` now returns `byte[]?`

[Tracking Issue #33864](https://github.com/dotnet/efcore/issues/33864)

#### Old behavior

The `EF.Functions.Unhex()` function was previously annotated to return `byte[]`.

#### New behavior

Starting with EF Core 9.0, Unhex() is now annotated to return `byte[]?`.

#### Why

`Unhex()` is translated to the SQLite `unhex` function, which returns NULL for invalid inputs. As a result, `Unhex()` returned `null` for those cases, in violation of the annotation.

#### Mitigations

If you are sure that the text content passed to `Unhex()` represents a valid, hexadecimal string, you can simply add the null-forgiving operator as an assertion that the invocation will never return null:

```csharp
var binaryData = await context.Blogs.Select(b => EF.Functions.Unhex(b.HexString)!).ToListAsync();
```

Otherwise, add runtime checks for null on the return value of Unhex().

<a name="sqlfunctionexpression-nullability"></a>

### SqlFunctionExpression's nullability arguments' arity validated

[Tracking Issue #33852](https://github.com/dotnet/efcore/issues/33852)

#### Old behavior

Previously it was possible to create a `SqlFunctionExpression` with a different number of arguments and nullability propagation arguments.

#### New behavior

Starting with EF Core 9.0, EF now throws if the number of arguments and nullability propagation arguments do not match.

#### Why

Not having matching number of arguments and nullability propagation arguments can lead to unexpected behavior.

#### Mitigations

Make sure the `argumentsPropagateNullability` has same number of elements as the `arguments`. When in doubt use `false` for nullability argument.

<a name="nullable-tostring"></a>

### `ToString()` method now returns empty string for `null` instances

[Tracking Issue #33941](https://github.com/dotnet/efcore/issues/33941)

#### Old behavior

Previously EF returned inconsistent results for the `ToString()` method when the argument value was `null`. E.g. `ToString()` on `bool?` property with `null` value returned `null`, but for non-property `bool?` expressions whose value was `null` it returned `True`. The behavior was also inconsistent for other data types, e.g. `ToString()` on `null` value enum returned empty string.

#### New behavior

Starting with EF Core 9.0, the `ToString()` method now consistently returns empty string in all cases when the argument value is `null`.

#### Why

The old behavior was inconsistent across different data types and situations, as well as not aligned with the [C# behavior](/dotnet/api/system.nullable-1.tostring#returns).

#### Mitigations

To revert to the old behavior, rewrite the query accordingly:

```csharp
var newBehavior = context.Entity.Select(x => x.NullableBool.ToString());
var oldBehavior = context.Entity.Select(x => x.NullableBool == null ? null : x.NullableBool.ToString());
```

<a name="shared-framework-dependencies"></a>

### Shared framework dependencies were updated to 9.0.x

#### Old behavior

Apps using the `Microsoft.NET.Sdk.Web` SDK and targetting net8.0 would resolve packages like `System.Text.Json`, `Microsoft.Extensions.Caching.Memory`, `Microsoft.Extensions.Configuration.Abstractions`, `Microsoft.Extensions.Logging` and `Microsoft.Extensions.DependencyModel` from the shared framework, so these assemblies wouldn't normally be deployed with the app.

#### New behavior

While EF Core 9.0 still supports net8.0 it now references the 9.0.x versions of `System.Text.Json`, `Microsoft.Extensions.Caching.Memory`, `Microsoft.Extensions.Configuration.Abstractions`, `Microsoft.Extensions.Logging` and `Microsoft.Extensions.DependencyModel`. Apps targetting net8.0 will not be able to leverage the shared framework to avoid deploying these assemblies.

#### Why

The matching dependency versions contain the latest security fixes and using them simplifies the servicing model for EF Core.

#### Mitigations

Change your app to target net9.0 to get the previous behavior.

## Azure Cosmos DB breaking changes

Extensive work has gone into making the Azure Cosmos DB provider better in 9.0. The changes include a number of high-impact breaking changes; if you are upgrading an existing application, please read the following carefully.

| **Breaking change**                                                                                                                                  | **Impact** |
|:---------------------------------------------------------------------------------------------------------------------------------------------------- | ---------- |
| [The discriminator property is now named `$type` instead of `Discriminator`](#cosmos-discriminator-name-change)                                      | High       |
| [The `id` property no longer contains the discriminator by default](#cosmos-id-property-changes)                                                     | High       |
| [The JSON `id` property is mapped to the key](#cosmos-key-changes)                                                                                   | High       |
| [Sync I/O via the Azure Cosmos DB provider is no longer supported](#cosmos-nosync)                                                                   | Medium     |
| [SQL queries must now project JSON values directly](#cosmos-sql-queries-with-value)                                                                  | Medium     |
| [Undefined results are now automatically filtered from query results](#cosmos-undefined-filtering)                                                   | Medium     |
| [Incorrectly translated queries are no longer translated](#cosmos-incorrect-translations)                                                            | Medium     |
| [`HasIndex` now throws instead of being ignored](#cosmos-hasindex-throws)                                                                            | Low        |
| [`IncludeRootDiscriminatorInJsonId` was renamed to `HasRootDiscriminatorInJsonId` after 9.0.0-rc.2](#cosmos-IncludeRootDiscriminatorInJsonId-rename) | Low        |

### High-impact changes

<a name="cosmos-discriminator-name-change"></a>

#### The discriminator property is now named `$type` instead of `Discriminator`

[Tracking Issue #34269](https://github.com/dotnet/efcore/issues/34269)

##### Old behavior

EF automatically adds a discriminator property to JSON documents to identify the entity type that the document represents. In previous versions of EF, this JSON property used to be named `Discriminator` by default.

##### New behavior

Starting with EF Core 9.0, the discriminator property is now called `$type` by default. If you have existing documents in Azure Cosmos DB from previous versions of EF, these use the old `Discriminator` naming, and after upgrading to EF 9.0, queries against those documents will fail.

##### Why

An emerging JSON practice uses a `$type` property in scenarios where a document's type needs to be identified. For example, .NET's System.Text.Json also supports polymorphism, using `$type` as its default discriminator property name ([docs](/dotnet/standard/serialization/system-text-json/polymorphism#customize-the-type-discriminator-name)). To align with the rest of the ecosystem and make it easier to interoperate with external tools, the default was changed.

##### Mitigations

The easiest mitigation is to simply configure the name of the discriminator property to be `Discriminator`, just as before:

```csharp
modelBuilder.Entity<Session>().HasDiscriminator<string>("Discriminator");
```

Doing this for all your top-level entity types will make EF behave just like before.

At this point, if you wish, you can also update all your documents to use the new `$type` naming.

<a name="cosmos-id-property-changes"></a>

#### The `id` property now contains only the EF key property by default

[Tracking Issue #34179](https://github.com/dotnet/efcore/issues/34179)

##### Old behavior

Previously, EF inserted the discriminator value of your entity type into the `id` property of the document. For example, if you saved a `Blog` entity type with an `Id` property containing 8, the JSON `id` property would contain `Blog|8`.

##### New behavior

Starting with EF Core 9.0, the JSON `id` property no longer contains the discriminator value, and only contains the value of your key property. For the above example, the JSON `id` property would simply be `8`. If you have existing documents in Azure Cosmos DB from previous versions of EF, these have the discriminator value in the JSON `id` property, and after upgrading to EF 9.0, queries against those documents will fail.

##### Why

Since the JSON `id` property must be unique, the discriminator was previously added to it to allow different entities with the same key value to exist. For example, this allowed having both a `Blog` and a `Post` with an `Id` property containing the value 8 within the same container and partition. This aligned better with relational database data modeling patterns, where each entity type is mapped to its own table, and therefore has its own key-space.

EF 9.0 generally changed the mapping to be more aligned with common Azure Cosmos DB NoSQL practices and expectations, rather than to correspond to the expectations of users coming from relational databases. In addition, having the discriminator value in the `id` property made it more difficult for external tools and systems to interact with EF-generated JSON documents; such external systems aren't generally aware of the EF discriminator values, which are by default derived from .NET types.

##### Mitigations

The easiest mitigation is to simply configure EF to include the discriminator in the JSON `id` property, as before. A new configuration option has been introduced for this purpose:

```csharp
modelBuilder.Entity<Session>().HasDiscriminatorInJsonId();
```

Doing this for all your top-level entity types will make EF behave just like before.

At this point, if you wish, you can also update all your documents to rewrite their JSON `id` property. Note that this is only possible if entities of different types don't share the same id value within the same container.

<a name="cosmos-key-changes"></a>

#### The JSON `id` property is mapped to the key

[Tracking Issue #34179](https://github.com/dotnet/efcore/issues/34179)

##### Old behavior

Previously, EF created a shadow property mapped to the JSON `id` property, unless one of the properties was mapped to `id` explicitly.

##### New behavior

Starting with EF Core 9, the key property will be mapped to the JSON `id` property by convention if possible. This means that the key property will no longer be persisted in the document under a different name with the same value, so non-EF code consuming the documents and relying on this property being present would no longer function correctly.

##### Why

EF 9.0 generally changed the mapping to be more aligned with common Azure Cosmos DB NoSQL practices and expectations. And it is not common to store the key value twice in the document.

##### Mitigations

If you would like to preserve EF Core 8 behavior the easiest mitigation is to use a new configuration option that has been introduced for this purpose:

```csharp
modelBuilder.Entity<Session>().HasShadowId();
```

Doing this for all your top-level entity types will make EF behave just like before. Or you could apply it to all entity types in the model with one call:

```csharp
modelBuilder.HasShadowIds();
```

### Medium-impact changes

<a name="cosmos-nosync"></a>

#### Sync I/O via the Azure Cosmos DB provider is no longer supported

[Tracking Issue #32563](https://github.com/dotnet/efcore/issues/32563)

##### Old behavior

Previously, calling synchronous methods like `ToList` or `SaveChanges` would cause EF Core to block synchronously using `.GetAwaiter().GetResult()` when executing async calls against the Azure Cosmos DB SDK. This can result in deadlock.

##### New behavior

Starting with EF Core 9.0, EF now throws by default when attempting to use synchronous I/O. The exception message is "Azure Cosmos DB does not support synchronous I/O. Make sure to use and correctly await only async methods when using Entity Framework Core to access Azure Cosmos DB. See [https://aka.ms/ef-cosmos-nosync](https://aka.ms/ef-cosmos-nosync) for more information."

##### Why

Synchronous blocking on asynchronous methods can result in deadlock, and the Azure Cosmos DB SDK only supports async methods.

##### Mitigations

In EF Core 9.0, the error can be suppressed with:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.ConfigureWarnings(w => w.Ignore(CosmosEventId.SyncNotSupported));
}
```

That being said, applications should stop using sync APIs with Azure Cosmos DB since this is not supported by the Azure Cosmos DB SDK. The ability to suppress the exception will be removed in a future release of EF Core, after which the only option will be to use async APIs.

<a name="cosmos-sql-queries-with-value"></a>

#### SQL queries must now project JSON values directly

[Tracking Issue #25527](https://github.com/dotnet/efcore/issues/25527)

##### Old behavior

Previously, EF generated queries such as the following:

```sql
SELECT c["City"] FROM root c
```

Such queries cause Azure Cosmos DB to wrap each result in a JSON object, as follows:

```json
[
    {
        "City": "Berlin"
    },
    {
        "City": "México D.F."
    }
]
```

##### New behavior

Starting with EF Core 9.0, EF now adds the `VALUE` modifier to queries as follows:

```sql
SELECT VALUE c["City"] FROM root c
```

Such queries cause Azure Cosmos DB to return the values directly, without being wrapped:

```json
[
    "Berlin",
    "México D.F."
]
```

If your application makes use of [SQL queries](xref:core/providers/cosmos/querying#sql-queries), such queries are likely broken after upgrading to EF 9.0, as they don't include the `VALUE` modifier.

##### Why

Wrapping each result in an additional JSON object can cause performance degradation in some scenarios, bloats the JSON result payload, and isn't the natural way to work with Azure Cosmos DB.

##### Mitigations

To mitigate, simply add the `VALUE` modifier to the projections of your SQL queries, as shown above.

<a name="cosmos-undefined-filtering"></a>

#### Undefined results are now automatically filtered from query results

[Tracking Issue #25527](https://github.com/dotnet/efcore/issues/25527)

##### Old behavior

Previously, EF generated queries such as the following:

```sql
SELECT c["City"] FROM root c
```

Such queries cause Azure Cosmos DB to wrap each result in a JSON object, as follows:

```json
[
    {
        "City": "Berlin"
    },
    {
        "City": "México D.F."
    }
]
```

If any of the results were undefined (e.g. the `City` property was absent from the document), an empty document was returned, and EF would return `null` for that result.

##### New behavior

Starting with EF Core 9.0, EF now adds the `VALUE` modifier to queries as follows:

```sql
SELECT VALUE c["City"] FROM root c
```

Such queries cause Azure Cosmos DB to return the values directly, without being wrapped:

```json
[
    "Berlin",
    "México D.F."
]
```

The Azure Cosmos DB behavior is to automatically filter `undefined` values out of results; this means that if one of the `City` properties is absent from the document, the query would return just a single result, rather than two results, with one being `null`.

##### Why

Wrapping each result in an additional JSON object can cause performance degradation in some scenarios, bloats the JSON result payload, and isn't the natural way to work with Azure Cosmos DB.

##### Mitigations

If getting `null` values for undefined results is important for your application, coalesce the `undefined` values to `null` using the new `EF.Functions.Coalesce` operator:

```csharp
var users = await context.Customer
    .Select(c => EF.Functions.CoalesceUndefined(c.City, null))
    .ToListAsync();
```

<a name="cosmos-incorrect-translations"></a>

#### Incorrectly translated queries are no longer translated

[Tracking Issue #34123](https://github.com/dotnet/efcore/issues/34123)

##### Old behavior

Previously, EF translated queries such as the following:

```csharp
var sessions = await context.Sessions
    .Take(5)
    .Where(s => s.Name.StartsWith("f"))
    .ToListAsync();
```

However, the SQL translation for this query was incorrect:

```sql
SELECT c
FROM root c
WHERE ((c["Discriminator"] = "Session") AND STARTSWITH(c["Name"], "f"))
OFFSET 0 LIMIT @__p_0
```

In SQL, the `WHERE` clause is evaluated _before_ the `OFFSET` and `LIMIT` clauses; but in the LINQ query above, the `Take` operator appears before the `Where` operator. As a result, such queries could return incorrect results.

##### New behavior

Starting with EF Core 9.0, such queries are no longer translated, and an exception is thrown.

##### Why

Incorrect translations can cause silent data corruption, which can introduce hard-to-discover bugs in your application. EF always prefer to fail-fast by throwing up-front rather than to possibly cause data corruption.

##### Mitigations

If you were happy with the previous behavior and would like to execute the same SQL, simply swap around the order of LINQ operators:

```csharp
var sessions = await context.Sessions
    .Where(s => s.Name.StartsWith("f"))
    .Take(5)
    .ToListAsync();
```

Unfortunately, Azure Cosmos DB does not currently support the `OFFSET` and `LIMIT` clauses in SQL subqueries, which is what the proper translation of the original LINQ query requires.

### Low-impact changes

<a name="cosmos-hasindex-throws"></a>

#### `HasIndex` now throws instead of being ignored

[Tracking Issue #34023](https://github.com/dotnet/efcore/issues/34023)

##### Old behavior

Previously, calls to <xref: Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder`1.HasIndex*> were ignored by the EF Cosmos DB provider.

##### New behavior

The provider now throws if <xref: Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder`1.HasIndex*> is specified.

##### Why

In Azure Cosmos DB, all properties are indexed by default, and no indexing needs to be specified. While it's possible to define a custom indexing policy, this isn't currently supported by EF, and can be done via the Azure Portal without EF support. Since <xref: Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder`1.HasIndex*> calls weren't doing anything, they are no longer allowed.

##### Mitigations

Remove any calls to <xref: Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder`1.HasIndex*>.

<a name="cosmos-IncludeRootDiscriminatorInJsonId-rename"></a>

#### `IncludeRootDiscriminatorInJsonId` was renamed to `HasRootDiscriminatorInJsonId` after 9.0.0-rc.2

[Tracking Issue #34717](https://github.com/dotnet/efcore/pull/34717)

##### Old behavior

The `IncludeRootDiscriminatorInJsonId` API was introduced in 9.0.0 rc.1.

##### New behavior

For the final release of EF Core 9.0, the API was renamed to `HasRootDiscriminatorInJsonId`

##### Why

Another related API was renamed to start with `Has` instead of `Include`, and so this one was renamed for consistency as well.

##### Mitigations

If your code is using the `IncludeRootDiscriminatorInJsonId` API, simply change it to reference `HasRootDiscriminatorInJsonId` instead.
