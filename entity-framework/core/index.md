---
title: Overview of Entity Framework Core - EF Core
description: General introductory overview of Entity Framework Core
author: rowanmiller
ms.date: 10/27/2016
uid: core/index
---

# Entity Framework Core

Entity Framework (EF) Core is a lightweight, extensible, [open source](https://github.com/aspnet/EntityFrameworkCore) and cross-platform version of the popular Entity Framework data access technology.

EF Core can serve as an object-relational mapper (O/RM), which:

* Enables .NET developers to work with a database using .NET objects.
* Eliminates the need for most of the data-access code that typically needs to be written.

EF Core supports many database engines, see [Database Providers](xref:core/providers/index) for details.

## The Model

With EF Core, data access is performed using a model. A model is made up of entity classes and a context object that represents a session with the database. The context object allows querying and saving data. For more information, see [Creating a Model](xref:core/modeling/index).

EF supports the following for model development:

* Generating a model from an existing database.
* Hand code a model to match the database.
* Use [EF Migrations](xref:core/managing-schemas/migrations/index) to create a database from a model. Migrations allow evolving the database as the model changes.

[!code-csharp[Main](../../samples/core/Intro/Model.cs)]

## Querying

Instances of your entity classes are retrieved from the database using Language Integrated Query (LINQ). For more information, see [Querying Data](xref:core/querying/index).

[!code-csharp[Main](../../samples/core/Intro/Program.cs#Querying)]

## Saving Data

Data is created, deleted, and modified in the database using instances of your entity classes. See [Saving Data](xref:core/saving/index) to learn more.

[!code-csharp[Main](../../samples/core/Intro/Program.cs#SavingData)]

## EF ORM considerations

While EF Core is good at abstracting many programming details, there are some  aspects of EF Core that need to be handled for production apps to avoid common pitfalls:

 - Intermediate-level knowledge or higher of the underlying database server is essential to debug and understand problems, performance issues, etc.
- Functional and integration testing:  It's important to replicate the production environment as close as possible to:
  - Find issues in the app.
  - Catch breaking changes when upgrading EF Core and other dependencies. For example, adding or upgrading frameworks like ASP.NET Core, Application Insights, Serilog, or Automapper. These dependencies can affect EF Core in unexpected ways.
- Performance and stress testing with representative loads. The naive usage of some features doesn't scale well. For example, multiple collections Includes, heavy use of lazy loading, conditional queries on non-indexed columns, massive updates, and inserts with store-generated values, lack of concurrency handling, large models, inadequate cache policy.
- Security: For example, handling of connection strings, database permissions for non-deployment operation, input validation for raw SQL, encryption for sensitive data.
- Make sure logging and diagnostics are sufficient and usable. For example, appropriate logging configuration, query tags, and Application Insights.
- Error recovery. Prepare contingencies for common failure scenarios such as version rollback, fallback servers, scale-out and load balancing, DoS mitigation, and data backups.
- Application deployment and migration. <!-- review I need some content to replace the following link. Links just provided as a handy ref. --> See https://github.com/dotnet/EntityFramework.Docs/issues/1879 and https://github.com/dotnet/EntityFramework.Docs/issues/814.
- Detailed examination and testing of generated migrations. Migrations should be thoroughly tested before being applied to production data. The shape of the schema and the column types cannot be easily changed once the tables contain production data. For example, on SqlServer, `nvarchar(max)` and `decimal(18, 2)` are rarely the best types for columns mapped to string and decimal properties.  <!-- review I need some content to replace the following link. Links just provided as a handy ref. --> See https://github.com/dotnet/efcore/issues/20159

## Next steps

For introductory tutorials, see [Getting Started with Entity Framework Core](xref:core/get-started/index).
