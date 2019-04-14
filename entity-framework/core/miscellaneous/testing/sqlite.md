---
title: Testing with SQLite - EF Core
author: rowanmiller
ms.date: 10/27/2016
ms.assetid: 7a2b75e2-1875-4487-9877-feff0651b5a6
uid: core/miscellaneous/testing/sqlite
---

# Testing with SQLite

SQLite has an in-memory mode that allows you to use SQLite to write tests against a relational database, without the overhead of actual database operations.

> [!TIP]  
> You can view this article's [sample](https://github.com/aspnet/EntityFramework.Docs/tree/master/samples/core/Miscellaneous/Testing) on GitHub

## Example testing scenario

Consider the following service that allows application code to perform some operations related to blogs. Internally it uses a `DbContext` that connects to a SQL Server database. It would be useful to swap this context to connect to an in-memory SQLite database so that we can write efficient tests for this service without having to modify the code, or do a lot of work to create a test double of the context.

[!code-csharp[Main](../../../../samples/core/Miscellaneous/Testing/BusinessLogic/BlogService.cs)]

## Get your context ready

### Avoid configuring two database providers

In your tests you are going to externally configure the context to use the InMemory provider. If you are configuring a database provider by overriding `OnConfiguring` in your context, then you need to add some conditional code to ensure that you only configure the database provider if one has not already been configured.

> [!TIP]  
> If you are using ASP.NET Core, then you should not need this code since your database provider is configured outside of the context (in Startup.cs).

[!code-csharp[Main](../../../../samples/core/Miscellaneous/Testing/BusinessLogic/BloggingContext.cs#OnConfiguring)]

### Add a constructor for testing

The simplest way to enable testing against a different database is to modify your context to expose a constructor that accepts a `DbContextOptions<TContext>`.

[!code-csharp[Main](../../../../samples/core/Miscellaneous/Testing/BusinessLogic/BloggingContext.cs#Constructors)]

> [!TIP]  
> `DbContextOptions<TContext>` tells the context all of its settings, such as which database to connect to. This is the same object that is built by running the OnConfiguring method in your context.

## Writing tests

The key to testing with this provider is the ability to tell the context to use SQLite, and control the scope of the in-memory database. The scope of the database is controlled by opening and closing the connection. The database is scoped to the duration that the connection is open. Typically you want a clean database for each test method.

>[!TIP]
> To use `SqliteConnection()` and the `.UseSqlite()` extension method, reference the NuGet package [Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/).

[!code-csharp[Main](../../../../samples/core/Miscellaneous/Testing/TestProject/SQLite/BlogServiceTests.cs)]
