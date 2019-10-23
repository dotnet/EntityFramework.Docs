---
title: Azure Cosmos DB Provider - Working with Unstructured Data - EF Core
author: AndriySvyryd
ms.author: ansvyryd
ms.date: 09/12/2019
ms.assetid: b47d41b6-984f-419a-ab10-2ed3b95e3919
uid: core/providers/cosmos/unstructured-data
---
# Working with Unstructured Data in EF Core Azure Cosmos DB Provider

EF Core was designed to make it easy to work with data that follows a schema defined in the model. However one of the strengths of Azure Cosmos DB is the flexibility in the shape of the data stored.

## Accessing the raw JSON

It is possible to access the properties that are not tracked by EF Core through a special property in [shadow-state](../../modeling/shadow-properties.md) named `"__jObject"` that contains a `JObject` representing the data recieved from the store and data that will be stored:

[!code-csharp[Unmapped](../../../../samples/core/Cosmos/UnstructuredData/Sample.cs?highlight=21-23&name=Unmapped)]

``` json
{
    "Id": 1,
    "Discriminator": "Order",
    "TrackingNumber": null,
    "id": "Order|1",
    "Address": {
        "ShipsToCity": "London",
        "Discriminator": "StreetAddress",
        "ShipsToStreet": "221 B Baker St"
    },
    "_rid": "eLMaAK8TzkIBAAAAAAAAAA==",
    "_self": "dbs/eLMaAA==/colls/eLMaAK8TzkI=/docs/eLMaAK8TzkIBAAAAAAAAAA==/",
    "_etag": "\"00000000-0000-0000-683e-0a12bf8d01d5\"",
    "_attachments": "attachments/",
    "BillingAddress": "Clarence House",
    "_ts": 1568164374
}
```

> [!WARNING]
> The `"__jObject"` property is part of the EF Core infrastructure and should only be used as a last resort as it is likely to have different behavior in future releases.

> [!NOTE]
> Changes to the entity will override the values stored in `"__jObject"` during `SaveChanges`.

## Using CosmosClient

To decouple completely from EF Core get the `CosmosClient` object that is [part of the Azure Cosmos DB SDK](https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-get-started) from `DbContext`:

[!code-csharp[CosmosClient](../../../../samples/core/Cosmos/UnstructuredData/Sample.cs?highlight=3&name=CosmosClient)]

## Missing property values

In the previous example we removed the `"TrackingNumber"` property from the order. Because of how indexing works in Cosmos DB, queries that reference the missing property somewhere else than in the projection could return unexpected results. For example:

[!code-csharp[MissingProperties](../../../../samples/core/Cosmos/UnstructuredData/Sample.cs?name=MissingProperties)]

The sorted query actually returns no results. This means that one should take care to always populate properties mapped by EF Core when working with the store directly.

> [!NOTE]
> This behavior might change in future versions of Cosmos. For instance, currently if the indexing policy defines the composite index {Id/? ASC, TrackingNumber/? ASC)}, then a query the has 'ORDER BY c.Id ASC, c.Discriminator ASC' __would__ return items that are missing the `"TrackingNumber"` property.
