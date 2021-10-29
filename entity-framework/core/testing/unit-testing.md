---
title: Unit testing with EF Core
description: Approaches and techniques to doing unit testing with applications using Entity Framework Core
author: roji
ms.date: 11/07/2021
uid: core/testing/unit-testing
---
# Unit Testing

In the context of the EF Core applications, unit tests are automated tests which do not involve the database system against which the application runs in production. There are various unit testing approaches which swap the database with a test double, as well as integration tests which do involve the production database system. It's recommend to first read the [testing overview page](xref:core/testing/index) for a comparison of the different testing patterns and guidelines on how to choose the best one for your scenario. This page shows how to use each unit testing approach, and provides code samples for them.

## Repository pattern

The repository pattern is the recommended technique for writing unit tests for EF Core applications - for more background, see [the corresponding section in the overview page](xref:core/testing/index#repository-pattern). The first step of implementing the repository pattern is to extract out your EF Core LINQ queries to a separate layer, which we'll later stub or mock. Here's an example of a repository interface for our blogging system:

[!code-csharp[Main](../../../samples/core/Testing/BusinessLogic/IBloggingRepository.cs?name=IBloggingRepository)]

... and here's a partial sample implementation for production use:

[!code-csharp[Main](../../../samples/core/Testing/BusinessLogic/BloggingRepository.cs?name=BloggingRepository)]

There's not much to it: the repository simply wraps an EF Core context, and exposes methods which execute the database queries and updates on it. A key point to note is that our `GetAllBlogs` method returns `IEnumerable<Blog>`, and not `IQueryable<Blog>`. Returning the latter would mean that query operators can still be composed over the result, requiring EF Core to still involved in translating the query; this would defeat the purpose of having a repository in the first place. `IEnumerable<Blog>` allows us to easily stub or mock what the repository returns.

For an ASP.NET application, we need to register the repository as a service in dependency injection by adding the following to the application's `ConfigureServices`:

[!code-csharp[Main](../../../samples/core/Testing/BloggingWebApi/Startup.cs?name=RegisterRepositoryInDI)]

Finally, our ASP.NET controllers get injected with the repository service instead of the EF Core context, and execute methods on it:

[!code-csharp[Main](../../../samples/core/Testing/BloggingWebApi/Controllers/BloggingControllerWithRepository.cs?name=BloggingControllerWithRepository&highlight=8)]

At this point, your application is architected according to the repository pattern: the only point of contact with the data access layer - EF Core - is now the repository layer, which acts as a mediator between application code and actual database queries. Unit tests can now be written simply by stubbing out the repository, or by mocking it with your favorite mocking library. Here's an example of a mock-based unit test using the popular [Moq](https://github.com/Moq/moq4) library:

[!code-csharp[Main](../../../samples/core/Testing/UnitTests/RepositoryBloggingControllerTest.cs?name=GetBlog)]

The full sample code can be viewed [here](https://github.com/dotnet/EntityFramework.Docs/blob/main/samples/core/Testing/UnitTests/RepositoryBloggingControllerTest.cs).

## SQLite in-memory

SQLite can easily be configured as the EF Core provider for your test suite instead of your production database system (e.g. SQL Server); consult the [SQLite provider docs](xref:core/providers/sqlite/index) for details. However, it's usually a good idea to use SQLite's [in-memory database](https://sqlite.org/inmemorydb.html) feature when unit testing, since it provides easy isolation between tests, and does not require dealing with actual SQLite files.

To use in-memory SQLite, it's important to understand that a new database is created whenever a low-level connection is opened, and that it's deleted what that connection is closed. In normal usage, EF Core's `DbContext` opens and closes database connections as needed - every time a query is executed - to avoid keeping connection for unnecessarily long times. However, with in-memory SQLite this would lead to resetting the database every time; so as a workaround, we open the connection before passing it to EF Core, and arrange for it to be closed only when the test completes:

[!code-csharp[Main](../../../samples/core/Testing/UnitTests/SqliteInMemoryBloggingControllerTest.cs?name=ConstructorAndDispose)]

Tests can now call `CreateContext`, which returns a context using the connection we set up in the constructor, ensuring we have a clean database with the seeded data.

The full sample code can be viewed [here](https://github.com/dotnet/EntityFramework.Docs/blob/main/samples/core/Testing/UnitTests/SqliteInMemoryBloggingControllerTest.cs).

## InMemory provider

As discussed in the [testing overview page](xref:core/testing/index#inmemory-as-a-database-fake), using the InMemory provider for testing is strongly discouraged; [consider using SQLite instead](#sqlite-in-memory), or [implementing the repository pattern](#repository-pattern). If you've decided to use InMemory, here is a typical test class constructor that sets up and seeds a new in-memory database before each test:

[!code-csharp[Main](../../../samples/core/Testing/UnitTests/InMemoryBloggingControllerTest.cs?name=Constructor)]

InMemory databases are identified by a simple, string name, and it's possible to connect to the same database several times by providing the same name (this is why the sample above must call `EnsureDeleted` before each test). However, note that in-memory databases are rooted in the context's internal service provider; while in most cases contexts share the same service provider, configuring contexts with different options may trigger the use of a new internal service provider. When that's the case, explicitly pass the same instance of <xref:Microsoft.EntityFrameworkCore.Storage.InMemoryDatabaseRoot> to `UseInMemoryDatabase` for all contexts which should share in-memory databases (this is typically done by having a static `InMemoryDatabaseRoot` field).

Note that by default, if a transaction is started, the InMemory provider will throw an exception, since transactions aren't supported. You may wish to have transactions silently ignored instead, by configuring EF Core to ignore `InMemoryEventId.TransactionIgnoredWarning` as in the above sample. However, if your code actually relies on transactional semantics - e.g. depends on rollback to be implemented - your test won't work.

The full sample code can be viewed [here](https://github.com/dotnet/EntityFramework.Docs/blob/main/samples/core/Testing/UnitTests/InMemoryBloggingControllerTest.cs).
