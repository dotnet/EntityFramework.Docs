---
title: Azure Cosmos DB Provider - EF Core
description: Documentation for the database provider that allows Entity Framework Core to be used with Azure Cosmos DB.
author: AndriySvyryd
ms.date: 02/12/2023
uid: core/providers/cosmos/index
---
# EF Core Azure Cosmos DB Provider

> [!WARNING]
> Extensive work has gone into the Azure Cosmos DB provider in 9.0. In order to improve the provider, a number of high-impact breaking changes had to be made; if you are upgrading an existing application, please read the [breaking changes section](xref:core/what-is-new/ef-core-9.0/breaking-changes#cosmos-breaking-changes) carefully.

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
>
> Azure Cosmos DB SDK does not support RBAC for management plane operations in Azure Cosmos DB. Use Azure Management API instead of EnsureCreatedAsync with RBAC.

## Connecting and authenticating

The Azure Cosmos DB provider for EF Core has multiple overloads of the [UseCosmos](/dotnet/api/Microsoft.EntityFrameworkCore.CosmosDbContextOptionsExtensions.UseCosmos) method. These overloads support the different ways that a connection can be made to the database, and the different ways of ensuring that the connection is secure.

> [!IMPORTANT]
> Make sure to understand [_Secure access to data in Azure Cosmos DB_](/azure/cosmos-db/secure-access-to-data) to understand the security implications and best practices for using each overload of the `UseCosmos` method.
> Generally, RBAC with token credentials is the recommended access-control mechanism.

| Connection Mechanism       | UseCosmos Overload                                                     | More information                                                                          |
|----------------------------|------------------------------------------------------------------------|----------------------------------------------------------------------------------------------|
| Account endpoint and key   | `UseCosmos<DbContext>(accountEndpoint, accountKey, databaseName)`      | [Primary/secondary keys](/azure/cosmos-db/secure-access-to-data#primary-keys)                |
| Account endpoint and token | `UseCosmos<DbContext>(accountEndpoint, tokenCredential, databaseName)` | [RBAC and Resource tokens](/azure/cosmos-db/secure-access-to-data#role-based-access-control) |
| Connection string          | `UseCosmos<DbContext>(connectionString, databaseName)`                 | [Work with account keys and connection strings](/azure/cosmos-db/scripts/cli/common/keys)    |

## Azure Cosmos DB options

It is also possible to configure the Azure Cosmos DB provider with a single connection string and to specify other options to customize the connection:

[!code-csharp[Configuration](../../../../samples/core/Cosmos/ModelBuilding/OptionsContext.cs?name=Configuration)]

The code above shows some possible options - these are not intended to be used at the same time. See the [Azure Cosmos DB Options documentation](/dotnet/api/microsoft.azure.cosmos.cosmosclientoptions) for a detailed description of the effect of each option mentioned above.
