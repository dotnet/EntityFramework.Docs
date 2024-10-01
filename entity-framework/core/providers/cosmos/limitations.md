---
title: Azure Cosmos DB Provider - Limitations - EF Core
description: Limitations of the Entity Framework Core Azure Cosmos DB provider as compared to other providers
author: AndriySvyryd
ms.date: 09/26/2024
uid: core/providers/cosmos/limitations
---
# EF Core Azure Cosmos DB Provider Limitations

The Azure Cosmos DB database provider targets the Azure Cosmos DB NoSQL store, which is a [document database](https://en.wikipedia.org/wiki/Document-oriented_database). Most EF Core providers target [relational databases](https://en.wikipedia.org/wiki/Relational_database). Document databases and relational databases behave in fundamentally different ways. EF Core does not attempt to hide these differences; rather EF Core provides common patterns that can be used successfully across both kinds of database, together with features tailored to a particular provider that follow best practices for a given type of database. If a feature of EF Core is a pit-of-failure for a certain kind of database, then typically the database provider will not implement that feature, and instead help steer uses towards a [pit-of-success](/archive/blogs/brada/the-pit-of-success) approach.

Common EF Core patterns that either do not apply, or are a pit-of-failure, when using a document database include:

- Schema migration is not supported, since there is no defined schema for the documents. However, there could be other mechanisms for dealing with evolving data shapes that do make sense with Azure Cosmos DB NoSQL, For example, [Schema versioning pattern with Azure Cosmos DB](https://github.com/dotnet/efcore/issues/23753), and [Azure Cosmos DB data migration](https://github.com/dotnet/efcore/issues/11099).
- Reverse-engineering (scaffolding) a model from an existing database is not supported. Again, this is not supported because there is no defined database schema to scaffold from. However, see [Use shape of documents in the Azure Cosmos DB database to scaffold a schema](https://github.com/dotnet/efcore/issues/30290).
- Schema concepts defined on the EF model, like indexes and constraints, are ignored when using a document database, since there is no schema. Note that Azure Cosmos DB NoSQL performs [automatic indexing of documents](/azure/cosmos-db/index-overview).
- Loading graphs of related entities from different documents is not supported. Document databases are not designed to perform joins across many documents; doing so would be very inefficient. Instead, it is more common to denormalize data so that everything needed is in one, or a small number, of documents. However, there are some forms of cross-document relationships that could be handled--see [Limited Include support for Azure Cosmos DB](https://github.com/dotnet/efcore/issues/16920#issuecomment-989721078).

> [!WARNING]
> The Azure Cosmos DB SDK, which the EF provider uses, does not support synchronous I/O. As a result, synchronous EF APIs such as `ToList` or `SaveChanges` throw in version 9.0 and above; always use asynchronous
> methods when using EF.
> Previous versions of EF supported the synchronous APIs by calling `.Wait()` on the returned `Task`; this is known as "sync over async", and is a highly discouraged technique that can lead to deadlocks. See the EF 9.0 [breaking change note](xref:core/what-is-new/ef-core-9.0/breaking-changes#cosmos-nosync) for more information.

Beyond the differences in relational and document databases, and limitations in the SDK, the EF Core provider for Azure Cosmos DB NoSQL does not include everything that _could_ be implemented using the combination of EF Core and the Azure Cosmos DB SDK. Potential enhancements in this area are tracked by [issues in the EF Core GitHub repo marked with the label `area-cosmos`](https://github.com/dotnet/efcore/issues?q=is%3Aopen+is%3Aissue+label%3Aarea-cosmos+sort%3Areactions-%2B1-desc+label%3Atype-enhancement) The best way to indicate the importance of an issue is to vote (👍) for it. This data will then feed into the [planning process](xref:core/what-is-new/release-planning) for the next release.
