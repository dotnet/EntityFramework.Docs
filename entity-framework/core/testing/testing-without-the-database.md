---
title: Testing without your Production Database System - EF Core
description: Techniques for testing EF Core applications without involving  your production database system
author: roji
ms.date: 01/24/2022
uid: core/testing/testing-without-the-database
---
# Testing without your production database system

In this page, we discuss techniques for writing automated tests which do not involve the database system against which the application runs in production, by swapping your database with a [test double](https://en.wikipedia.org/wiki/Test_double). There are various types of test doubles and approaches for doing this, and it's recommended to thoroughly read [Choosing a testing strategy](xref:core/testing/choosing-a-testing-strategy) to fully understand the different options. Finally, it's also possible to test against your production database system; this is covered in [Testing against your production database system](xref:core/testing/testing-with-the-database).

> [!TIP]
> This page shows [xUnit](https://xunit.net/) techniques, but similar concepts exist in other testing frameworks, including [NUnit](https://nunit.org/).

## Repository pattern

If you've decided to write tests without involving your production database system, then the recommended technique for doing so is the repository pattern; for more background on this, see [this section](xref:core/testing/choosing-a-testing-strategy#repository-pattern). The first step of implementing the repository pattern is to extract out your EF Core LINQ queries to a separate layer, which we'll later stub or mock. Here's an example of a repository interface for our blogging system:

[!code-csharp[Main](../../../samples/core/Testing/BusinessLogic/IBloggingRepository.cs?name=IBloggingRepository)]

... and here's a partial sample implementation for production use:

```csharp
public class BloggingRepository : IBloggingRepository
{
    private readonly BloggingContext _context;

    public BloggingRepository(BloggingContext context)
        => _context = context;

    public async Task<Blog> GetBlogByNameAsync(string name)
        => await _context.Blogs.FirstOrDefaultAsync(b => b.Name == name);

    // Other code...
}
```

There's not much to it: the repository simply wraps an EF Core context, and exposes methods which execute the database queries and updates on it. A key point to note is that our `GetAllBlogs` method returns `IAsyncEnumerable<Blog>` (or `IEnumerable<Blog>`), and not `IQueryable<Blog>`. Returning the latter would mean that query operators can still be composed over the result, requiring that EF Core still be involved in translating the query; this would defeat the purpose of having a repository in the first place. `IAsyncEnumerable<Blog>` allows us to easily stub or mock what the repository returns.

For an ASP.NET Core application, we need to register the repository as a service in dependency injection by adding the following to the application's `ConfigureServices`:

[!code-csharp[Main](../../../samples/core/Testing/BloggingWebApi/Startup.cs?name=RegisterRepositoryInDI)]

Finally, our controllers get injected with the repository service instead of the EF Core context, and execute methods on it:

[!code-csharp[Main](../../../samples/core/Testing/BloggingWebApi/Controllers/BloggingControllerWithRepository.cs?name=BloggingControllerWithRepository&highlight=8)]

At this point, your application is architected according to the repository pattern: the only point of contact with the data access layer - EF Core - is now via the repository layer, which acts as a mediator between application code and actual database queries. Tests can now be written simply by stubbing out the repository, or by mocking it with your favorite mocking library. Here's an example of a mock-based test using the popular [Moq](https://github.com/Moq/moq4) library:

[!code-csharp[Main](../../../samples/core/Testing/TestingWithoutTheDatabase/RepositoryBloggingControllerTest.cs?name=GetBlog)]

The full sample code can be viewed [here](https://github.com/dotnet/EntityFramework.Docs/blob/main/samples/core/Testing/TestingWithoutTheDatabase/RepositoryBloggingControllerTest.cs).

## SQLite in-memory

SQLite can easily be configured as the EF Core provider for your test suite instead of your production database system (e.g. SQL Server); consult the [SQLite provider docs](xref:core/providers/sqlite/index) for details. However, it's usually a good idea to use SQLite's [in-memory database](https://sqlite.org/inmemorydb.html) feature when testing, since it provides easy isolation between tests, and does not require dealing with actual SQLite files.

To use in-memory SQLite, it's important to understand that a new database is created whenever a low-level connection is opened, and that it's deleted when that connection is closed. In normal usage, EF Core's `DbContext` opens and closes database connections as needed - every time a query is executed - to avoid keeping connection for unnecessarily long times. However, with in-memory SQLite this would lead to resetting the database every time; so as a workaround, we open the connection before passing it to EF Core, and arrange for it to be closed only when the test completes:

[!code-csharp[Main](../../../samples/core/Testing/TestingWithoutTheDatabase/SqliteInMemoryBloggingControllerTest.cs?name=ConstructorAndDispose)]

Tests can now call `CreateContext`, which returns a context using the connection we set up in the constructor, ensuring we have a clean database with the seeded data.

The full sample code for SQLite in-memory testing can be viewed [here](https://github.com/dotnet/EntityFramework.Docs/blob/main/samples/core/Testing/TestingWithoutTheDatabase/SqliteInMemoryBloggingControllerTest.cs).

## In-memory provider

As discussed in the [testing overview page](xref:core/testing/choosing-a-testing-strategy#inmemory-as-a-database-fake), using the in-memory provider for testing is strongly discouraged; [consider using SQLite instead](#sqlite-in-memory), or [implementing the repository pattern](#repository-pattern). If you've decided to use in-memory, here is a typical test class constructor that sets up and seeds a new in-memory database before each test:

[!code-csharp[Main](../../../samples/core/Testing/TestingWithoutTheDatabase/InMemoryBloggingControllerTest.cs?name=Constructor)]

The full sample code for in-memory testing can be viewed [here](https://github.com/dotnet/EntityFramework.Docs/blob/main/samples/core/Testing/TestingWithoutTheDatabase/InMemoryBloggingControllerTest.cs).

### In-memory database naming

In-memory databases are identified by a simple, string name, and it's possible to connect to the same database several times by providing the same name (this is why the sample above must call `EnsureDeleted` before each test). However, note that in-memory databases are rooted in the context's internal service provider; while in most cases contexts share the same service provider, configuring contexts with different options may trigger the use of a new internal service provider. When that's the case, explicitly pass the same instance of <xref:Microsoft.EntityFrameworkCore.Storage.InMemoryDatabaseRoot> to `UseInMemoryDatabase` for all contexts which should share in-memory databases (this is typically done by having a static `InMemoryDatabaseRoot` field).

### Transactions

Note that by default, if a transaction is started, the in-memory provider will throw an exception since transactions aren't supported. You may wish to have transactions silently ignored instead, by configuring EF Core to ignore `InMemoryEventId.TransactionIgnoredWarning` as in the above sample. However, if your code actually relies on transactional semantics - e.g. depends on rollback actually rolling back changes - your test won't work.

### Views

The in-memory provider allows the definition of views via LINQ queries, using <xref:Microsoft.EntityFrameworkCore.InMemoryEntityTypeBuilderExtensions.ToInMemoryQuery*>:

[!code-csharp[Main](../../../samples/core/Testing/TestingWithoutTheDatabase/InMemoryBloggingControllerTest.cs?name=ToInMemoryQuery)]
