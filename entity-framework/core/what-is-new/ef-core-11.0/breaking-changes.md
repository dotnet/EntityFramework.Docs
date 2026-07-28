---
title: Breaking changes in EF Core 11 (EF11) - EF Core
description: List of breaking changes introduced in Entity Framework Core 11 (EF11)
author: AndriySvyryd
ms.date: 07/13/2026
uid: core/what-is-new/ef-core-11.0/breaking-changes
---

# Breaking changes in EF Core 11 (EF11)

This page documents API and behavior changes that have the potential to break existing applications updating from EF Core 10 to EF Core 11. Make sure to review earlier breaking changes if updating from an earlier version of EF Core:

- [Breaking changes in EF Core 10](xref:core/what-is-new/ef-core-10.0/breaking-changes)
- [Breaking changes in EF Core 9](xref:core/what-is-new/ef-core-9.0/breaking-changes)
- [Breaking changes in EF Core 8](xref:core/what-is-new/ef-core-8.0/breaking-changes)
- [Breaking changes in EF Core 7](xref:core/what-is-new/ef-core-7.0/breaking-changes)
- [Breaking changes in EF Core 6](xref:core/what-is-new/ef-core-6.0/breaking-changes)

## Summary

| **Breaking change**                                                                                       | **Impact** |
|:----------------------------------------------------------------------------------------------------------|------------|
| [Cosmos: `__jObject` shadow property removed; JObject no longer used for serialization](#jObject-removed) | High       |
| [Cosmos: Unmapped properties are no longer preserved](#unmapped-properties)                               | High       |
| [Cosmos: Floating-point values are now truncated when materializing to fixed-point types](#truncation)    | Low        |

## High-impact changes

<a name="jObject-removed"></a>

### Cosmos: `__jObject` shadow property removed; JObject no longer used for serialization

[Tracking Issue #5421](https://github.com/dotnet/EntityFramework.Docs/issues/5421)

#### Old behavior

Previously, the Azure Cosmos DB provider added a shadow property named `"__jObject"` of type `JObject` (from `Newtonsoft.Json`) to every entity type. This property contained the raw JSON document as received from and sent to Cosmos DB, allowing users to access unmapped or raw data:

```csharp
var order = await context.Orders.FirstAsync();
var rawJson = context.Entry(order).Property<JObject>("__jObject").CurrentValue;
var billingAddress = rawJson["BillingAddress"]?.Value<string>();
```

EF Core used `Newtonsoft.Json` (via `JObject`) internally for document serialization and deserialization.

#### New behavior

Starting with EF Core 11, the `__jObject` shadow property no longer exists. EF Core now uses `System.Text.Json` (`Utf8JsonReader`/`Utf8JsonWriter`) for document serialization and deserialization, and no longer relies on `Newtonsoft.Json`.

Accessing the `"__jObject"` property will throw an `InvalidOperationException`.

#### Why

The `JObject`-based approach required a dependency on `Newtonsoft.Json` and limited performance improvements. Switching to `System.Text.Json` aligns EF Core Cosmos with the rest of the .NET ecosystem and enables significant performance gains.

#### Mitigations

To access the raw JSON document, use the `CosmosClient` directly instead of relying on `__jObject`:

```csharp
var cosmosClient = context.Database.GetCosmosClient();
var container = cosmosClient.GetContainer("myDatabase", "myContainer");
var response = await container.ReadItemAsync<JsonElement>("1", new PartitionKey("1"));
var billingAddress = response.Resource.GetProperty("BillingAddress").GetString();
```

For more information, see [Working with Unstructured Data in Azure Cosmos DB](xref:core/providers/cosmos/unstructured-data).

<a name="unmapped-properties"></a>

### Cosmos: Unmapped properties are no longer preserved

[Tracking Issue #5421](https://github.com/dotnet/EntityFramework.Docs/issues/5421)

#### Old behavior

Previously, when EF Core read a Cosmos DB document that contained properties not mapped in the EF model, those extra properties were preserved in the `__jObject` shadow property and written back to the database on the next `SaveChanges`. This meant unmapped data in documents was transparently round-tripped.

#### New behavior

Starting with EF Core 11, unmapped properties in a Cosmos DB document are ignored when reading. Any extra JSON properties that are not part of the EF model will be lost if the entity is subsequently saved.

#### Why

Because `__jObject` has been removed (see above), there is no mechanism to preserve unmapped properties. EF Core 11 uses a lean JSON reader that only processes the properties it knows about.

#### Mitigations

If your application relies on preserving unmapped data, consider one of the following options:

- **Use `CosmosClient` directly** for documents where you need full control over the JSON shape.
- **Map all relevant properties** explicitly in your EF model, even if they are not used by application logic.
- **Use a JSON column or an untyped dictionary property** to capture extra data, if that pattern fits your model.

## Low-impact changes

<a name="truncation"></a>

### Cosmos: Floating-point values are now truncated when materializing to fixed-point types

[Tracking Issue #38138](https://github.com/dotnet/efcore/issues/38138)

#### Old behavior

Previously, when a query projection returned a floating-point value (e.g., the result of a numeric expression such as `3 / 4` returned by Cosmos as `0.75`) and the target property was a fixed-point type (`int`, `long`, `decimal`, etc.), EF Core would **round** the value. This meant `0.75` would materialize as `1`.

#### New behavior

Starting with EF Core 11, such values are **truncated** instead of rounded. `0.75` now materializes as `0`, matching standard .NET integer truncation behavior (`(int)0.75 == 0`).

#### Why

Truncation is the standard .NET behavior for explicit numeric conversions and is consistent with how other providers behave. The previous rounding behavior was a bug.

#### Mitigations

If you relied on the previous rounding behavior, apply explicit rounding in your queries using `Math.Round`:

```csharp
var result = await context.Products
    .Select(p => (int)Math.Round((double)(p.Int / (p.Int + 1))))
    .SingleAsync();
```
