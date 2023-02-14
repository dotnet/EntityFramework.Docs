---
title: Azure Cosmos DB Provider - EF Core
description: Documentation for the database provider that allows Entity Framework Core to be used with Azure Cosmos DB.
author: AndriySvyryd
ms.date: 02/12/2023
uid: core/providers/cosmos/index
---
# EF Core Azure Cosmos DB Provider

This database provider allows Entity Framework Core to be used with Azure Cosmos DB. The provider is maintained as part of the [Entity Framework Core Project](https://github.com/dotnet/efcore).

It is strongly recommended to familiarize yourself with the [Azure Cosmos DB documentation](/azure/cosmos-db/introduction) before reading this section.

> [!NOTE]
> This provider only works with Azure Cosmos DB for NoSQL.

## Install

Install the [Microsoft.EntityFrameworkCore.Cosmos NuGet package](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Cosmos/).

### [.NET Core CLI](#tab/dotnet-core-cli)

```dotnetcli
dotnet add package Microsoft.EntityFrameworkCore.Cosmos
```

### [Visual Studio](#tab/vs)

```powershell
Install-Package Microsoft.EntityFrameworkCore.Cosmos
```

***

## Get started

> [!TIP]
> You can view this article's [sample on GitHub](https://github.com/dotnet/EntityFramework.Docs/tree/main/samples/core/Cosmos).

As for other providers the first step is to call [UseCosmos](/dotnet/api/Microsoft.EntityFrameworkCore.CosmosDbContextOptionsExtensions.UseCosmos):

[!code-csharp[Configuration](../../../../samples/core/Cosmos/ModelBuilding/OrderContext.cs?name=Configuration)]

> [!WARNING]
> The endpoint and key are hardcoded here for simplicity, but in a production app these should be [stored securely](/aspnet/core/security/app-secrets#secret-manager). See [Connecting and authenticating](xref:core/providers/cosmos/index#connecting-and-authenticating) for different ways to connect to Azure Cosmos DB.

In this example `Order` is a simple entity with a reference to the [owned type](xref:core/modeling/owned-entities) `StreetAddress`.

[!code-csharp[Order](../../../../samples/core/Cosmos/ModelBuilding/Order.cs?name=Order)]

[!code-csharp[StreetAddress](../../../../samples/core/Cosmos/ModelBuilding/StreetAddress.cs?name=StreetAddress)]

Saving and querying data follows the normal EF pattern:

[!code-csharp[HelloCosmos](../../../../samples/core/Cosmos/ModelBuilding/Sample.cs?name=HelloCosmos)]

> [!IMPORTANT]
> Calling [EnsureCreatedAsync](/dotnet/api/Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.EnsureCreatedAsync) is necessary to create the required containers and insert the [seed data](xref:core/modeling/data-seeding) if present in the model. However `EnsureCreatedAsync` should only be called during deployment, not normal operation, as it may cause performance issues.

## Connecting and authenticating

The Azure Cosmos DB provider for EF Core has multiple overloads of the [UseCosmos](/dotnet/api/Microsoft.EntityFrameworkCore.CosmosDbContextOptionsExtensions.UseCosmos) method. These overloads support the different ways that a connection can be made to the database, and the different ways of ensuring that the connection is secure.

> [!IMPORTANT]
> Make sure to understand [_Secure access to data in Azure Cosmos DB_](/azure/cosmos-db/secure-access-to-data) to understand the security implications and best practices for using each overload of the `UseCosmos` method.

| Connection Mechanism       | UseCosmos Overload                                                     | More information                                                                          |
|----------------------------|------------------------------------------------------------------------|-------------------------------------------------------------------------------------------|
| Account endpoint and key   | `UseCosmos<DbContext>(accountEndpoint, accountKey, databaseName)`      | [Primary/secondary keys](/azure/cosmos-db/secure-access-to-data#primary-keys)             |
| Account endpoint and token | `UseCosmos<DbContext>(accountEndpoint, tokenCredential, databaseName)` | [Resource tokens](/azure/cosmos-db/secure-access-to-data#primary-keys)                    |
| Connection string          | `UseCosmos<DbContext>(connectionString, databaseName)`                 | [Work with account keys and connection strings](/azure/cosmos-db/scripts/cli/common/keys) |

## Queries

### LINQ queries

[EF Core LINQ queries](xref:core/querying/index) can be executed against Azure Cosmos DB in the same way as for other database providers. For example:

<!--
        var stringResults = await context.Triangles.Where(
                e => e.Name.Length > 4
                     && e.Name.Trim().ToLower() != "obtuse"
                     && e.Name.TrimStart().Substring(2, 2).Equals("uT", StringComparison.OrdinalIgnoreCase))
            .ToListAsync();
-->
[!code-csharp[StringTranslations](../../../../samples/core/Miscellaneous/NewInEFCore6.Cosmos/CosmosQueriesSample.cs?name=StringTranslations)]

> [!NOTE]
> The Azure Cosmos DB provider does not translate the same set of LINQ queries as other providers. See [_Limitations_](xref:core/providers/cosmos/limitations) for more information.

### SQL queries

Queries can also be written [directly in SQL](xref:core/querying/sql-queries). For example:

<!--
            var maxAngle = 60;
            var results = await context.Triangles.FromSqlRaw(
                    @"SELECT * FROM root c WHERE c[""Angle1""] <= {0} OR c[""Angle2""] <= {0}", maxAngle)
                .ToListAsync();
-->
[!code-csharp[FromSql](../../../../samples/core/Miscellaneous/NewInEFCore6.Cosmos/CosmosQueriesSample.cs?name=FromSql)]

This query results in the following query execution:

```sql
SELECT c
FROM (
    SELECT * FROM root c WHERE c["Angle1"] <= @p0 OR c["Angle2"] <= @p0
) c
```

Just like for relational `FromSql` queries, the hand written SQL can be further composed using LINQ operators. For example:

<!--
            var maxAngle = 60;
            var results = await context.Triangles.FromSqlRaw(
                    @"SELECT * FROM root c WHERE c[""Angle1""] <= {0} OR c[""Angle2""] <= {0}", maxAngle)
                .Where(e => e.InsertedOn <= DateTime.UtcNow)
                .Select(e => e.Angle1).Distinct()
                .ToListAsync();
-->
[!code-csharp[FromSqlComposed](../../../../samples/core/Miscellaneous/NewInEFCore6.Cosmos/CosmosQueriesSample.cs?name=FromSqlComposed)]

This combination of SQL and LINQ is translated to:

```sql
SELECT DISTINCT c["Angle1"]
FROM (
    SELECT * FROM root c WHERE c["Angle1"] <= @p0 OR c["Angle2"] <= @p0
) c
WHERE (c["InsertedOn"] <= GetCurrentDateTime())
```

## Azure Cosmos DB options

It is also possible to configure the Azure Cosmos DB provider with a single connection string and to specify other options to customize the connection:

[!code-csharp[Configuration](../../../../samples/core/Cosmos/ModelBuilding/OptionsContext.cs?name=Configuration)]

> [!TIP]
> The code above shows possible options. It is not intended that these will all be used at the same time! See the [Azure Cosmos DB Options documentation](/dotnet/api/microsoft.azure.cosmos.cosmosclientoptions) for a detailed description of the effect of each option mentioned above.

## Cosmos-specific model customization

By default all entity types are mapped to the same container, named after the derived context (`"OrderContext"` in this case). To change the default container name use [HasDefaultContainer](/dotnet/api/Microsoft.EntityFrameworkCore.CosmosModelBuilderExtensions.HasDefaultContainer):

[!code-csharp[DefaultContainer](../../../../samples/core/Cosmos/ModelBuilding/OrderContext.cs?name=DefaultContainer)]

To map an entity type to a different container use [ToContainer](/dotnet/api/Microsoft.EntityFrameworkCore.CosmosEntityTypeBuilderExtensions.ToContainer):

[!code-csharp[Container](../../../../samples/core/Cosmos/ModelBuilding/OrderContext.cs?name=Container)]

To identify the entity type that a given item represent EF Core adds a discriminator value even if there are no derived entity types. The name and value of the discriminator [can be changed](xref:core/modeling/inheritance).

If no other entity type will ever be stored in the same container the discriminator can be removed by calling [HasNoDiscriminator](/dotnet/api/Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder.HasNoDiscriminator):

[!code-csharp[NoDiscriminator](../../../../samples/core/Cosmos/ModelBuilding/OrderContext.cs?name=NoDiscriminator)]

### Partition keys

By default, EF Core will create containers with the partition key set to `"__partitionKey"` without supplying any value for it when inserting items. But to fully leverage the performance capabilities of Azure Cosmos DB, a [carefully selected partition key](/azure/cosmos-db/partition-data) should be used. It can be configured by calling [HasPartitionKey](/dotnet/api/Microsoft.EntityFrameworkCore.CosmosEntityTypeBuilderExtensions.HasPartitionKey):

[!code-csharp[PartitionKey](../../../../samples/core/Cosmos/ModelBuilding/OrderContext.cs?name=PartitionKey)]

> [!NOTE]
>The partition key property can be of any type as long as it is [converted to string](xref:core/modeling/value-conversions).

Once configured the partition key property should always have a non-null value. A query can be made single-partition by adding a <xref:Microsoft.EntityFrameworkCore.CosmosQueryableExtensions.WithPartitionKey%2A> call.

[!code-csharp[PartitionKey](../../../../samples/core/Cosmos/ModelBuilding/Sample.cs?name=PartitionKey&highlight=14)]

It is generally recommended to add the partition key to the primary key as that best reflects the server semantics and allows some optimizations, for example in `FindAsync`.

### Provisioned throughput

If you use EF Core to create the Azure Cosmos DB database or containers you can configure [provisioned throughput](/azure/cosmos-db/set-throughput) for the database by calling <xref:Microsoft.EntityFrameworkCore.CosmosModelBuilderExtensions.HasAutoscaleThroughput%2A?displayProperty=nameWithType> or <xref:Microsoft.EntityFrameworkCore.CosmosModelBuilderExtensions.HasManualThroughput%2A?displayProperty=nameWithType>. For example:

<!--
modelBuilder.HasManualThroughput(2000);
modelBuilder.HasAutoscaleThroughput(4000);
-->
[!code-csharp[ModelThroughput](../../../../samples/core/Miscellaneous/NewInEFCore6.Cosmos/CosmosModelConfigurationSample.cs?name=ModelThroughput)]

To configure provisioned throughput for a container call <xref:Microsoft.EntityFrameworkCore.CosmosEntityTypeBuilderExtensions.HasAutoscaleThroughput%2A?displayProperty=nameWithType> or <xref:Microsoft.EntityFrameworkCore.CosmosEntityTypeBuilderExtensions.HasManualThroughput%2A?displayProperty=nameWithType>. For example:

<!--
modelBuilder.Entity<Family>(
    entityTypeBuilder =>
    {
        entityTypeBuilder.HasManualThroughput(5000);
        entityTypeBuilder.HasAutoscaleThroughput(3000);
    });
-->
[!code-csharp[EntityTypeThroughput](../../../../samples/core/Miscellaneous/NewInEFCore6.Cosmos/CosmosModelConfigurationSample.cs?name=EntityTypeThroughput)]

### Time-to-live

Entity types in the Azure Cosmos DB model can now be configured with a default time-to-live. For example:

```csharp
modelBuilder.Entity<Hamlet>().HasDefaultTimeToLive(3600);
```

Or, for the analytical store:

```csharp
modelBuilder.Entity<Hamlet>().HasAnalyticalStoreTimeToLive(3600);
```

Time-to-live for individual entities can be set using a property mapped to "ttl" in the JSON document. For example:

<!--
            modelBuilder.Entity<Village>()
                .HasDefaultTimeToLive(3600)
                .Property(e => e.TimeToLive)
                .ToJsonProperty("ttl");
-->
[!code-csharp[TimeToLiveProperty](../../../../samples/core/Miscellaneous/NewInEFCore6.Cosmos/CosmosModelConfigurationSample.cs?name=TimeToLiveProperty)]

> [!NOTE]
> A default time-to-live must configured on the entity type for the "ttl" to have any effect. See [_Time to Live (TTL) in Azure Cosmos DB_](/azure/cosmos-db/nosql/time-to-live) for more information.

The time-to-live property is then set before the entity is saved. For example:

<!--
        var village = new Village { Id = "DN41", Name = "Healing", TimeToLive = 60 };
        context.Add(village);
        await context.SaveChangesAsync();
-->
[!code-csharp[SetTtl](../../../../samples/core/Miscellaneous/NewInEFCore6.Cosmos/CosmosModelConfigurationSample.cs?name=SetTtl)]

The time-to-live property can be a [shadow property](xref:core/modeling/shadow-properties) to avoid polluting the domain entity with database concerns. For example:

<!--
            modelBuilder.Entity<Hamlet>()
                .HasDefaultTimeToLive(3600)
                .Property<int>("TimeToLive")
                .ToJsonProperty("ttl");
-->
[!code-csharp[TimeToLiveShadowProperty](../../../../samples/core/Miscellaneous/NewInEFCore6.Cosmos/CosmosModelConfigurationSample.cs?name=TimeToLiveShadowProperty)]

The shadow time-to-live property is then set by [accessing the tracked entity](xref:core/change-tracking/entity-entries). For example:

<!--
        var hamlet = new Hamlet { Id = "DN37", Name = "Irby" };
        context.Add(hamlet);
        context.Entry(hamlet).Property("TimeToLive").CurrentValue = 60;
        await context.SaveChangesAsync();
-->
[!code-csharp[SetTtlShadow](../../../../samples/core/Miscellaneous/NewInEFCore6.Cosmos/CosmosModelConfigurationSample.cs?name=SetTtlShadow)]

## Embedded entities

> [!NOTE]
> Related entity types are configured as owned by default. To prevent this for a specific entity type call <xref:Microsoft.EntityFrameworkCore.ModelBuilder.Entity%2A?displayProperty=nameWithType>.

For Azure Cosmos DB, owned entities are embedded in the same item as the owner. To change a property name use [ToJsonProperty](/dotnet/api/Microsoft.EntityFrameworkCore.CosmosEntityTypeBuilderExtensions.ToJsonProperty):

[!code-csharp[PropertyNames](../../../../samples/core/Cosmos/ModelBuilding/OrderContext.cs?name=PropertyNames)]

With this configuration the order from the example above is stored like this:

```json
{
    "Id": 1,
    "PartitionKey": "1",
    "TrackingNumber": null,
    "id": "1",
    "Address": {
        "ShipsToCity": "London",
        "ShipsToStreet": "221 B Baker St"
    },
    "_rid": "6QEKAM+BOOABAAAAAAAAAA==",
    "_self": "dbs/6QEKAA==/colls/6QEKAM+BOOA=/docs/6QEKAM+BOOABAAAAAAAAAA==/",
    "_etag": "\"00000000-0000-0000-683c-692e763901d5\"",
    "_attachments": "attachments/",
    "_ts": 1568163674
}
```

Collections of owned entities are embedded as well. For the next example we'll use the `Distributor` class with a collection of `StreetAddress`:

[!code-csharp[Distributor](../../../../samples/core/Cosmos/ModelBuilding/Distributor.cs?name=Distributor)]

The owned entities don't need to provide explicit key values to be stored:

[!code-csharp[OwnedCollection](../../../../samples/core/Cosmos/ModelBuilding/Sample.cs?name=OwnedCollection)]

They will be persisted in this way:

```json
{
    "Id": 1,
    "Discriminator": "Distributor",
    "id": "Distributor|1",
    "ShippingCenters": [
        {
            "City": "Phoenix",
            "Street": "500 S 48th Street"
        },
        {
            "City": "Anaheim",
            "Street": "5650 Dolly Ave"
        }
    ],
    "_rid": "6QEKANzISj0BAAAAAAAAAA==",
    "_self": "dbs/6QEKAA==/colls/6QEKANzISj0=/docs/6QEKANzISj0BAAAAAAAAAA==/",
    "_etag": "\"00000000-0000-0000-683c-7b2b439701d5\"",
    "_attachments": "attachments/",
    "_ts": 1568163705
}
```

Internally EF Core always needs to have unique key values for all tracked entities. The primary key created by default for collections of owned types consists of the foreign key properties pointing to the owner and an `int` property corresponding to the index in the JSON array. To retrieve these values entry API could be used:

[!code-csharp[ImpliedProperties](../../../../samples/core/Cosmos/ModelBuilding/Sample.cs?name=ImpliedProperties)]

> [!TIP]
> When necessary the default primary key for the owned entity types can be changed, but then key values should be provided explicitly.

### Collections of primitive types

Collections of supported primitive types, such as `string` and `int`, are discovered and mapped automatically. Supported collections are all types that implement <xref:System.Collections.Generic.IReadOnlyList%601> or <xref:System.Collections.Generic.IReadOnlyDictionary%602>. For example, consider this entity type:

<!--
public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public IList<string> Quotes { get; set; }
    public IDictionary<string, string> Notes { get; set; }
}
-->
[!code-csharp[BookEntity](../../../../samples/core/Miscellaneous/NewInEFCore6.Cosmos/CosmosPrimitiveTypesSample.cs?name=BookEntity)]

Both the list and the dictionary can be populated and inserted into the database in the normal way:

<!--
using var context = new BooksContext();

var book = new Book
{
    Title = "How It Works: Incredible History",
    Quotes = new List<string>
    {
        "Thomas (Tommy) Flowers was the British engineer behind the design of the Colossus computer.",
        "Invented originally for Guinness, plastic widgets are nitrogen-filled spheres.",
        "For 20 years after its introduction in 1979, the Walkman dominated the personal stereo market."
    },
    Notes = new Dictionary<string, string>
    {
        { "121", "Fridges" },
        { "144", "Peter Higgs" },
        { "48", "Saint Mark's Basilica" },
        { "36", "The Terracotta Army" }
    }
};

context.Add(book);
context.SaveChanges();
-->
[!code-csharp[Insert](../../../../samples/core/Miscellaneous/NewInEFCore6.Cosmos/CosmosPrimitiveTypesSample.cs?name=Insert)]

This results in the following JSON document:

```json
{
    "Id": "0b32283e-22a8-4103-bb4f-6052604868bd",
    "Discriminator": "Book",
    "Notes": {
        "36": "The Terracotta Army",
        "48": "Saint Mark's Basilica",
        "121": "Fridges",
        "144": "Peter Higgs"
    },
    "Quotes": [
        "Thomas (Tommy) Flowers was the British engineer behind the design of the Colossus computer.",
        "Invented originally for Guinness, plastic widgets are nitrogen-filled spheres.",
        "For 20 years after its introduction in 1979, the Walkman dominated the personal stereo market."
    ],
    "Title": "How It Works: Incredible History",
    "id": "Book|0b32283e-22a8-4103-bb4f-6052604868bd",
    "_rid": "t-E3AIxaencBAAAAAAAAAA==",
    "_self": "dbs/t-E3AA==/colls/t-E3AIxaenc=/docs/t-E3AIxaencBAAAAAAAAAA==/",
    "_etag": "\"00000000-0000-0000-9b50-fc769dc901d7\"",
    "_attachments": "attachments/",
    "_ts": 1630075016
}
```

These collections can then be updated, again in the normal way:

<!--
book.Quotes.Add("Pressing the emergency button lowered the rods again.");
book.Notes["48"] = "Chiesa d'Oro";

context.SaveChanges();
-->
[!code-csharp[Updates](../../../../samples/core/Miscellaneous/NewInEFCore6.Cosmos/CosmosPrimitiveTypesSample.cs?name=Updates)]

Limitations:

* Only dictionaries with string keys are supported
* Querying into the contents of primitive collections is not currently supported. Vote for [#16926](https://github.com/dotnet/efcore/issues/16926), [#25700](https://github.com/dotnet/efcore/issues/25700), and [#25701](https://github.com/dotnet/efcore/issues/25701) if these features are important to you.

## Working with disconnected entities

Every item needs to have an `id` value that is unique for the given partition key. By default EF Core generates the value by concatenating the discriminator and the primary key values, using '|' as a delimiter. The key values are only generated when an entity enters the `Added` state. This might pose a problem when [attaching entities](xref:core/saving/disconnected-entities) if they don't have an `id` property on the .NET type to store the value.

To work around this limitation one could create and set the `id` value manually or mark the entity as added first, then changing it to the desired state:

[!code-csharp[Attach](../../../../samples/core/Cosmos/ModelBuilding/Sample.cs?highlight=4&name=Attach)]

This is the resulting JSON:

```json
{
    "Id": 1,
    "Discriminator": "Distributor",
    "id": "Distributor|1",
    "ShippingCenters": [
        {
            "City": "Phoenix",
            "Street": "500 S 48th Street"
        }
    ],
    "_rid": "JBwtAN8oNYEBAAAAAAAAAA==",
    "_self": "dbs/JBwtAA==/colls/JBwtAN8oNYE=/docs/JBwtAN8oNYEBAAAAAAAAAA==/",
    "_etag": "\"00000000-0000-0000-9377-d7a1ae7c01d5\"",
    "_attachments": "attachments/",
    "_ts": 1572917100
}
```

## Optimistic concurrency with eTags

To configure an entity type to use [optimistic concurrency](xref:core/saving/concurrency) call <xref:Microsoft.EntityFrameworkCore.CosmosEntityTypeBuilderExtensions.UseETagConcurrency%2A>. This call will create an `_etag` property in [shadow state](xref:core/modeling/shadow-properties) and set it as the concurrency token.

[!code-csharp[Main](../../../../samples/core/Cosmos/ModelBuilding/OrderContext.cs?name=ETag)]

To make it easier to resolve concurrency errors you can map the eTag to a CLR property using <xref:Microsoft.EntityFrameworkCore.CosmosPropertyBuilderExtensions.IsETagConcurrency%2A>.

[!code-csharp[Main](../../../../samples/core/Cosmos/ModelBuilding/OrderContext.cs?name=ETagProperty)]
