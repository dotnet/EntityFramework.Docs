---
title: Modeling - Azure Cosmos DB Provider - EF Core
description: Configuring the model with the Azure Cosmos DB EF Core Provider
author: roji
ms.date: 09/19/2024
uid: core/providers/cosmos/modeling
---
# Configuring the model with the EF Core Azure Cosmos DB Provider

## Containers and entity types

In Azure Cosmos DB, JSON documents are stored in containers. Unlike tables in relational databases, Azure Cosmos DB containers can contain documents with different shapes - a container does not impose a uniform schema on its documents. However, various configuration options are defined at the container level, and therefore affect all documents contained within it. See the [Azure Cosmos DB documentation on containers](/azure/cosmos-db/resource-model) for more information.

By default, EF maps all entity types to the same container; this is usually a good default in terms of performance and pricing. The default container is named after the .NET context type (`OrderContext` in this case). To change the default container name, use <xref:Microsoft.EntityFrameworkCore.CosmosModelBuilderExtensions.HasDefaultContainer*>:

```csharp
modelBuilder.HasDefaultContainer("Store");
```

To map an entity type to a different container use <xref:Microsoft.EntityFrameworkCore.CosmosEntityTypeBuilderExtensions.ToContainer*>:

```csharp
modelBuilder.Entity<Order>().ToContainer("Orders");
```

Before mapping entity types to different containers, make sure you understand the potential performance and pricing implications (e.g. with regards to dedicated and shared throughput); [see the Azure Cosmos DB documentation to learn more](/azure/cosmos-db/resource-model).

## IDs and keys

Azure Cosmos DB requires all documents to have an `id` JSON property which uniquely identifies them. Like other EF providers, the EF Azure Cosmos DB provider will attempt to find a property named `Id` or `<type name>Id`, and configure that property as the key of your entity type, mapping it to the `id` JSON property. You can configure any property to be the key property by using <xref:Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder`1.HasKey*>; see [the general EF documentation on keys](xref:core/modeling/keys) for more information.

Developers coming to Azure Cosmos DB from other databases sometimes expect the key (`Id`) property to be generated automatically. For example, on SQL Server, EF configures numeric key properties to be IDENTITY columns, where auto-incrementing values are generated in the database. In contrast, Azure Cosmos DB does not support automatic generation of properties, and so key properties must be explicitly set. Inserting an entity type with an unset key property will simply insert the CLR default value for that property (e.g. 0 for `int`), and a second insert will fail; EF issues a warning if you attempt to do this.

If you'd like to have a GUID as your key property, you can configure EF to generate unique, random values at the client:

```csharp
modelBuilder.Entity<Session>().Property(b => b.Id).HasValueGenerator<GuidValueGenerator>();
```

## Partition keys

Azure Cosmos DB uses partitioning to achieve horizontal scaling; proper modeling and careful selection of the partition key is vital for achieving good performance and keeping costs down. It's highly recommended to read [the Azure Cosmos DB documentation on partitioning](/azure/cosmos-db/partition-data) and to plan your partitioning strategy in advance.

To configure the partition key with EF, call [HasPartitionKey](/dotnet/api/Microsoft.EntityFrameworkCore.CosmosEntityTypeBuilderExtensions.HasPartitionKey), passing it a regular property on your entity type:

```csharp
modelBuilder.Entity<Order>().HasPartitionKey(o => o.PartitionKey);
```

Any property can be made into a partition key as long as it is [converted to string](xref:core/modeling/value-conversions). Once configured, the partition key property should always have a non-null value; trying to insert a new entity type with an unset partition key property will result in an error.

Note that Azure Cosmos DB allows two documents with the same `id` property to exist in a container, as long as they're in different partitions; this means that in order to uniquely identify a document within a container, both the `id` and the partition key properties must all be provided. Because of this, EF's internal notion of the entity primary key contains both of these elements by convention, unlike e.g. relational databases where there is no partition key concept. This means e.g. that [`FindAsync`](xref:core/change-tracking/entity-entries#find-and-findasync) requires both key and partition key properties ([see further docs](xref:core/providers/cosmos/querying#findasync)), and a query must specify these in its `Where` clause to benefit from efficient and cost-effective [`point reads`](xref:core/providers/cosmos/querying#point-reads).

Note that the partition key is defined at the container level. This notably means that it's not possible for multiple entity types in the same container to have different partition key properties. If you need to define different partition keys, map the relevant entity types to different containers.

### Hierarchical partition keys

Azure Cosmos DB also supports _hierarchical_ partition keys to optimize data distribution even further; [see the documentation for more details](/azure/cosmos-db/hierarchical-partition-keys). EF 9.0 added support for hierarchical partition keys; to configure these, simply pass up to 3 properties to [HasPartitionKey](/dotnet/api/Microsoft.EntityFrameworkCore.CosmosEntityTypeBuilderExtensions.HasPartitionKey):

```csharp
modelBuilder.Entity<Order>().HasPartitionKey(o => new { e.TenantId, e.UserId, e.SessionId });
```

With such a hierarchical partition key, queries can be easily sent only to the a relevant subset of sub-partitions. For example, if you query for the Orders of a specific tenant, those queries will only be executed against the sub-partitions for that tenant.

If you don't configure a partition key with EF, a warning will be logged at startup; EF Core will create containers with the partition key set to `__partitionKey`, and won't supply any value for it when inserting items. When no partition key is set, your container will be limited to 20 GB of data, which is the maximum storage for a single [logical partition](/azure/cosmos-db/partitioning-overview). While this can work for small dev/ test applications, it is highly discouraged to deploy a production application without a well-configured partition key strategy.

Once your partition key properties are properly configured, you can provide values for them in queries; see [Querying with partition keys](xref:core/providers/cosmos/querying#partition-keys) for more information.

## Discriminators

Since multiple entity types may be mapped to the same container, EF Core always adds a `$type` discriminator property to all JSON documents you save (this property was called `Discriminator` before EF 9.0); this allows EF to recognize documents being loaded from the database, and materialize the right .NET type. Developers coming from relational databases may be familiar with discriminators in the context of [table-per-hierarchy inheritance (TPH)](xref:core/modeling/inheritance#table-per-hierarchy-and-discriminator-configuration); in Azure Cosmos DB, discriminators are used not just in inheritance mapping scenarios, but also because the same container can contain completely different document types.

The discriminator property name and values can be configured with the standard EF APIs, [see these docs for more information](xref:core/modeling/inheritance). If you're mapping a single entity type to a container, are confident that you'll never be mapping another one, and would like to get rid of the discriminator property, call [HasNoDiscriminator](/dotnet/api/Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder.HasNoDiscriminator):

```csharp
modelBuilder.Entity<Order>().HasNoDiscriminator();
```

Since the same container can contain different entity types, and the JSON `id` property must be unique within a container partition, you cannot have the same `id` value for entities of different types in the same container partition. Compare this to relational databases, where each entity type is mapped to a different table, and therefore has its own, separate key space. It is therefore your responsibility to ensure the `id` uniqueness of documents you insert into a container. If you need to have different entity types with the same primary key values, you can instruct EF to automatically insert the discriminator into the `id` property as follows:

```csharp
modelBuilder.Entity<Session>().HasDiscriminatorInJsonId();
```

While this may make it easier to work with `id` values, it may make it harder to interoperate with external applications working with your documents, as they now must be aware of EF's concatenated `id` format, as well as the discriminator values, which are by default derived from your .NET types. Note that this was the default behavior prior to EF 9.0.

An additional option is to instruct EF to insert only the _root discriminator_, which is the discriminator of the root entity type of the hierarchy, into the `id` property:

```csharp
modelBuilder.Entity<Session>().HasRootDiscriminatorInJsonId();
```

This is similar, but allows EF to use efficient [point reads](xref:core/providers/cosmos/querying#point-reads) in more scenarios. If you need to insert a discriminator into the `id` property, consider inserting the root discriminator for better performance.

## Provisioned throughput

If you use EF Core to create the Azure Cosmos DB database or containers you can configure [provisioned throughput](/azure/cosmos-db/set-throughput) for the database by calling <xref:Microsoft.EntityFrameworkCore.CosmosModelBuilderExtensions.HasAutoscaleThroughput*?displayProperty=nameWithType> or <xref:Microsoft.EntityFrameworkCore.CosmosModelBuilderExtensions.HasManualThroughput*?displayProperty=nameWithType>. For example:

<!--
modelBuilder.HasManualThroughput(2000);
modelBuilder.HasAutoscaleThroughput(4000);
-->
[!code-csharp[ModelThroughput](../../../../samples/core/Miscellaneous/NewInEFCore6.Cosmos/CosmosModelConfigurationSample.cs?name=ModelThroughput)]

To configure provisioned throughput for a container call <xref:Microsoft.EntityFrameworkCore.CosmosEntityTypeBuilderExtensions.HasAutoscaleThroughput*?displayProperty=nameWithType> or <xref:Microsoft.EntityFrameworkCore.CosmosEntityTypeBuilderExtensions.HasManualThroughput*?displayProperty=nameWithType>. For example:

<!--
modelBuilder.Entity<Family>(
    entityTypeBuilder =>
    {
        entityTypeBuilder.HasManualThroughput(5000);
        entityTypeBuilder.HasAutoscaleThroughput(3000);
    });
-->
[!code-csharp[EntityTypeThroughput](../../../../samples/core/Miscellaneous/NewInEFCore6.Cosmos/CosmosModelConfigurationSample.cs?name=EntityTypeThroughput)]

## Time-to-live

Entity types in the Azure Cosmos DB model can be configured with a default time-to-live. For example:

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
> Related entity types are configured as owned by default. To prevent this for a specific entity type call <xref:Microsoft.EntityFrameworkCore.ModelBuilder.Entity*?displayProperty=nameWithType>.

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

Collections of supported primitive types, such as `string` and `int`, are discovered and mapped automatically. Supported collections are all types that implement <xref:System.Collections.Generic.IReadOnlyList`1> or <xref:System.Collections.Generic.IReadOnlyDictionary`2>. For example, consider this entity type:

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

The `IList` and the `IDictionary` can be populated and persisted to the database:

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

* Only dictionaries with string keys are supported.
* Support for querying into primitive collections was added in EF Core 9.0.

## Optimistic concurrency with eTags

To configure an entity type to use [optimistic concurrency](xref:core/saving/concurrency) call <xref:Microsoft.EntityFrameworkCore.CosmosEntityTypeBuilderExtensions.UseETagConcurrency*>. This call will create an `_etag` property in [shadow state](xref:core/modeling/shadow-properties) and set it as the concurrency token.

[!code-csharp[Main](../../../../samples/core/Cosmos/ModelBuilding/OrderContext.cs?name=ETag)]

To make it easier to resolve concurrency errors you can map the eTag to a CLR property using <xref:Microsoft.EntityFrameworkCore.CosmosPropertyBuilderExtensions.IsETagConcurrency*>.

[!code-csharp[Main](../../../../samples/core/Cosmos/ModelBuilding/OrderContext.cs?name=ETagProperty)]
